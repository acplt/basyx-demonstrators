using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElecPickAndPlace : SimBehaviour
{
    public int XAxis_Position;
    public int ZAxis_Position;
    public bool Gripper_Close;

    public bool Gripper_Opened;
    public bool Gripper_Closed;

    public PneumaticCylinder XAxis;
    public PneumaticCylinder ZAxis;
    public Gripper Gripper;

    public float GripperSpeed;

    private float gripperPos;

    private void Start()
    {
        
    }

    private void Update()
    {
        XAxis.Stroke = 0.001f * (float)XAxis_Position;
        ZAxis.Stroke = 0.001f * (float)ZAxis_Position;
        
        if (Gripper_Close)
        {
            gripperPos += GripperSpeed * Time.deltaTime;
        }
        else
        {
            gripperPos -= GripperSpeed * Time.deltaTime;
        }
        gripperPos = Mathf.Clamp(gripperPos, Gripper.Min, Gripper.Max);
        Gripper.Stroke = gripperPos;

        Gripper_Opened = Gripper.Open.Detected;
        Gripper_Closed = Gripper.Closed.Detected;
    }
}
