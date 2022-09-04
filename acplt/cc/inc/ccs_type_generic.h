#ifndef CCS_TYPE_GENERIC_H
#define CCS_TYPE_GENERIC_H

#include <C3_ControlComponent.h>
#include <open62541/types.h>

typedef struct ccs_type_generic_OpMode {
    int workState;
    C3_ES_State exState;
    UA_DateTime timer;
} ccs_type_generic_OpMode;

void ccs_type_generic_addOpMode(C3_CC* cc, char* name, void* opModeExecute);

#endif /* CCS_TYPE_GENERIC_H */