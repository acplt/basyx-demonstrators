#include <ccs_io_generic.h>

bool
ccs_io_generic_findVariableByName(char* name, char* type, char* variable, CCS_IO_SHMVARIABLE* variableOut) {
    char* signalName = malloc(sizeof(char) * (strlen(name)+strlen(type)+strlen(variable)+3));
    sprintf(signalName, "%s_%s.%s", name, type, variable);
    for (size_t i = 0; i < CCS_IOLIST_SIZE; i++) {
        if(strcmp(CCS_IOLIST[i].name, signalName) == 0) {
            *variableOut = CCS_IOLIST[i];
            free(signalName);
            return true;
        }
    }
    fprintf(stderr, "Error. Didn't find signal in IO list: %s\n", signalName);
    free(signalName);
    return false;
}

bool
ccs_io_generic_findVariable(C3_Info info, char* variable, CCS_IO_SHMVARIABLE* variableOut) {
    return ccs_io_generic_findVariableByName(info.name, info.type, variable, variableOut);
}

static void
ccs_io_generic_init(void *context, C3_IO *io) {
    CCS_TYPE_GENERIC_IOCONFIG *config = (CCS_TYPE_GENERIC_IOCONFIG *)context;
    *io = config->io;
    for (size_t i = 0; i < config->size; i++) {
        ccs_io_readValue(config->variables[i].address, &config->values[i]);
    }
}

static void
ccs_io_generic_clear(void *context) {
    free(((CCS_TYPE_GENERIC_IOCONFIG *)context)->values);
    free(context);
}

static void
ccs_io_generic_read(void *context, C3_IO io) {
    CCS_TYPE_GENERIC_IOCONFIG *config = (CCS_TYPE_GENERIC_IOCONFIG *)context;
    for (size_t i = 0; i < config->size; i++) {
        if(!config->variables[i].isOutput)
            ccs_io_readValue(config->variables[i].address, &config->values[i]);
    }
}

static void
ccs_io_generic_write(void *context, C3_IO io) {
    CCS_TYPE_GENERIC_IOCONFIG *config = (CCS_TYPE_GENERIC_IOCONFIG *)context;
    for (size_t i = 0; i < config->size; i++) {
        if(!config->variables[i].isOutput)
            ccs_io_writeValue(config->variables[i].address, config->values[i]);
    }
}

void
ccs_io_generic_add(C3_CC *cc, C3_IO* io, size_t size, CCS_IO_SHMVARIABLE* variables, unsigned int* values){
    CCS_TYPE_GENERIC_IOCONFIG* config = calloc(1, sizeof(CCS_TYPE_GENERIC_IOCONFIG));
    config->variables = variables;
    config->size = size;
    config->values = values;
    config->io = io;

    C3_IOConfig ioConfig = C3_IOCONFIG_NULL;
    ioConfig.context = config;
    ioConfig.init = ccs_io_generic_init;
    ioConfig.read = ccs_io_generic_read;
    ioConfig.write = ccs_io_generic_write;
    ioConfig.clear = ccs_io_generic_clear;
    C3_CC_setIOConfig(cc, ioConfig);
}

void
ccs_io_generic_addByNames(C3_CC *cc, C3_IO* io, size_t size, char** names, unsigned int* values) {
    CCS_IO_SHMVARIABLE* variables = calloc(size, sizeof(CCS_IO_SHMVARIABLE));
    C3_Info info = C3_CC_getInfo(cc);
    for (size_t i = 0; i < size; i++) {
        ccs_io_generic_findVariable(info, names[i], &variables[i]);
    }
    ccs_io_generic_add(cc, io, size, variables, values);
}