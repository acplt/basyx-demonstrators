cmake_minimum_required(VERSION 3.10)

project(basys-demonstrator-control-component-server)

# Set output to own directory to separate build artifacts
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)

# CCProfilesUA library overwrite options
set(CCPUA_BUILD_PROFILESERVER OFF CACHE BOOL "Don't build profile server" FORCE)
set(CCPUA_BUILD_TESTS OFF CACHE BOOL "Don't build tests for now" FORCE)
set(CCPUA_BUILD_PACKML OFF CACHE BOOL "Don't use packML interface for smaller examples." FORCE)
add_subdirectory(ccProfilesUA)

# Source files excluding main
set(CCSERVER_SOURCE_FILES 
    "src/ccs_io.c"
    "src/ccs_ioList.c"
    "src/ccs_type_generic.c"
    "src/ccs_type_robot.c"
    "src/ccs_type_conveyor.c"
)

# Control Component Server executable
add_executable(ccs_server ${CCSERVER_SOURCE_FILES} "src/ccs_server.c")
target_include_directories(ccs_server PUBLIC "inc")
target_link_libraries(ccs_server ccProfilesUA)

# Shared Memory Tool executable
if(WIN32)
    add_executable(ccs_shmtool "src/ccs_io.c" "src/ccs_shmtool.c")
    target_include_directories(ccs_shmtool PUBLIC "inc")
endif()