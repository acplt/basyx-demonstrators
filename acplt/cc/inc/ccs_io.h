#ifndef CCS_IO_H
#define CCS_IO_H

#ifdef WIN32
#include <windows.h>
#endif

#include <stdlib.h>
#include <stdbool.h>
#include <string.h>

#define SHM_DEFAULT_ADDRESS 0
#define SHM_VALUESIZE 4		  //Every value is 4 Byte big.

typedef struct {
	void * hMapFile;
	unsigned int* shm;
	unsigned int size;
    bool swap;
} CCS_IO_SHMHANDLE;

//DataTypes
typedef enum {
    CCS_IO_SHMDATATYPE_BOOL,
    CCS_IO_SHMDATATYPE_INT,
    CCS_IO_SHMDATATYPE_UINT,
    CCS_IO_SHMDATATYPE_REAL,
    CCS_IO_SHMDATATYPE_STRING,
    CCS_IO_SHMDATATYPE_UNDEFINED
} CCS_IO_SHMDATATYPE;

typedef union {
	bool boolean;
	int integer;
	unsigned int uint;
	float real;
	char string[4];
} CCS_IO_SHMANY;

typedef struct {
	unsigned int address;
    char* name;
    CCS_IO_SHMDATATYPE datatype;
} CCS_IO_SHMVARIABLE;

/*
 * SHM Handle open/close resource
 */
 void ccs_io_closeSHM();
 bool ccs_io_openSHM(char* name, unsigned int size);

/*
 * Read values.
 */
 bool ccs_io_readValue(unsigned int address, CCS_IO_SHMANY * pValue);
 bool ccs_io_readValue_bool(unsigned int address, bool* pValue);
 bool ccs_io_readValue_int(unsigned int address, int* pValue);
 bool ccs_io_readValue_uint(unsigned int address, unsigned int* pValue);
 bool ccs_io_readValue_real(unsigned int address, float* pValue);

/*
 * Write values.
 */
 bool ccs_io_writeValue(unsigned int address, CCS_IO_SHMANY value);
 bool ccs_io_writeValue_bool(unsigned int address, bool value);
 bool ccs_io_writeValue_int(unsigned int address, int value);
 bool ccs_io_writeValue_uint(unsigned int address, unsigned int value);
 bool ccs_io_writeValue_real(unsigned int address, float value);
 bool ccs_io_writeValue_string(unsigned int address, char* value, unsigned int maxSize);

#endif /* CCS_IO_H */