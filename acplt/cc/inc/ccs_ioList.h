#ifndef CCS_IOLIST_H
#define CCS_IOLIST_H

typedef enum {
    CCS_IO_SHMDATATYPE_BOOL,
    CCS_IO_SHMDATATYPE_INT,
    CCS_IO_SHMDATATYPE_UINT,
    CCS_IO_SHMDATATYPE_REAL,
    CCS_IO_SHMDATATYPE_STRING,
    CCS_IO_SHMDATATYPE_UNDEFINED
} CCS_IO_SHMDATATYPE;

typedef struct {
	unsigned int address;
    char* name;
    CCS_IO_SHMDATATYPE datatype;
} CCS_IO_SHMVARIABLE;

static unsigned int CCS_IOLIST_SIZE = 44;
static CCS_IO_SHMVARIABLE CCS_IOLIST[] = {
    {66, "RB01_Robot.Close", CCS_IO_SHMDATATYPE_BOOL},
    {67, "RB01_Robot.DropOff", CCS_IO_SHMDATATYPE_BOOL},
    {68, "RB01_Robot.Home", CCS_IO_SHMDATATYPE_BOOL},
    {69, "RB01_Robot.Open", CCS_IO_SHMDATATYPE_BOOL},
    {70, "RB01_Robot.PickUp", CCS_IO_SHMDATATYPE_BOOL},
    {136, "RB01_Robot.InPosition", CCS_IO_SHMDATATYPE_BOOL},
    {137, "RB01_Robot.IsClosed", CCS_IO_SHMDATATYPE_BOOL},
    {138, "RB01_Robot.IsOpen", CCS_IO_SHMDATATYPE_BOOL},
    {71, "RB02_Robot.Close", CCS_IO_SHMDATATYPE_BOOL},
    {72, "RB02_Robot.DropOff", CCS_IO_SHMDATATYPE_BOOL},
    {73, "RB02_Robot.Home", CCS_IO_SHMDATATYPE_BOOL},
    {74, "RB02_Robot.Open", CCS_IO_SHMDATATYPE_BOOL},
    {75, "RB02_Robot.PickUp", CCS_IO_SHMDATATYPE_BOOL},
    {139,"RB02_Robot.InPosition", CCS_IO_SHMDATATYPE_BOOL},
    {140,"RB02_Robot.IsClosed", CCS_IO_SHMDATATYPE_BOOL},
    {141,"RB02_Robot.IsOpen", CCS_IO_SHMDATATYPE_BOOL},
    {18, "GF01_BeltConveyor.Backward", CCS_IO_SHMDATATYPE_BOOL},
    {19, "GF01_BeltConveyor.Forward", CCS_IO_SHMDATATYPE_BOOL},
    {20, "GF02_BeltConveyor.Backward", CCS_IO_SHMDATATYPE_BOOL},
    {21, "GF02_BeltConveyor.Forward", CCS_IO_SHMDATATYPE_BOOL},
    {22, "GF03_BeltConveyor.Backward", CCS_IO_SHMDATATYPE_BOOL},
    {23, "GF03_BeltConveyor.Forward", CCS_IO_SHMDATATYPE_BOOL},
    {24,"GF04_BeltConveyor.Backward", CCS_IO_SHMDATATYPE_BOOL},
    {25,"GF04_BeltConveyor.Forward", CCS_IO_SHMDATATYPE_BOOL},
    {26,"GF05_BeltConveyor.Backward", CCS_IO_SHMDATATYPE_BOOL},
    {27,"GF05_BeltConveyor.Forward", CCS_IO_SHMDATATYPE_BOOL},
    {28,"GF06_BeltConveyor.Backward", CCS_IO_SHMDATATYPE_BOOL},
    {29,"GF06_BeltConveyor.Forward", CCS_IO_SHMDATATYPE_BOOL},
    {30,"GF07_BeltConveyor.Backward", CCS_IO_SHMDATATYPE_BOOL},
    {31,"GF07_BeltConveyor.Forward", CCS_IO_SHMDATATYPE_BOOL},
    {102,"GF01LS01_LightBarrier.Detected", CCS_IO_SHMDATATYPE_BOOL},
    {103,"GF01LS02_LightBarrier.Detected", CCS_IO_SHMDATATYPE_BOOL},
    {104,"GF02LS01_LightBarrier.Detected", CCS_IO_SHMDATATYPE_BOOL},
    {105,"GF02LS02_LightBarrier.Detected", CCS_IO_SHMDATATYPE_BOOL},
    {106,"GF03LS01_LightBarrier.Detected", CCS_IO_SHMDATATYPE_BOOL},
    {107,"GF03LS02_LightBarrier.Detected", CCS_IO_SHMDATATYPE_BOOL},
    {108,"GF04LS01_LightBarrier.Detected", CCS_IO_SHMDATATYPE_BOOL},
    {109,"GF04LS02_LightBarrier.Detected", CCS_IO_SHMDATATYPE_BOOL},
    {112,"GF05LS01_LightBarrier.Detected", CCS_IO_SHMDATATYPE_BOOL},
    {113,"GF05LS02_LightBarrier.Detected", CCS_IO_SHMDATATYPE_BOOL},
    {114,"GF06LS01_LightBarrier.Detected", CCS_IO_SHMDATATYPE_BOOL},
    {115,"GF06LS02_LightBarrier.Detected", CCS_IO_SHMDATATYPE_BOOL},
    {116,"GF07LS01_LightBarrier.Detected", CCS_IO_SHMDATATYPE_BOOL},
    {117,"GF07LS02_LightBarrier.Detected", CCS_IO_SHMDATATYPE_BOOL}
};

#endif /* CCS_IOLIST_H */