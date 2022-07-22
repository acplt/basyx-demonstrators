#include <ccs_type_conveyor.h>

/* Input Output definitions */

typedef struct CCS_TYPE_CONVEYOR_IO {
    bool Forward;
    bool Backward;
    bool LS01;
    bool LS02;
} CCS_TYPE_CONVEYOR_IO;

typedef struct CCS_TYPE_CONVEYOR_IOCONFIG {
    unsigned int Forward;
    unsigned int Backward;
    unsigned int LS01;
    unsigned int LS02;
} CCS_TYPE_CONVEYOR_IOCONFIG;

static void
ccs_type_conveyor_ioInit(void *context, C3_IO *io) {
    *io = calloc(1, sizeof(CCS_TYPE_CONVEYOR_IO));
}

static void
ccs_type_conveyor_ioRead(void *context, C3_IO io) {
    CCS_TYPE_CONVEYOR_IO *conveyor = (CCS_TYPE_CONVEYOR_IO *)io;
    CCS_TYPE_CONVEYOR_IOCONFIG *address = (CCS_TYPE_CONVEYOR_IOCONFIG *)context;
    ccs_io_readValue_bool(address->Forward, &conveyor->Forward);
    ccs_io_readValue_bool(address->Backward, &conveyor->Backward);
    ccs_io_readValue_bool(address->LS01, &conveyor->LS01);
    ccs_io_readValue_bool(address->LS02, &conveyor->LS02);  
}

void
ccs_type_conveyor_ioAdd(C3_CC *cc) {
    CCS_TYPE_CONVEYOR_IOCONFIG* addresses = calloc(1, sizeof(CCS_TYPE_CONVEYOR_IOCONFIG));
    C3_Info info = C3_CC_getInfo(cc);
    bool result = false;
    result &= ccs_type_generic_findVariable(info, "Forward", &addresses->Forward);
    result &= ccs_type_generic_findVariable(info, "Backward", &addresses->Backward);
    result &= ccs_type_generic_findVariable(info, "Lightdetector", &addresses->LS01);
    result &= ccs_type_generic_findVariable(info, "Lightdetector", &addresses->LS02);
   
    if(result == false){
       
}
    C3_IOConfig ioConfig = C3_IOCONFIG_NULL;
    ioConfig.context = addresses;
    ioConfig.init = ccs_type_conveyor_ioInit;
    ioConfig.read = ccs_type_conveyor_ioRead;
    C3_CC_setIOConfig(cc, ioConfig);
}
/* Operation modes (skills) */

static void
ccs_type_conveyor_FPASS(C3_CC *cc, struct C3_OP_OpMode *opMode, C3_IO io, C3_ES_Order *order,
               const char **workingState, const char **errorState) {
    ccs_type_generic_OpMode *op = (ccs_type_generic_OpMode *)opMode->context;
    CCS_TYPE_CONVEYOR_IO *conveyor = (CCS_TYPE_CONVEYOR_IO *)io;
    C3_Status status = C3_CC_getStatus(cc);
    UA_DateTime now = UA_DateTime_now();

    // Handle an execution state change
    if(status.executionState != op->exState) {
        // Reset state timer and save current state
        op->timer = now;
        op->exState = status.executionState;
        op->workState = 0;
    }

    // Implement execution state behaviour
    
     if(status.executionState == C3_ES_STATE_EXECUTE) {
        if(op->workState == 0) {
            op->workState++;
            *workingState = "Conveyor move forwrd";
            conveyor->Forward = true;
            conveyor->LS01 = true;
            conveyor->LS02 = true;
        }

    } else if(status.executionState == C3_ES_STATE_STOPPING){
        conveyor->Forward = false;
        conveyor->Backward = false;
        conveyor->LS01 = false;
        conveyor->LS02 = false;
        *workingState = "Safe Stop";
        *order = C3_ES_ORDER_SC;
    } else if(status.executionState == C3_ES_STATE_ABORTING){
        conveyor->Forward = false;
        conveyor->Backward = false;
        conveyor->LS01 = false;
        conveyor->LS02 = false;
        *workingState = "Fast Stop";
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
    ccs_type_generic_addOpMode(cc, "FPASS", ccs_type_conveyor_FPASS);

    ccs_type_conveyor_ioAdd(cc);
}