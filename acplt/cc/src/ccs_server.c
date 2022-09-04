#include <cc_connector.h>
#include <cc_instanciation.h>

#include <profile_utilities.h>

#include <signal.h>
#include <stdio.h>
#include <stdlib.h>

#include <ccs_type.h>

UA_Boolean running = true;
static void
stopHandler(int sig) {
    running = false;
}

static void
usage(void) {
    printf("Usage: ccs_server");
    printf(" [--port <port>]");
    printf(" [--ioPath <File path to IO_Configuration.xml>]");
//    printf(" [--loglevel DEBUG|INFO|WARN|ERROR|FATAL]");
    printf(" [--help]\n");
}
static void
usageError(const char *errorMsg) {
    fprintf(stderr, "Error: %s\n", errorMsg);
    usage();
}

static int
parseCLI(int argc, char **argv, UA_UInt16 *port, char** ioPath, UA_LogLevel *loglevel) {
    for(int argpos = 1; argpos < argc; argpos++) {
        if(strcmp(argv[argpos], "--help") == 0 || strcmp(argv[argpos], "-h") == 0) {
            usage();
            printf(
                "\n\tExample: ccs_server --port 16664 --ioPath ../IO_Configuration.xml\n");
            return EXIT_FAILURE;
        }
        // parse server port
        if(strcmp(argv[argpos], "--port") == 0) {
            argpos++;
            if(argpos >= argc) {
                usageError("parameter --port given, but no port specified!");
                return EXIT_FAILURE;
            }
            *port = strtol(argv[argpos], NULL, 10);
            continue;
        }
        // parse io path
        if(strcmp(argv[argpos], "--ioPath") == 0) {
            argpos++;
            if(argpos >= argc) {
                usageError("parameter --ioPath given, but no path specified!");
                return EXIT_FAILURE;
            }
            *ioPath = argv[argpos];
            continue;
        }
        /*
        if(strcmp(argv[argpos], "--loglevel") == 0) {
            argpos++;
            if(argpos >= argc) {
                usageError("parameter --loglevel given, but no level specified!");
                return EXIT_FAILURE;
            }
            if(strcmp(argv[argpos], "DEBUG") == 0)
                *loglevel = UA_LOGLEVEL_DEBUG;
            else if(strcmp(argv[argpos], "INFO") == 0)
                *loglevel = UA_LOGLEVEL_INFO;
            else if(strcmp(argv[argpos], "WARN") == 0)
                *loglevel = UA_LOGLEVEL_WARNING;
            else if(strcmp(argv[argpos], "ERROR") == 0)
                *loglevel = UA_LOGLEVEL_ERROR;
            else if(strcmp(argv[argpos], "FATAL") == 0)
                *loglevel = UA_LOGLEVEL_FATAL;
            else {
                usageError("parameter --loglevel given, but loglevel unknown.\n Possible options are: DEBUG|INFO|WARN|ERROR|FATAL");
                return EXIT_FAILURE;
            }
            continue;
        }*/
        // Unknown option
        usageError("unknown command line option.");
        return EXIT_FAILURE;
    }
    return EXIT_SUCCESS;
}

static int ccInstanceCounter = 0;
static bool ccTypeInstaciated[CCS_TYPE_COUNT] = {0};

static void
createControlComponentFromType(UA_Server *server, char* name, CCS_TYPE type) {
    C3_CC* cc = ccs_type_createInstance(name, type);

    UA_NodeId typeId = UA_NODEID_NUMERIC(NS_DSTYPES, type + 1);
    if(!ccTypeInstaciated[type]) {
        ccTypeInstaciated[type] = true;
        createControlComponentType(server, cc, typeId);
    }

    UA_NodeId id = UA_NODEID_NUMERIC(NS_APPLICATION, ++ccInstanceCounter);
    UA_LOG_INFO(UA_Log_Stdout, UA_LOGCATEGORY_USERLAND, "Create Control Component %s of type %s with id ns=%d;i=%d",
     name, CCS_TYPE_NAMES[type], id.namespaceIndex, id.identifier.numeric);
    createControlComponent(server, cc, typeId, id);
}

