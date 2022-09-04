#ifndef CCS_IO_H
#define CCS_IO_H

#ifdef WIN32
#include <windows.h>
#endif

#include <stdlib.h>
#include <stdbool.h>
#include <string.h>

typedef struct {
	void * hMapFile;
	unsigned int* shm;
	unsigned int size;
    bool swap;
} CCS_IO_SHMHANDLE;

/*
 * SHM Handle open/close resource
 */
void ccs_io_closeSHM();
bool ccs_io_openSHM(char* name, unsigned int size);

/*
 * Read values.
 */
bool ccs_io_readValue(unsigned int address, unsigned int* value);

/*
 * Write values.
 */
bool ccs_io_writeValue(unsigned int address, unsigned int value);
bool ccs_io_writeValue_string(unsigned int address, char* value, unsigned int maxSize);

#endif /* CCS_IO_H */