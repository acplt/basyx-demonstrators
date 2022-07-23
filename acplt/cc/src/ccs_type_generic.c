#include <ccs_type_generic.h>

static bool
ccs_type_generic_init(C3_CC *cc, C3_OP_OpMode *opMode) {
    ccs_type_generic_OpMode *context = (ccs_type_generic_OpMode *)opMode->context;
    context->workState = -2;
    context->timer = 0;
    return true;
}

static void
ccs_type_generic_clear(C3_CC *cc, struct C3_OP_OpMode *opMode) {
    free(opMode->context);
}

static bool
ccs_type_generic_select(C3_CC *cc, struct C3_OP_OpMode *opMode) {
    const C3_Status status = C3_CC_getStatus(cc);
    if(C3_ES_ISACTIVESTATE[status.executionState]) {
        // Don't allow operation mode changes in active states
        // This could be even more permissive (e.g. only change operatin mode in STOPPED)
        return false;
    }

    ccs_type_generic_OpMode *context = (ccs_type_generic_OpMode *)opMode->context;
    context->exState = -1;
    context->workState = -1;
    context->timer = 0;
    return true;
}

static bool
ccs_type_generic_deselect(C3_CC *cc, struct C3_OP_OpMode *opMode) {
    const C3_Status status = C3_CC_getStatus(cc);
    if(C3_ES_ISACTIVESTATE[status.executionState]) {
        // Don't allow operation mode changes in active states
        // This could be even more permissive (e.g. only change operatin mode in STOPPED)
        return false;
    }
    return true;
}

void
ccs_type_generic_addOpMode(C3_CC* cc, char* name, void* opModeExecute) {
    C3_OP_OpMode opMode;
    opMode.context = malloc(sizeof(ccs_type_generic_OpMode));
    opMode.name = name;
    opMode.init = ccs_type_generic_init;
    opMode.execute = opModeExecute;
    opMode.clear = ccs_type_generic_clear;
    opMode.select = ccs_type_generic_select;
    opMode.deselect = ccs_type_generic_deselect;
    if(C3_CC_addOperationMode(cc, &opMode) < 0) {
        free(opMode.context);
    }
}

bool
ccs_type_generic_findVariableByName(char* name, char* type, char* variable, unsigned int* address) {
    char* signalName = malloc(sizeof(char) * (strlen(name)+strlen(type)+strlen(variable)+3));
    sprintf(signalName, "%s_%s.%s", name, type, variable);
    for (size_t i = 0; i < CCS_IOLIST_SIZE; i++) {
        if(strcmp(CCS_IOLIST[i].name, signalName) == 0) {
            *address = CCS_IOLIST[i].address;
            free(signalName);
            return false;
        }
    }
    free(signalName);
    return false;
}

bool
ccs_type_generic_findVariable(C3_Info info, char* variable, unsigned int* address) {
    return ccs_type_generic_findVariableByName(info.name, info.type, variable, address);
}
