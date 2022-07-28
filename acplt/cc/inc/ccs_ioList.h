#ifndef CCS_IOLIST_H
#define CCS_IOLIST_H

#include <stdio.h>
#include <stdbool.h>
#include <string.h>
#include <stdlib.h>

typedef enum {
    CCS_IO_SHMDATATYPE_BOOL,
    CCS_IO_SHMDATATYPE_INT,
    CCS_IO_SHMDATATYPE_UINT,
    CCS_IO_SHMDATATYPE_REAL,
    CCS_IO_SHMDATATYPE_STRING,
    CCS_IO_SHMDATATYPE_UNDEFINED
} CCS_IO_SHMDATATYPE;

typedef struct {
	unsigned int address;
    char* name;
    CCS_IO_SHMDATATYPE datatype;
} CCS_IO_SHMVARIABLE;

extern unsigned int CCS_IOLIST_SIZE;
extern CCS_IO_SHMVARIABLE* CCS_IOLIST;
extern char* CCS_IOLIST_SHMNAME;

bool ccs_ioList_readIOConfigurationFromXML(char* filepath);

#endif /* CCS_IOLIST_H */