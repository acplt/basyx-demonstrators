#include <ccs_type_robot.h>

/* Input Output definitions */

typedef struct CCS_TYPE_ROBOT_IO {
    bool open;
    bool close;
    bool home;
    bool pickUp;
    bool dropOff;
    bool isInPos;
    bool isOpen;
    bool isClosed;
} CCS_TYPE_ROBOT_IO;

typedef struct CCS_TYPE_ROBOT_IOCONFIG {
    unsigned int open;
    unsigned int close;
    unsigned int home;
    unsigned int pickUp;
    unsigned int dropOff;
    unsigned int isInPos;
    unsigned int isOpen;
    unsigned int isClosed;
} CCS_TYPE_ROBOT_IOCONFIG;

static void
ccs_type_robot_ioInit(void *context, C3_IO *io) {
    *io = calloc(1, sizeof(CCS_TYPE_ROBOT_IO));
}

static void
ccs_type_robot_ioRead(void *context, C3_IO io) {
    CCS_TYPE_ROBOT_IO *robot = (CCS_TYPE_ROBOT_IO *)io;
    CCS_TYPE_ROBOT_IOCONFIG *address = (CCS_TYPE_ROBOT_IOCONFIG *)context;
    ccs_io_readValue_bool(address->isInPos, &robot->isInPos);
    ccs_io_readValue_bool(address->isOpen, &robot->isOpen);
    ccs_io_readValue_bool(address->isClosed, &robot->isClosed);
}

static void
ccs_type_robot_ioWrite(void *context, C3_IO io) {
    CCS_TYPE_ROBOT_IO *robot = (CCS_TYPE_ROBOT_IO *)io;
    CCS_TYPE_ROBOT_IOCONFIG *address = (CCS_TYPE_ROBOT_IOCONFIG *)context;
    ccs_io_writeValue_bool(address->open, robot->open);
    ccs_io_writeValue_bool(address->close, robot->close);
    ccs_io_writeValue_bool(address->home, robot->home);
    ccs_io_writeValue_bool(address->pickUp, robot->pickUp);
    ccs_io_writeValue_bool(address->dropOff, robot->dropOff);
}

static void
ccs_type_robot_ioAdd(C3_CC *cc) {
    CCS_TYPE_ROBOT_IOCONFIG* addresses = calloc(1, sizeof(CCS_TYPE_ROBOT_IOCONFIG));
    C3_Info info = C3_CC_getInfo(cc);
    ccs_type_generic_findVariable(info, "Open", &addresses->open);
    ccs_type_generic_findVariable(info, "Close", &addresses->close);
    ccs_type_generic_findVariable(info, "Home", &addresses->home);
    ccs_type_generic_findVariable(info, "PickUp", &addresses->pickUp);
    ccs_type_generic_findVariable(info, "DropOff", &addresses->dropOff);
    ccs_type_generic_findVariable(info, "InPosition", &addresses->isInPos);
    ccs_type_generic_findVariable(info, "IsOpen", &addresses->isOpen);
    ccs_type_generic_findVariable(info, "IsClosed", &addresses->isClosed);

    C3_IOConfig ioConfig = C3_IOCONFIG_NULL;
    ioConfig.context = addresses;
    ioConfig.init = ccs_type_robot_ioInit;
    ioConfig.read = ccs_type_robot_ioRead;
    ioConfig.write = ccs_type_robot_ioWrite;
    C3_CC_setIOConfig(cc, ioConfig);
}

/* Operation modes (skills) */

static void
ccs_type_robot_PP(C3_CC *cc, struct C3_OP_OpMode *opMode, C3_IO io, C3_ES_Order *order,
               const char **workingState, const char **errorState) {
    ccs_type_generic_OpMode *op = (ccs_type_generic_OpMode *)opMode->context;
    CCS_TYPE_ROBOT_IO *robot = (CCS_TYPE_ROBOT_IO *)io;
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
    if(status.executionState == C3_ES_STATE_RESETTING) {
        if(op->workState == 0) {
            *errorState = C3_ERRORSTATE_NONE;
            if(robot->home && robot->isInPos && robot->isOpen) {
                *workingState = "READY";
                *order = C3_ES_ORDER_SC;
            }else{
                op->workState++;
                *workingState = "Go to home position and open gripper";
                robot->home = true;
                robot->pickUp = false;
                robot->dropOff = false;
                robot->open = true;
                robot->close = false;
            }
        }
        if(robot->isInPos &&  robot->isOpen && (now > (op->timer + UA_DATETIME_SEC))) {
            *workingState = "READY";
            *order = C3_ES_ORDER_SC;
        }
    } else if(status.executionState == C3_ES_STATE_EXECUTE) {
        if(op->workState == 0) {
            op->workState++;
            *workingState = "Go to pick up position";
            robot->home = false;
        }
        if(op->workState == 1) {
            if(now > (op->timer + UA_DATETIME_MSEC * 100)) {
                robot->pickUp = true;
                if(robot->isInPos) {
                    op->workState++;
                    *workingState = "Pick product";
                    robot->close = true;
                    robot->open = false;
                }
            }
        }
        if(op->workState == 2) {
            if(robot->isClosed) {
                op->workState++;
                op->timer = now;
                *workingState = "Go to drop off position";
                robot->pickUp = false;
            }
        }
        if(op->workState == 3) {
            if(now > (op->timer + UA_DATETIME_MSEC * 100)) {
                robot->dropOff = true;
                if(robot->isInPos) {
                    op->workState++;
                    *workingState = "Drop product";
                    robot->close = false;
                    robot->open = true;
                }
            }
        }
        if(op->workState == 4) {
            if(robot->isOpen) {
                op->workState++;
                op->timer = now;
                *workingState = "Go to home position";
                robot->dropOff = false;
            }
        }
        if(op->workState == 5) {
            if(now > (op->timer + UA_DATETIME_MSEC * 100)) {
                robot->home = true;
                if(robot->isInPos) {
                    *workingState = "Done";
                    *order = C3_ES_ORDER_SC;
                }
            }
        }
    } else if(status.executionState == C3_ES_STATE_STOPPING){
        robot->home = false;
        robot->pickUp = false;
        robot->dropOff = false;
        robot->open = false;
        robot->close = false;
        *workingState = "Safe Stop";
        *order = C3_ES_ORDER_SC;
    }else if(status.executionState == C3_ES_STATE_ABORTING){
        robot->home = false;
        robot->pickUp = false;
        robot->dropOff = false;
        robot->open = false;
        robot->close = false;
        *workingState = "Fast Stop";
        *order = C3_ES_ORDER_SC;
    }else if(C3_ES_ISACTIVESTATE[status.executionState]) {
        // An active state, that was not covered / implemented (e.g. HOLDING, UNHOLDING)
        // --> Just skip through
        *workingState = "Done";
        *order = C3_ES_ORDER_SC;
    }
}

/* Instanciate the control component */

void ccs_type_robot(C3_CC* cc){
    ccs_type_generic_addOpMode(cc, "PP", ccs_type_robot_PP);
    ccs_type_robot_ioAdd(cc);
}