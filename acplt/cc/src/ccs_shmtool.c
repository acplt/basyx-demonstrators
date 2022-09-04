#include <signal.h>
#include <stdio.h>
#include <stdlib.h>

#include <ccs_io.h>

static bool interactive = false;
static unsigned int interval = 0;
static char* name = "Local\\Hilt3DShm";
static unsigned int size = 150;
static unsigned int address = 0;
static bool addressSpecified = false;
static bool write = false;
static unsigned int value = 0;

static void
usage(void) {
    printf("Usage: ccs_shmtool");
    printf(" [--interactive]");
    printf(" [--interval <interactiveSleepTimeInMs>]");
    printf(" [--size <SHMSignalCount>]");
    printf(" [--address <signalNumber>]");
    printf(" [--write]");
    printf(" [--value <valueToWrite>]");
    printf(" [--help]\n");
}
static void
usageError(const char *errorMsg) {
    fprintf(stderr, "Error: %s\n", errorMsg);
    usage();
}

static int
parseCLI(int argc, char **argv) {
    for(int argpos = 1; argpos < argc; argpos++) {
        if(strcmp(argv[argpos], "--help") == 0 || strcmp(argv[argpos], "-h") == 0) {
            usage();
            printf(
                "\n\tExample: ccs_shmtool --interactive --write --address 12 --value 42\n");
            return EXIT_FAILURE;
        }
        if(strcmp(argv[argpos], "--interactive") == 0) {
            interactive = true;
            continue;
        }
        if(strcmp(argv[argpos], "--write") == 0) {
            write = true;
            continue;
        }
        if(strcmp(argv[argpos], "--value") == 0) {
            argpos++;
            if(argpos >= argc) {
                usageError("parameter --value given, but no value specified!");
                return EXIT_FAILURE;
            }
            value = strtol(argv[argpos], NULL, 10);
            continue;
        }
        if(strcmp(argv[argpos], "--address") == 0) {
            argpos++;
            if(argpos >= argc) {
                usageError("parameter --address given, but no address specified!");
                return EXIT_FAILURE;
            }
            address = strtol(argv[argpos], NULL, 10);
            addressSpecified = true;
            continue;
        }
        if(strcmp(argv[argpos], "--size") == 0) {
            argpos++;
            if(argpos >= argc) {
                usageError("parameter --size given, but no size specified!");
                return EXIT_FAILURE;
            }
            size = strtol(argv[argpos], NULL, 10);
            continue;
        }
        if(strcmp(argv[argpos], "--interval") == 0) {
            argpos++;
            if(argpos >= argc) {
                usageError("parameter --interval given, but no interval specified!");
                return EXIT_FAILURE;
            }
            interval = strtol(argv[argpos], NULL, 10);
            continue;
        }
        // Unknown option
        usageError("unknown command line option.");
        return EXIT_FAILURE;
    }
    return EXIT_SUCCESS;
}

static void
stopHandler(int sig) {
    interactive = false;
}

int
main(int argc, char **argv) {
    signal(SIGINT, stopHandler);
    signal(SIGTERM, stopHandler);

    /* Parse command line arguments */
    if(parseCLI(argc, argv) != EXIT_SUCCESS)
        return EXIT_FAILURE;

    /* Open SHM*/
    if(!ccs_io_openSHM(name, size)) { //TODO: get this from cli
        fprintf(stderr, "Error while opening the shm %s with size %d. Error message:\n%s\n", name, size, GetLastError());
        return EXIT_FAILURE;
    }

    /* Read or write*/
    bool result = false;
    if(interactive) {
        if(!addressSpecified) {
            printf("Address,value: ");
            write = scanf("%d,%d", &address, &value) > 1;
            fflush(stdin);
        }
        while(interactive) {
            if(write) {
                result = ccs_io_writeValue(address, value);       
            }else{
                result = ccs_io_readValue(address, &value);
            }
            if(!result){
                fprintf(stderr, "Error while accessing the address %d. Error message:\n%s\n", address, GetLastError());
                break;
            }else{
                fprintf(stdout, "%s %d=%d\n", write ? "write" : "read" , address, value);
            }
            if(interval > 0) {
                Sleep(interval);
            }else{
                printf("Address,value: ");
                write = scanf("%d,%d", &address, &value) > 1;
                fflush(stdin);
            }
        }
    }else if(addressSpecified){
        if(write) {
            result = ccs_io_writeValue(address, value);       
        }else{
            result = ccs_io_readValue(address, &value);
        }
        if(!result){
            fprintf(stderr, "Error while accessing the address %d. Error message:\n%s\n", address, GetLastError());
        }else{
            fprintf(stdout, "%s %d=%d\n", write ? "write" : "read" , address, value);
        }
    }else{
        fprintf(stdout, "Not interactive and no address specified. Just open and close the SHM.\n");
    }

    /* Close SHM */
    ccs_io_closeSHM();
    return result ? EXIT_SUCCESS : EXIT_FAILURE;
}