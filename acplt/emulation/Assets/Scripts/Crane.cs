using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crane : SimBehaviour
{
    public float XAxis_Position;
    public float YAxis_Position;
    public float Hook_XAxis_Position;
    public float Hook_YAxis_Position;
    public float Hook_ZAxis_Position;

    public Bridge bridge;
    public Trolley trolley;
    public Hook hook;

    private void Start()
    {
        
    }

    private void Update()
    {
        trolley.Position = XAxis_Position;
        bridge.Position = YAxis_Position;
        hook.PositionX = Hook_XAxis_Position;
        hook.PositionY = Hook_YAxis_Position;
        hook.PositionZ = Hook_ZAxis_Position;
        trolley.Length = hook.PositionZ;
    }
}
