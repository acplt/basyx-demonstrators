#include <cc_connector.h>
#include <cc_instanciation.h>

#include <profile_utilities.h>

#include <signal.h>
#include <stdio.h>
#include <stdlib.h>

#include <ccs_io.h>

UA_Boolean running = true;
static void
stopHandler(int sig) {
    running = false;
}

static void
usage(void) {
    printf("Usage: ccs_server");
    printf(" [--port <port>]");
    printf(" [--help]\n");
}
static void
usageError(const char *errorMsg) {
    fprintf(stderr, "Error: %s\n", errorMsg);
    usage();
}

static int
parseCLI(int argc, char **argv, UA_UInt16 *port) {
    for(int argpos = 1; argpos < argc; argpos++) {
        if(strcmp(argv[argpos], "--help") == 0 || strcmp(argv[argpos], "-h") == 0) {
            usage();
            printf(
                "\n\tExample: ccs_server --port 16664\n");
            return EXIT_FAILURE;
        }
        // parse server url
        if(strcmp(argv[argpos], "--port") == 0) {
            argpos++;
            if(argpos >= argc) {
                usageError("parameter --port given, but no port specified!");
                return EXIT_FAILURE;
            }
            *port = strtol(argv[argpos], NULL, 10);
            continue;
        }
        // Unknown option
        usageError("unknown command line option.");
        return EXIT_FAILURE;
    }
    return EXIT_SUCCESS;
}

static void
createControlComponentFromType(UA_Server *server, size_t type) {
    /* Create control component and fill in example informations */
    C3_CC *cc = C3_CC_new();
    C3_Info info;
    info.id = "CC1";
    info.name = "CC1";
    info.description = "Represents the control component 1";
    info.profile = C3_PROFILE_BASYSDEMO;
    info.type = "CCType";
    C3_CC_setInfo(cc, &info);

    UA_NodeId typeId = UA_NODEID_NUMERIC(NS_DSTYPES, type+1);
    UA_NodeId id = UA_NODEID_NUMERIC(NS_APPLICATION, 1);
    createControlComponentType(server, cc, typeId);
    createControlComponent(server, cc, typeId, id);
}

/* IO Shared Memory Mapping*/
static UA_StatusCode
readIO(UA_Server *server, const UA_NodeId *sessionId, void *sessionContext, const UA_NodeId *nodeId, void *nodeContext,
          UA_Boolean sourceTimeStamp, const UA_NumericRange *range, UA_DataValue *dataValue) {
    CCS_IO_SHMVARIABLE* shmVar = (CCS_IO_SHMVARIABLE*) nodeContext;
    CCS_IO_SHMANY shmValue = {0};
    if(!ccs_io_readValue(shmVar->address, &shmValue)) {
        dataValue->hasValue = false;
        return UA_STATUSCODE_BADNODATA;
    }

    UA_StatusCode result = UA_STATUSCODE_GOOD;
    switch (shmVar->datatype) {
        case CCS_IO_SHMDATATYPE_BOOL:
            result = UA_Variant_setScalarCopy(&dataValue->value, (void*)&shmValue.boolean, &UA_TYPES[UA_TYPES_BOOLEAN]);
            break;
        default:
            return UA_STATUSCODE_BADTYPEMISMATCH;
    }
    dataValue->hasValue = true;
    return result;
}

static UA_StatusCode
writeIO(UA_Server *server, const UA_NodeId *sessionId, void *sessionContext, const UA_NodeId *nodeId, void *nodeContext,
           const UA_NumericRange *range, const UA_DataValue *data) {
    CCS_IO_SHMVARIABLE* shmVar = (CCS_IO_SHMVARIABLE*) nodeContext;
    CCS_IO_SHMANY shmValue = {0};
    switch (shmVar->datatype) {
        case CCS_IO_SHMDATATYPE_BOOL:
            shmValue.boolean = *((bool*)(data->value.data));
            break;
        default:
            return UA_STATUSCODE_BADTYPEMISMATCH;
    }

    if(ccs_io_writeValue(shmVar->address, shmValue)) {
        return UA_STATUSCODE_GOOD;
    }
    return UA_STATUSCODE_BADWRITENOTSUPPORTED;
}

