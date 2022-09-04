#include <ccs_type.h>

/* Input Output definitions */

typedef struct CCS_TYPE_COVERUNIT_IO {
    bool up;
    bool down;
    bool unlock;
    bool isDetected;
    bool isExtracted;
    bool isRetracted;
    bool isUnlocked;
} CCS_TYPE_COVERUNIT_IO;

/* Operation modes (skills) */

static void
ccs_type_coverUnit_Assemble(C3_CC *cc, struct C3_OP_OpMode *opMode, C3_IO io, C3_ES_Order *order,
               const char **workingState, const char **errorState) {
    ccs_type_generic_OpMode *op = (ccs_type_generic_OpMode *)opMode->context;
    CCS_TYPE_COVERUNIT_IO *unit = (CCS_TYPE_COVERUNIT_IO *)io;
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
        if(op->workState == 0)
            *errorState = C3_ERRORSTATE_NONE;
        if(unit->isUnlocked){
            unit->unlock = false;
            if(op->workState!=1) {
                *workingState = "Lock cover";
                op->workState = 1;
            }
        } else if(!unit->isRetracted) {
            unit->up = true;
            unit->down = false;
            if(op->workState!=2) {
                *workingState = "Move up";
                op->workState = 2;
            }
        } else {
            *workingState = "READY";
            *errorState = C3_ERRORSTATE_NONE;
            *order = C3_ES_ORDER_SC;
        }
    } else if(status.executionState == C3_ES_STATE_STARTING) {
        if(!unit->isDetected){
            unit->unlock = true;
            if(op->workState!=1) {
                *workingState = "Wait for cover";
                op->workState = 1;
            }
        } else {
            unit->unlock = false;
            if(op->workState!=2) {
                *workingState = "Lock cover";
                op->workState = 2;
                op->timer = now;
            } else if(!unit->isUnlocked) {
                *workingState = "Cover locked";
                *order = C3_ES_ORDER_SC;
            }
        }
    } else if(status.executionState == C3_ES_STATE_EXECUTE) {
        if(op->workState == 0) {
            op->workState++;
            *workingState = "Press down";
            unit->down = true;
            unit->up = false;
        }
        if(op->workState == 1) {
            if(unit->isExtracted && !unit->isRetracted) {
                op->workState++;
                *workingState = "Move up";
                unit->up = true;
                unit->down = false;
            }
        }
        if(op->workState == 2) {
            if(unit->isRetracted && !unit->isExtracted) {
                *workingState = "Done";
                *order = C3_ES_ORDER_SC;
            }
        }
    } else if(status.executionState == C3_ES_STATE_STOPPING){
        unit->up = false;
        unit->down = false;
        unit->unlock = false;
        *workingState = "Safe Stop";
        *order = C3_ES_ORDER_SC;
    }else if(status.executionState == C3_ES_STATE_ABORTING){
        unit->up = false;
        unit->down = false;
        unit->unlock = false;
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

void ccs_type_coverUnit(C3_CC* cc){   
    CCS_TYPE_COVERUNIT_IO *io = calloc(1,sizeof(CCS_TYPE_COVERUNIT_IO));
    ccs_io_generic_addByNames(cc, (C3_IO*)io, 7,
        (char*[7]){"Up","Down","Unlock","Detected","Extracted","Retracted","Unlocked"},
        (unsigned int**)(void*[7]){&io->up, &io->down, &io->unlock, &io->isDetected, &io->isExtracted, &io->isRetracted, &io->isUnlocked}
    );

    ccs_type_generic_addOpMode(cc, "ASSEMBLY", ccs_type_coverUnit_Assemble);
}