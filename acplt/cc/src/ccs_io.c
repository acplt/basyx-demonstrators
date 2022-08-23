#include <ccs_io.h>

static CCS_IO_SHMHANDLE shmHandle = {0};

void ccs_io_closeSHM(){
#ifdef WIN32
    if(shmHandle.hMapFile != NULL) CloseHandle(shmHandle.hMapFile);
#endif
    shmHandle.size = 0;
    shmHandle.shm = NULL;
    shmHandle.hMapFile = NULL;
}

bool ccs_io_openSHM(char* name, unsigned int size){
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
    	                size*SHM_VALUESIZE);
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

unsigned int ccs_io_swappValue(unsigned int value){
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

bool ccs_io_readValue(unsigned int address, CCS_IO_SHMANY * pValue) {
	bool result = true;

	//check value pointer not NULL
	if (pValue == NULL) return false;

	// Check if shm already open
    if(shmHandle.shm == NULL || shmHandle.size == 0){
    	result = false;
    }

	// Try to read from SHM
    if(result){
    	if(address < shmHandle.size){
    	    pValue->uint = (shmHandle.swap) ? ccs_io_swappValue(shmHandle.shm[address]) :  shmHandle.shm[address];
        	result = true;
    	}else{
    		result = false;
    	}
    }
    return result;
}

bool ccs_io_readValue_bool(unsigned int address, bool * pValue) {
	CCS_IO_SHMANY value;
	bool result = ccs_io_readValue(address, &value);
	if(result == true) *pValue = (value.boolean != false);
	return result;
}

bool ccs_io_readValue_int(unsigned int address, int * pValue) {
	CCS_IO_SHMANY value;
	bool result = ccs_io_readValue(address, &value);
	if(result == true) *pValue = value.integer;
	return result;
}

bool ccs_io_readValue_uint(unsigned int address, unsigned int * pValue) {
	CCS_IO_SHMANY value;
	bool result = ccs_io_readValue(address, &value);
	if(result == true) *pValue = value.uint;
	return result;
}

bool ccs_io_readValue_real(unsigned int address, float * pValue) {
	CCS_IO_SHMANY value;
	bool result = ccs_io_readValue(address, &value);
	if(result == true) *pValue = value.real;
	return result;
}


bool ccs_io_writeValue(unsigned int address, CCS_IO_SHMANY value){
	bool result = true;
	// Check if shm already open
    if(shmHandle.shm == NULL || shmHandle.size == 0){
    	result = false;
    }

	if(shmHandle.swap)
		value.uint = ccs_io_swappValue(value.uint);

	if(result){
    	if(address < shmHandle.size){
	    	shmHandle.shm[address] = value.uint;
        	result = true;
    	}else{
    		result = false;
    	}
    }
	return result;
}

bool ccs_io_writeValue_bool(unsigned int address, bool value){
	CCS_IO_SHMANY writeValue;
	writeValue.boolean = (value != false);
	return ccs_io_writeValue(address, writeValue);
}
bool ccs_io_writeValue_int(unsigned int address, int value){
	CCS_IO_SHMANY writeValue;
	writeValue.integer = value;
	return ccs_io_writeValue(address, writeValue);
}
bool ccs_io_writeValue_uint(unsigned int address, unsigned int value){
	CCS_IO_SHMANY writeValue;
	writeValue.uint = value;
	return ccs_io_writeValue(address, writeValue);
}
bool ccs_io_writeValue_real(unsigned int address, float value){
	CCS_IO_SHMANY writeValue;
	writeValue.real = value;
	return ccs_io_writeValue(address, writeValue);
}

bool ccs_io_writeValue_string(unsigned int address, char* value, unsigned int maxSize){
	bool result = true;

	// Check if shm already open
    if(shmHandle.shm == NULL || shmHandle.size == 0){
    	result = false;
    }

    if(result){
		if(address < shmHandle.size){
			unsigned int size = strlen(value);
			if(size > maxSize) size = maxSize;
			memset(&shmHandle.shm[address], '\0' , sizeof(char) * maxSize);
			memcpy(&shmHandle.shm[address], value, sizeof(char) * size);
			result = true;
		}else{
			result = false;
		}
	}
	return result;
}