static void
addVariable(UA_Server *server, UA_UInt32 id, CCS_IO_SHMVARIABLE* variable) {
    UA_VariableAttributes attr = UA_VariableAttributes_default;
    attr.displayName = UA_LOCALIZEDTEXT("en-US", variable->name);
    attr.accessLevel = UA_ACCESSLEVELMASK_READ | UA_ACCESSLEVELMASK_WRITE;

    UA_DataSource simIODataSource;
    simIODataSource.read = readIO;
    simIODataSource.write = writeIO;
    UA_Server_addDataSourceVariableNode(server, UA_NODEID_NUMERIC(NS_APPLICATION, id), UA_NODEID_NUMERIC(NS_APPLICATION, 200000),
                                        UA_NODEID_NUMERIC(0, UA_NS0ID_HASPROPERTY), UA_QUALIFIEDNAME(NS_APPLICATION, variable->name),
                                        UA_NODEID_NUMERIC(0, UA_NS0ID_BASEDATAVARIABLETYPE), attr,
                                        simIODataSource, variable, NULL);
}

//TODO: get this from IO_Configuration.xml
static unsigned int CCS_IO_VARS_SIZE = 2;
static CCS_IO_SHMVARIABLE CCS_IO_VARS[] = {
    {0, "Test0", CCS_IO_SHMDATATYPE_BOOL},
    {1, "Test1", CCS_IO_SHMDATATYPE_BOOL}
};

static void addVariablesToUA(UA_Server *server) {
    UA_UInt32 id = 200000;
    addFolder(server, "SHM", "Shared Memory IOs", "Folder for the IOs used by the plant emulation (Unity)",
              UA_NODEID_NUMERIC(NS_APPLICATION, id), UA_NODEID_NUMERIC(0, UA_NS0ID_OBJECTSFOLDER), false);
    for (size_t i = 0; i < CCS_IO_VARS_SIZE; i++) {
        addVariable(server, ++id, &CCS_IO_VARS[i]);
    }
}

int
main(int argc, char **argv) {
    signal(SIGINT, stopHandler);
    signal(SIGTERM, stopHandler);

    /* Parse command line arguments */
    UA_UInt16 port = 4840;
    if(parseCLI(argc, argv, &port) != EXIT_SUCCESS)
        return EXIT_FAILURE;



    /* Create an empty server with default settings */
    UA_Server *server = UA_Server_new();
    /* Customize server name and ns1 name */
    // TODO make loglevel a cli parameter
    setServerConfig(server, "BaSys Control Component Server", "ccs_server", port, UA_LOGLEVEL_INFO);

    // Create list of SHM variables
    ccs_io_openSHM("Local\\Hilt3DShm", CCS_IO_VARS_SIZE); //TODO: get this from IO_Configuration.xml or cli
    addVariablesToUA(server);

    // Create types and parent folder for dummy CCs
    createControlComponentEnvironment(server, NS_PROFILES_URI, "BaSysDemonstratorVendor");

    createControlComponentFromType(server, 0);

    /* Run the control components */
    void *cc_executionLoopContext = cc_executionLoopContext_new(server);
    UA_UInt64 cc_executionLoopId = 1;
    UA_Server_addRepeatedCallback(server, cc_executionLoop, cc_executionLoopContext, 10, &cc_executionLoopId);

    /* Run the server */
    UA_StatusCode retval = UA_Server_run(server, &running);

    /* Free ressources */
    UA_Server_removeCallback(server, cc_executionLoopId);
    cc_executionLoopContext_delete(cc_executionLoopContext);
    clearControlComponentEnvironment(server);
    UA_Server_delete(server);
    ccs_io_closeSHM();

    return retval == UA_STATUSCODE_GOOD ? EXIT_SUCCESS : EXIT_FAILURE;
}