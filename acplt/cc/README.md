# Control Component Server for BaSys Demonstrator (ACPLT Version)

[![acplt](https://github.com/acplt/basyx-demonstrators/actions/workflows/acplt.yml/badge.svg)](https://github.com/acplt/basyx-demonstrators/actions/workflows/acplt.yml)

This folder holds an implementation for an OPC UA server instanciating control components specified via a proprietary configuration file in a C project.
The code is build via github actions for Windows 64 bit only, as the demonstrator is windows based.

The project uses:

* the [C3 library](https://git.rwth-aachen.de/acplt/basys4.2/c3/) as control component C implementation library
* the [CCProfilesUA](https://git.rwth-aachen.de/acplt/basys4.2/ccProfilesUA/) profile server to generate OPC UA interfaces
* the [open62541 SDK](https://www.open62541.org) for the OPC UA Server

These are integrated as submodules via the CCProfilesUA project.

## Shared Memory Tool

Additionally this folder contains source code for a simplistic executable to read and write shared memory values via command line: `ccs_shmtool.exe`