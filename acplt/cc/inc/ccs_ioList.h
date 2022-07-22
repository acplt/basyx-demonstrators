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

/* Robot1 IOs
  <ioelement direction="FromSHM" name="RB01_Robot.Close" adsSymbolName="RB01.Close" s7Variable="" signal="66" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="RB01_Robot.DropOff" adsSymbolName="RB01.DropOff" s7Variable="" signal="67" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="RB01_Robot.Home" adsSymbolName="RB01.Home" s7Variable="" signal="68" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="RB01_Robot.Open" adsSymbolName="RB01.Open" s7Variable="" signal="69" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="RB01_Robot.PickUp" adsSymbolName="RB01.PickUp" s7Variable="" signal="70" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="RB01_Robot.InPosition" adsSymbolName="RB01.InPosition" s7Variable="" signal="136" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="RB01_Robot.IsClosed" adsSymbolName="RB01.IsClosed" s7Variable="" signal="137" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="RB01_Robot.IsOpen" adsSymbolName="RB01.IsOpen" s7Variable="" signal="138" size="4" type="bool" unit="none" />
*/
/* Robot2 IOs
  <ioelement direction="FromSHM" name="RB02_Robot.Close" adsSymbolName="RB02.Close" s7Variable="" signal="71" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="RB02_Robot.DropOff" adsSymbolName="RB02.DropOff" s7Variable="" signal="72" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="RB02_Robot.Home" adsSymbolName="RB02.Home" s7Variable="" signal="73" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="RB02_Robot.Open" adsSymbolName="RB02.Open" s7Variable="" signal="74" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="RB02_Robot.PickUp" adsSymbolName="RB02.PickUp" s7Variable="" signal="75" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="RB02_Robot.InPosition" adsSymbolName="RB02.InPosition" s7Variable="" signal="139" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="RB02_Robot.IsClosed" adsSymbolName="RB02.IsClosed" s7Variable="" signal="140" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="RB02_Robot.IsOpen" adsSymbolName="RB02.IsOpen" s7Variable="" signal="141" size="4" type="bool" unit="none" />
/* Conveyor IOs
  <ioelement direction="FromSHM" name="GF01_BeltConveyor.Backward" adsSymbolName="GF01.Backward" s7Variable="AD0.1" signal="18" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="GF01_BeltConveyor.Forward" adsSymbolName="GF01.Forward" s7Variable="AD0.0" signal="19" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="GF02_BeltConveyor.Backward" adsSymbolName="GF02.Backward" s7Variable="AD0.3" signal="20" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="GF02_BeltConveyor.Forward" adsSymbolName="GF02.Forward" s7Variable="AD0.2" signal="21" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="GF03_BeltConveyor.Backward" adsSymbolName="GF03.Backward" s7Variable="AD0.5" signal="22" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="GF03_BeltConveyor.Forward" adsSymbolName="GF03.Forward" s7Variable="AD0.4" signal="23" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="GF04_BeltConveyor.Backward" adsSymbolName="GF04.Backward" s7Variable="AD0.7" signal="24" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="GF04_BeltConveyor.Forward" adsSymbolName="GF04.Forward" s7Variable="AD0.6" signal="25" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="GF05_BeltConveyor.Backward" adsSymbolName="GF05.Backward" s7Variable="AD0.9" signal="26" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="GF05_BeltConveyor.Forward" adsSymbolName="GF05.Forward" s7Variable="AD0.8" signal="27" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="GF06_BeltConveyor.Backward" adsSymbolName="GF06.Backward" s7Variable="AD0.11" signal="28" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="GF06_BeltConveyor.Forward" adsSymbolName="GF06.Forward" s7Variable="" codesysVariable="QW0.7" signal="29" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="GF07_BeltConveyor.Backward" adsSymbolName="GF07.Backward" s7Variable="AD0.13" signal="30" size="4" type="bool" unit="none" />
  <ioelement direction="FromSHM" name="GF07_BeltConveyor.Forward" adsSymbolName="GF07.Forward" s7Variable="AD0.12" signal="31" size="4" type="bool" unit="none" />
  LightBarrier
   <ioelement direction="ToSHM" name="GF01LS01_LightBarrier.Detected" adsSymbolName="" s7Variable="ED0.0" signal="102" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="GF01LS02_LightBarrier.Detected" adsSymbolName="" s7Variable="ED0.1" signal="103" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="GF02LS01_LightBarrier.Detected" adsSymbolName="" s7Variable="ED0.2" signal="104" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="GF02LS02_LightBarrier.Detected" adsSymbolName="" s7Variable="ED0.3" signal="105" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="GF03LS01_LightBarrier.Detected" adsSymbolName="" s7Variable="ED0.4" signal="106" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="GF03LS02_LightBarrier.Detected" adsSymbolName="" s7Variable="ED0.5" signal="107" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="GF04LS01_LightBarrier.Detected" adsSymbolName="" s7Variable="ED0.6" signal="108" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="GF04LS02_LightBarrier.Detected" adsSymbolName="" s7Variable="ED0.7" signal="109" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="GF05_Workstation.Backward" adsSymbolName="" s7Variable="" signal="110" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="GF05_Workstation.Forward" adsSymbolName="" s7Variable="" signal="111" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="GF05LS01_LightBarrier.Detected" adsSymbolName="" s7Variable="ED0.8" signal="112" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="GF05LS02_LightBarrier.Detected" adsSymbolName="" s7Variable="ED0.9" signal="113" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="GF06LS01_LightBarrier.Detected" adsSymbolName="" s7Variable="ED0.10" signal="114" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="GF06LS02_LightBarrier.Detected" adsSymbolName="" s7Variable="ED0.11" signal="115" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="GF07LS01_LightBarrier.Detected" adsSymbolName="" s7Variable="ED0.12" signal="116" size="4" type="bool" unit="none" />
  <ioelement direction="ToSHM" name="GF07LS02_LightBarrier.Detected" adsSymbolName="" s7Variable="ED0.13" signal="117" size="4" type="bool" unit="none" />
  */

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
    {117,"GF07LS02_LightBarrier.Detected", CCS_IO_SHMDATATYPE_BOOL},
    
    
};

#endif /* CCS_IOLIST_H */