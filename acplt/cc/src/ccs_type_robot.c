#include <ccs_type.h>

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
    CCS_TYPE_ROBOT_IO *io = calloc(1,sizeof(CCS_TYPE_ROBOT_IO));
    ccs_io_generic_addByNames(cc, (C3_IO*)io, 8,
        (char*[8]){"Open","Close","Home","PickUp","DropOff","InPosition","IsOpen","IsClosed"},
        (unsigned int*)(void*[8]){&io->open, &io->close, &io->home, &io->pickUp, &io->dropOff, &io->isInPos, &io->isOpen, &io->isClosed}
    );

    ccs_type_generic_addOpMode(cc, "PP", ccs_type_robot_PP);
}