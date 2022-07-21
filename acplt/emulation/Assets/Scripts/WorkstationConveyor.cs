using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkstationConveyor : SimBehaviour
{
    public bool Forward;
    public bool Backward;

    public BeltConveyor Conveyor;
    public LightBarrier LS01;
    public LightBarrier LS02;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Conveyor.Forward & LS01.Detected)
        {
            Forward = true;
        }
        if (Conveyor.Forward & LS02.Detected)
        {
            Forward = false;
        }
        if (Conveyor.Backward & LS02.Detected)
        {
            Backward = true;
        }
        if (Conveyor.Backward & LS01.Detected)
        {
            Backward = false;
        }

    }
}
