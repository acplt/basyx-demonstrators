#ifndef CCS_TYPE_GENERIC_H
#define CCS_TYPE_GENERIC_H

#include <C3_ControlComponent.h>
#include <open62541/types.h>
#include <ccs_ioList.h>
#include <ccs_io.h>

typedef struct ccs_type_generic_OpMode {
    int workState;
    C3_ES_State exState;
    UA_DateTime timer;
} ccs_type_generic_OpMode;

void ccs_type_generic_addOpMode(C3_CC* cc, char* name, void* opModeExecute);

bool ccs_type_generic_findVariable(C3_Info info, char* variable, unsigned int* address);

#endif /* CCS_TYPE_GENERIC_H */