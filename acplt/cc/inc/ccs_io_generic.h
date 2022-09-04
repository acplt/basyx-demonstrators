#ifndef CCS_IO_GENERIC_H
#define CCS_IO_GENERIC_H

#include <C3_ControlComponent.h>
#include <ccs_ioList.h>
#include <ccs_io.h>

typedef struct CCS_TYPE_GENERIC_IOCONFIG {
    size_t size;
    CCS_IO_SHMVARIABLE* variables;
    unsigned int** values;
    C3_IO io;
} CCS_TYPE_GENERIC_IOCONFIG;

bool ccs_io_generic_findVariableByName(char* name, char* type, char* variable, CCS_IO_SHMVARIABLE* variableOut);
bool ccs_io_generic_findVariable(C3_Info info, char* variable, CCS_IO_SHMVARIABLE* variableOut);

void ccs_io_generic_add(C3_CC *cc, C3_IO* io, size_t size, CCS_IO_SHMVARIABLE* variables, unsigned int** values);
void ccs_io_generic_addByNames(C3_CC *cc, C3_IO* io, size_t size, char** names, unsigned int** values);

#endif /* CCS_IO_GENERIC_H */