/* IO Shared Memory Mapping*/
static UA_StatusCode
readIO(UA_Server *server, const UA_NodeId *sessionId, void *sessionContext, const UA_NodeId *nodeId, void *nodeContext,
          UA_Boolean sourceTimeStamp, const UA_NumericRange *range, UA_DataValue *dataValue) {
    CCS_IO_SHMVARIABLE* shmVar = (CCS_IO_SHMVARIABLE*) nodeContext;
    unsigned int shmValue = 0;
    if(ccs_io_readValue(shmVar->address, &shmValue)) {
        dataValue->hasValue = true;
    }

    UA_StatusCode result = UA_STATUSCODE_GOOD;
    switch (shmVar->datatype) {
        case CCS_IO_SHMDATATYPE_INT:
            return UA_Variant_setScalarCopy(&dataValue->value, (void*)&shmValue, &UA_TYPES[UA_TYPES_INT32]);
        case CCS_IO_SHMDATATYPE_UINT:
            return UA_Variant_setScalarCopy(&dataValue->value, (void*)&shmValue, &UA_TYPES[UA_TYPES_UINT32]);
        case CCS_IO_SHMDATATYPE_REAL:
            return UA_Variant_setScalarCopy(&dataValue->value, (void*)&shmValue, &UA_TYPES[UA_TYPES_FLOAT]);
        default:
        case CCS_IO_SHMDATATYPE_BOOL:
            return UA_Variant_setScalarCopy(&dataValue->value, (void*)&shmValue, &UA_TYPES[UA_TYPES_BOOLEAN]);
        case CCS_IO_SHMDATATYPE_STRING:
            return UA_STATUSCODE_BADNOTIMPLEMENTED;
    }
}

