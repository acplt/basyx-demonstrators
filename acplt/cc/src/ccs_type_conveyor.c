#include <ccs_type_conveyor.h>

/* Input Output definitions */

typedef struct CCS_TYPE_CONVEYOR_IO {
    bool forward;
    bool backward;
    bool entry;
    bool exit;
} CCS_TYPE_CONVEYOR_IO;

typedef struct CCS_TYPE_CONVEYOR_IOCONFIG {
    unsigned int forward;
    unsigned int backward;
    unsigned int entry;
    unsigned int exit;
} CCS_TYPE_CONVEYOR_IOCONFIG;

static void
ccs_type_conveyor_ioInit(void *context, C3_IO *io) {
    *io = calloc(1, sizeof(CCS_TYPE_CONVEYOR_IO));
}

static void
ccs_type_conveyor_ioRead(void *context, C3_IO io) {
    CCS_TYPE_CONVEYOR_IO *conveyor = (CCS_TYPE_CONVEYOR_IO *)io;
    CCS_TYPE_CONVEYOR_IOCONFIG *address = (CCS_TYPE_CONVEYOR_IOCONFIG *)context;
    ccs_io_readValue_bool(address->entry, &conveyor->entry);
    ccs_io_readValue_bool(address->exit, &conveyor->exit);  
}

static void
ccs_type_conveyor_ioWrite(void *context, C3_IO io) {
    CCS_TYPE_CONVEYOR_IO *robot = (CCS_TYPE_CONVEYOR_IO *)io;
    CCS_TYPE_CONVEYOR_IOCONFIG *address = (CCS_TYPE_CONVEYOR_IOCONFIG *)context;
    ccs_io_writeValue_bool(address->forward, robot->forward);
    ccs_io_writeValue_bool(address->backward, robot->backward);
}

static void
ccs_type_conveyor_ioAdd(C3_CC *cc) {
    CCS_TYPE_CONVEYOR_IOCONFIG* addresses = calloc(1, sizeof(CCS_TYPE_CONVEYOR_IOCONFIG));
    C3_Info info = C3_CC_getInfo(cc);
    ccs_type_generic_findVariable(info, "Forward", &addresses->forward);
    ccs_type_generic_findVariable(info, "Backward", &addresses->backward);
    char lsName[16];
    snprintf(lsName, 15, "%sLS01",info.name); //GF01LS01
    ccs_type_generic_findVariableByName(lsName, "LightBarrier" , "Detected", &addresses->entry);
    snprintf(lsName, 15, "%sLS02",info.name); //GF01LS02
    ccs_type_generic_findVariableByName(lsName, "LightBarrier", "Detected", &addresses->exit);
   
    C3_IOConfig ioConfig = C3_IOCONFIG_NULL;
    ioConfig.context = addresses;
    ioConfig.init = ccs_type_conveyor_ioInit;
    ioConfig.read = ccs_type_conveyor_ioRead;
    ioConfig.write = ccs_type_conveyor_ioWrite;
    C3_CC_setIOConfig(cc, ioConfig);
}

/* Operation modes (skills) */

static void
ccs_type_conveyor_Position(C3_CC *cc, struct C3_OP_OpMode *opMode, C3_IO io, C3_ES_Order *order,
               const char **workingState, const char **errorState) {
    ccs_type_generic_OpMode *op = (ccs_type_generic_OpMode *)opMode->context;
    CCS_TYPE_CONVEYOR_IO *conveyor = (CCS_TYPE_CONVEYOR_IO *)io;
    C3_Status status = C3_CC_getStatus(cc);
    UA_DateTime now = UA_DateTime_now();

    bool forward = opMode->name[0] == 'F';

    // Handle an execution state change
    if(status.executionState != op->exState) {
        // Reset state timer and save current state
        op->timer = now;
        op->exState = status.executionState;
        op->workState = 0;
    }

    // Implement execution state behaviour
     if(status.executionState == C3_ES_STATE_EXECUTE) {
        switch(op->workState) {
            case 0: // Switch on motor
                *workingState = "Move in";
                conveyor->forward = forward;
                conveyor->backward = !forward;
                op->workState++;
            case 1: // Wait till last lightbarrier is on
                if((forward ? conveyor->exit : conveyor->entry)) op->workState++;
                break;
            case 2: // TAKE or PASS
                if(strncmp(&opMode->name[1], "PASS", 4) == 0) { //Check if name of operation mode has TAKE in it (FPASS, BPASS)
                    *workingState = "Pass";
                    op->workState++;
                }else{
                    op->workState = 6;
                }
            case 3: // PASS: Wait till last lightbarrier is off
                if(!(forward ? conveyor->exit : conveyor->entry)) op->workState++;
                break;
            case 4:
                *workingState = "Leaving"; // Deutsch: Nachlauf
                op->timer = now;
                op->workState++;
                break;
            case 5: // PASS done
                if(now > (op->timer + UA_DATETIME_SEC)) {
                    conveyor->forward = false;
                    conveyor->backward = false;
                    *order = C3_ES_ORDER_SC;
                    break;
                }
                break;
            case 6: // TAKE
                *workingState = "Positioning"; // Deutsch: Nachlauf
                op->timer = now;
                op->workState++;
                break;
            case 7:
                if(now > (op->timer + UA_DATETIME_MSEC * 50)) {
                    conveyor->forward = false;
                    conveyor->backward = false;
                    *order = C3_ES_ORDER_SC;
                }
                break;
        }
    } else if(status.executionState == C3_ES_STATE_COMPLETING) {
        if(strncmp(&opMode->name[1], "PASS", 4) == 0) //Check if name of operation mode has TAKE in it (FPASS, BPASS)
            *workingState = forward ? "Passed exit" : "Passed entry";
        else
            *workingState = forward ? "Positioned at exit" : "Positioned at entry";
        *order = C3_ES_ORDER_SC;
    } else if(status.executionState == C3_ES_STATE_STOPPING ||
              status.executionState == C3_ES_STATE_ABORTING){
        *workingState = "Stop motor";
        conveyor->forward = false;
        conveyor->backward = false;
        *order = C3_ES_ORDER_SC;
    } else if(C3_ES_ISACTIVESTATE[status.executionState]) {
        // An active state, that was not covered / implemented (e.g. HOLDING, UNHOLDING)
        // --> Just skip through
        *workingState = "Done";
        *order = C3_ES_ORDER_SC;
    }
}

/* Instanciate the control component */

void ccs_type_conveyor(C3_CC* cc){
    ccs_type_generic_addOpMode(cc, "FPASS", ccs_type_conveyor_Position);
    ccs_type_generic_addOpMode(cc, "FTAKE", ccs_type_conveyor_Position);
    ccs_type_generic_addOpMode(cc, "BPASS", ccs_type_conveyor_Position);
    ccs_type_generic_addOpMode(cc, "BTAKE", ccs_type_conveyor_Position);

    ccs_type_conveyor_ioAdd(cc);
}