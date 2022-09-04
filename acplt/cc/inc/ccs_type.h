#ifndef CCS_TYPE_H
#define CCS_TYPE_H

#include <ccs_type_generic.h>
#include <ccs_io_generic.h>

#define CCS_TYPE_COUNT 3

typedef enum CCS_TYPE {
    CCS_TYPE_ROBOT,
    CCS_TYPE_CONVEYOR,
    CCS_TYPE_COVERUNIT,
} CCS_TYPE;

static const char* CCS_TYPE_NAMES[CCS_TYPE_COUNT] = {
    "Robot",
    "BeltConveyor",
    "CoverUnit",
};

void ccs_type_robot(C3_CC* cc);
void ccs_type_conveyor(C3_CC* cc);
void ccs_type_coverUnit(C3_CC* cc);

static const void (*CCS_TYPE_FUNCTIONS[CCS_TYPE_COUNT])(C3_CC* cc) = {
    ccs_type_robot,
    ccs_type_conveyor,
    ccs_type_coverUnit,
};

C3_CC* ccs_type_createInstance(char* name, CCS_TYPE type);

#endif /* CCS_TYPE_H */