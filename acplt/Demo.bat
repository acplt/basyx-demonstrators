@echo off

REM Add function for option selection and add options to quit (q) or stop process (s)

ECHO 1. Emulation
SET selectOption=y
SET /p selectOption="     - Start Unity 3D application simulating the real plant? [Y/n]: "
IF %selectOption% == y (
    START "Emulation" /AFFINITY 2 emulation\BaSys_Demonstrator.exe QUALITY-FASTEST RESOLUTION-1920-1080-WINDOW REGISTRY-127.0.0.1
)

ECHO:
ECHO 2. Control Component Server
SET selectOption=y
SET /p selectOption="     - Start Control Component application with OPC UA server? [Y/n]: "
IF %selectOption% == y (
    START "Control Component Server" /D cc ccs_server.exe --ioPath ..\emulation\IO_Configuration.xml
)

ECHO:
ECHO 3. AAS Repository Server
SET selectOption=n
SET /p selectOption="     - Rebuild CC Submodels from: aas\aas_generator\config.ini? [y/N]: "
IF %selectOption% == y (
    START /D aas Generate_CC_Submodels.exe
)
SET selectOption=y
SET /p selectOption="     - Start BaSyx python SDK based Asset Administration Shell repository server? [Y/n]: "
IF %selectOption% == y (
    START "AAS Repository Server" /D aas AAS_Repository_Server.exe
)

ECHO:
ECHO 4. Workflow Executor
SET selectOption=y
SET /p selectOption="     - Start Workflow Executor Python Application? [Y/n]: "
IF %selectOption% == y (
    ECHO ... working on that
)

ECHO:
ECHO 5. BPMN Engine
SET selectOption=n
SET /p selectOption="     - Delete old Camunda database from: bpmn\internal\camunda-h2-default? [y/N]: "
IF %selectOption% == y (
    DEL /Q /F "bpmn\internal\camunda-h2-default"
)
SET selectOption=y
SET /p selectOption="     - Start camunda with webapps, rest api and ressources from: bpmn\configuration\resources? [Y/n]: "
IF %selectOption% == y (
    START "BPMN Engine Camunda" /D bpmn\internal run.bat start --webapps --rest
)

ECHO:
ECHO Done. Demo is started. See README for usage.
ECHO:
pause