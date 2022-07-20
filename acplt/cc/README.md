# Control Component Server for 

This project implements an OPC UA server instanciating mulitple control components specified via a proprietary configuration file in a C project.

The project uses:

* the [C3 library](https://git.rwth-aachen.de/acplt/basys4.2/c3/) as control component C implementation library
* the [CCProfilesUA](https://git.rwth-aachen.de/acplt/basys4.2/ccProfilesUA/) profile server to generate OPC UA interfaces
* the [open62541 SDK](https://www.open62541.org) for the OPC UA Server

These are integrated as submodules via the CCProfilesUA project.
