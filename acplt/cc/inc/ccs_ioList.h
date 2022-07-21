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

/* Robot IOs
  <ioelement direction="FromSHM" name="RB01_Robot.Close" adsSymbolName="RB01.Close" s7Variable="" signal="66" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="RB01_Robot.DropOff" adsSymbolName="RB01.DropOff" s7Variable="" signal="67" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="RB01_Robot.Home" adsSymbolName="RB01.Home" s7Variable="" signal="68" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="RB01_Robot.Open" adsSymbolName="RB01.Open" s7Variable="" signal="69" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="RB01_Robot.PickUp" adsSymbolName="RB01.PickUp" s7Variable="" signal="70" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="RB01_Robot.InPosition" adsSymbolName="RB01.InPosition" s7Variable="" signal="136" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="RB01_Robot.IsClosed" adsSymbolName="RB01.IsClosed" s7Variable="" signal="137" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="RB01_Robot.IsOpen" adsSymbolName="RB01.IsOpen" s7Variable="" signal="138" size="4" type="bool" unit="none" />
*/

static unsigned int CCS_IOLIST_SIZE = 8;
static CCS_IO_SHMVARIABLE CCS_IOLIST[] = {
    {66, "RB01_Robot.Close", CCS_IO_SHMDATATYPE_BOOL},
    {67, "RB01_Robot.DropOff", CCS_IO_SHMDATATYPE_BOOL},
    {68, "RB01_Robot.Home", CCS_IO_SHMDATATYPE_BOOL},
    {69, "RB01_Robot.Open", CCS_IO_SHMDATATYPE_BOOL},
    {70, "RB01_Robot.PickUp", CCS_IO_SHMDATATYPE_BOOL},
    {136, "RB01_Robot.InPosition", CCS_IO_SHMDATATYPE_BOOL},
    {137, "RB01_Robot.IsClosed", CCS_IO_SHMDATATYPE_BOOL},
    {138, "RB01_Robot.IsOpen", CCS_IO_SHMDATATYPE_BOOL}
};

#endif /* CCS_IOLIST_H */