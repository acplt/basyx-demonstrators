#include <ccs_io.h>

static CCS_IO_SHMHANDLE shmHandle = {0};

void ccs_io_closeSHM() {
#ifdef WIN32
    if(shmHandle.hMapFile != NULL) CloseHandle(shmHandle.hMapFile);
#endif
    shmHandle.size = 0;
    shmHandle.shm = NULL;
    shmHandle.hMapFile = NULL;
}

bool ccs_io_openSHM(char* name, unsigned int size) {
#ifdef WIN32
    void * hMapFile = OpenFileMapping(
                    FILE_MAP_ALL_ACCESS,   // read/write access
                    false,                 // do not inherit the name
					name);               // name of mapping object
    if (hMapFile != NULL) {
    	unsigned int* shm = (unsigned int*) MapViewOfFile(hMapFile, // handle to map object
    	                FILE_MAP_ALL_ACCESS,  // read/write permission
    	                0,
    	                0,
    	                size*4); //Every value is 4 Byte big.
        if (shm != NULL){
        	shmHandle.shm = shm;
            shmHandle.hMapFile = hMapFile;
            shmHandle.size = size;
            return true;
        }else{
            CloseHandle(hMapFile);
            ccs_io_closeSHM(shmHandle);
        	return false;
        }
    }else{
    	return false;
    }
#endif
    return false;
}

unsigned int ccs_io_swappValue(unsigned int value) {
	char *b;
	unsigned int b0,b1,b2,b3;

	//interprete value as 4 bytes
	b = (char*) &value;

	//Filter relevant byte for every 4byte value
	b0 = (b[0] << 24u)& 0xff000000;
	b1 = (b[1] << 16u)& 0x00ff0000;
	b2 = (b[2] << 8u) & 0x0000ff00;
	b3 = (b[3] << 0u) & 0x000000ff;

	return (unsigned int)(b3 | b2 | b1 | b0);
}

bool ccs_io_readValue(unsigned int address, unsigned int* value) {
	if (value == NULL)
		return false;
    if(shmHandle.shm == NULL || shmHandle.size == 0)
		return false;
	if(address >= shmHandle.size)
		return false;
	*value = shmHandle.swap ? ccs_io_swappValue(shmHandle.shm[address]) : shmHandle.shm[address];
	return true;
}

bool ccs_io_writeValue(unsigned int address, unsigned int value) {
    if(shmHandle.shm == NULL || shmHandle.size == 0)
		return false;
    if(address >= shmHandle.size)
		return false;
	shmHandle.shm[address] = shmHandle.swap ? ccs_io_swappValue(value) : value;
	return true;
}

bool ccs_io_writeValue_string(unsigned int address, char* value, unsigned int maxSize) {
    if(shmHandle.shm == NULL || shmHandle.size == 0)
    	return false;
	if(address >= shmHandle.size)
		return false;

	unsigned int size = strlen(value);
	if(size > maxSize) size = maxSize;
	memset(&shmHandle.shm[address], '\0' , sizeof(char) * maxSize);
	memcpy(&shmHandle.shm[address], value, sizeof(char) * size);

	return true;
}