static UA_StatusCode
writeIO(UA_Server *server, const UA_NodeId *sessionId, void *sessionContext, const UA_NodeId *nodeId, void *nodeContext,
           const UA_NumericRange *range, const UA_DataValue *data) {
    CCS_IO_SHMVARIABLE* shmVar = (CCS_IO_SHMVARIABLE*) nodeContext;
    unsigned int shmValue = 0;
    switch (shmVar->datatype) {
        case CCS_IO_SHMDATATYPE_BOOL:
            shmValue = *((bool*)(data->value.data));
            break;
        case CCS_IO_SHMDATATYPE_INT:
            shmValue = *((UA_Int32*)(data->value.data));
            break;
        case CCS_IO_SHMDATATYPE_UINT:
            shmValue = *((UA_UInt32*)(data->value.data));
            break;
        case CCS_IO_SHMDATATYPE_REAL:
            shmValue = *((UA_Float*)(data->value.data));
            break;
        case CCS_IO_SHMDATATYPE_STRING:
            return UA_STATUSCODE_BADNOTIMPLEMENTED;
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
    UA_LOG_DEBUG(UA_Log_Stdout, UA_LOGCATEGORY_USERLAND, "Add SHM variable %s with address %d", variable->name, variable->address);
    UA_Server_addDataSourceVariableNode(server, UA_NODEID_NUMERIC(NS_APPLICATION, id), UA_NODEID_NUMERIC(NS_APPLICATION, 200000),
                                        UA_NODEID_NUMERIC(0, UA_NS0ID_HASPROPERTY), UA_QUALIFIEDNAME(NS_APPLICATION, variable->name),
                                        UA_NODEID_NUMERIC(0, UA_NS0ID_BASEDATAVARIABLETYPE), attr,
                                        simIODataSource, variable, NULL);
}

static void addVariablesToUA(UA_Server *server) {
    UA_UInt32 id = 200000;
    addFolder(server, "SHM", "Shared Memory IOs", "Folder for the IOs used by the plant emulation (Unity)",
              UA_NODEID_NUMERIC(NS_APPLICATION, id), UA_NODEID_NUMERIC(0, UA_NS0ID_OBJECTSFOLDER), false);
    for (size_t i = 0; i < CCS_IOLIST_SIZE; i++) {
        addVariable(server, ++id, &CCS_IOLIST[i]);
    }
}

int
main(int argc, char **argv) {
    signal(SIGINT, stopHandler);
    signal(SIGTERM, stopHandler);

    /* Parse command line arguments */
    UA_UInt16 port = 4840;
    char* ioPath = "IO_Configuration.xml";
    UA_LogLevel loglevel = UA_LOGLEVEL_INFO;
    if(parseCLI(argc, argv, &port, &ioPath, &loglevel) != EXIT_SUCCESS)
        return EXIT_FAILURE;

    /* Create an empty server with default settings */
    UA_Server *server = UA_Server_new();
    /* Customize server name and ns1 name */
    setServerConfig(server, "BaSys Control Component Server", "ccs_server", port, loglevel);

    // Create list of SHM variables
    UA_LOG_INFO(UA_Log_Stdout, UA_LOGCATEGORY_USERLAND, "Read io config from: %s", ioPath);
    ccs_ioList_readIOConfigurationFromXML(ioPath);
    UA_LOG_INFO(UA_Log_Stdout, UA_LOGCATEGORY_USERLAND, "Using SHM %s with size %d", CCS_IOLIST_SHMNAME, CCS_IOLIST_SIZE);
    ccs_io_openSHM(CCS_IOLIST_SHMNAME, CCS_IOLIST_SIZE);
    addVariablesToUA(server);

    // Create types and parent folder for dummy CCs
    createControlComponentEnvironment(server, NS_PROFILES_URI, "BaSysDemonstratorVendor");

    //TODO: get cc types and instances names from configuration file or cli
    createControlComponentFromType(server, "RB01", CCS_TYPE_ROBOT);
    createControlComponentFromType(server, "RB02", CCS_TYPE_ROBOT);
    createControlComponentFromType(server, "GF01", CCS_TYPE_CONVEYOR);
    createControlComponentFromType(server, "GF02", CCS_TYPE_CONVEYOR);
    createControlComponentFromType(server, "GF03", CCS_TYPE_CONVEYOR);
    createControlComponentFromType(server, "GF04", CCS_TYPE_CONVEYOR);
    createControlComponentFromType(server, "GF05", CCS_TYPE_CONVEYOR);
    createControlComponentFromType(server, "GF06", CCS_TYPE_CONVEYOR);
    createControlComponentFromType(server, "GF07", CCS_TYPE_CONVEYOR);
    createControlComponentFromType(server, "FS01", CCS_TYPE_COVERUNIT);

    /* Run the control components */
    //TODO make cycletime a CLI parameter
    UA_LOG_INFO(UA_Log_Stdout, UA_LOGCATEGORY_USERLAND, "Starting Control Component execution loop with %d ms cycletime.", 10);
    void *cc_executionLoopContext = cc_executionLoopContext_new(server);
    UA_UInt64 cc_executionLoopId = 1;
    UA_Server_addRepeatedCallback(server, cc_executionLoop, cc_executionLoopContext, 10, &cc_executionLoopId);

    /* Run the server */
    UA_LOG_INFO(UA_Log_Stdout, UA_LOGCATEGORY_USERLAND, "Starting OPC UA Server.");
    UA_StatusCode retval = UA_Server_run(server, &running);

    /* Free ressources */
    UA_Server_removeCallback(server, cc_executionLoopId);
    cc_executionLoopContext_delete(cc_executionLoopContext);
    clearControlComponentEnvironment(server);
    UA_Server_delete(server);
    ccs_io_closeSHM();

    return retval == UA_STATUSCODE_GOOD ? EXIT_SUCCESS : EXIT_FAILURE;
}