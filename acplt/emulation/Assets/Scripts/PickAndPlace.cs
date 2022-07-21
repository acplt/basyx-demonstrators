using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAndPlace : SimBehaviour
{
    public float XAxis_Position;
    public float ZAxis_Position;
    public float Gripper_Position;

    public bool XAxis_Extract;
    public bool XAxis_Retract;
    public bool ZAxis_Extract;
    public bool ZAxis_Retract;
    public bool Gripper_Open;
    public bool Gripper_Close;

    public bool XAxis_Extracted;
    public bool XAxis_Retracted;
    public bool ZAxis_Extracted;
    public bool ZAxis_Retracted;
    public bool Gripper_IsOpen;
    public bool Gripper_IsClosed;

    public PneumaticCylinder XAxis;
    public PneumaticCylinder ZAxis;
    public Gripper Gripper;

    public ReedSensor XAxis_Sensor1;
    public ReedSensor XAxis_Sensor2;
    public ReedSensor ZAxis_Sensor1;
    public ReedSensor ZAxis_Sensor2;
    public ReedSensor Gripper_Sensor1;
    public ReedSensor Gripper_Sensor2;

    public float XAxisSpeed;
    public float ZAxisSpeed;
    public float GripperSpeed;

    private const float mm = 1000.0f;

    private void Start()
    {
        
    }

    private void Update()
    {
        Move_XAxis();
        Move_ZAxis();
        Move_Gripper();
    }

    private void Move_XAxis()
    {
        if (XAxis_Extract & !XAxis_Retract)
        {
            XAxis.Stroke += XAxisSpeed * Time.deltaTime;
        }
        if (XAxis_Retract & !XAxis_Extract)
        {
            XAxis.Stroke -= XAxisSpeed * Time.deltaTime;
        }
        XAxis.Stroke = Mathf.Clamp(XAxis.Stroke, XAxis.Min, XAxis.Max);

        XAxis_Extracted = XAxis_Sensor2.Detected;
        XAxis_Retracted = XAxis_Sensor1.Detected;
    }

    private void Move_ZAxis()
    {
        if (ZAxis_Extract & !ZAxis_Retract)
        {
            ZAxis.Stroke += ZAxisSpeed * Time.deltaTime;
        }
        if (ZAxis_Retract & !ZAxis_Extract)
        {
            ZAxis.Stroke -= ZAxisSpeed * Time.deltaTime;
        }
        ZAxis.Stroke = Mathf.Clamp(ZAxis.Stroke, ZAxis.Min, ZAxis.Max);

        ZAxis_Extracted = ZAxis_Sensor2.Detected;
        ZAxis_Retracted = ZAxis_Sensor1.Detected;
    }

    private void Move_Gripper()
    {
        if (Gripper_Close)
        {
            Gripper.Stroke -= GripperSpeed * Time.deltaTime;
        }
        if (Gripper_Open)
        {
            Gripper.Stroke += GripperSpeed * Time.deltaTime;
        }
        Gripper.Stroke = Mathf.Clamp(Gripper.Stroke, Gripper.Min, Gripper.Max);
        Gripper.IsDown = ZAxis_Extracted;

        Gripper_IsOpen = Gripper_Sensor1.Detected;
        Gripper_IsClosed = Gripper_Sensor2.Detected;
    }


}
