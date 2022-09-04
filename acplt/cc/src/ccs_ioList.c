#include <ccs_ioList.h>

char* CCS_IOLIST_SHMNAME = "Local\\Hilt3DShm";
unsigned int CCS_IOLIST_SIZE = 0;
CCS_IO_SHMVARIABLE* CCS_IOLIST = {0};

bool
ccs_ioList_readIOConfigurationFromXML(char* filepath){
    FILE * fp;
    char line[512];
    size_t len = 0;
    ssize_t read;

    fp = fopen(filepath, "r");
    if (fp == NULL)
        return false;

    size_t lines = 0;
    int ch = 0;
    while(!feof(fp)) {
        ch = fgetc(fp);
        if(ch == '\n')
            lines++;
    }
    if(lines == 9)
        return false;
    rewind(fp);
    CCS_IOLIST = calloc(lines, sizeof(CCS_IO_SHMVARIABLE));

    fgets(line, 511 , fp);
    fgets(line, 511 , fp);
    char *shmNamePos = strstr(line, "shm=\"");
    if(shmNamePos != NULL) {
        char shmName[64];     
        if(sscanf(shmNamePos, "shm=\"%63s\"", shmName)==1) {
            size_t shmNameLength = strlen(shmName);
            CCS_IOLIST_SHMNAME = malloc(sizeof(char) * (shmNameLength + 6)); // "Local\\"
            strncpy(CCS_IOLIST_SHMNAME, "Local\\", 6);
            strncpy(&CCS_IOLIST_SHMNAME[6], shmName, shmNameLength-1); //cut off trailing "
            CCS_IOLIST_SHMNAME[5 + shmNameLength] = '\0';
        }
    }

    size_t signalCount = 0;
    for (size_t i=0 ; i < lines ; i++ ) {
        fgets(line, 511 , fp);
        if(strstr(line, "<ioelement") == NULL)
            continue;

        char *namePos = strstr(line, "name=\"");
        if(namePos == NULL)
            continue;        
        char name[64];
        if(sscanf(namePos, "name=\"%63s\"", name)!=1)
            continue;

        char *addressPos = strstr(line, "signal=\"");
        if(addressPos == NULL)
            continue;
        unsigned int address = 0;
        if(sscanf(addressPos, "signal=\"%d\"", &address)!=1)
            continue;

        CCS_IO_SHMDATATYPE type = CCS_IO_SHMDATATYPE_UNDEFINED;
        if(strstr(line, "type=\"bool\""))
            type = CCS_IO_SHMDATATYPE_BOOL;
        else if(strstr(line, "type=\"int\""))
            type = CCS_IO_SHMDATATYPE_INT;
        else if(strstr(line, "type=\"uint\""))
            type = CCS_IO_SHMDATATYPE_UINT;
        else if(strstr(line, "type=\"float\""))
            type = CCS_IO_SHMDATATYPE_REAL;
        //else if(strstr(line, "type=\"string\""))
        //    type = CCS_IO_SHMDATATYPE_string;
        else
            continue;

        if(strstr(line, "direction=\"FromSHM\""))
            CCS_IOLIST[signalCount].isOutput = true;

        size_t nameLen = strlen(name);
        CCS_IOLIST[signalCount].name = malloc(sizeof(char) * nameLen);
        strncpy(CCS_IOLIST[signalCount].name, name, nameLen-1); //cut off trailing "
        CCS_IOLIST[signalCount].name[nameLen-1] = '\0';
        CCS_IOLIST[signalCount].address = address;
        CCS_IOLIST[signalCount].datatype = type;
        signalCount++;
    }
    CCS_IOLIST_SIZE = signalCount;
    CCS_IOLIST = realloc(CCS_IOLIST, signalCount * sizeof(CCS_IO_SHMVARIABLE));

    fclose(fp);
    return true;
}