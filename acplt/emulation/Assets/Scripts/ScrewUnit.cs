using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrewUnit : SimBehaviour
{
    public bool XAxis_Extract;
    public bool XAxis_Retract;
    public bool YAxis_Extract;
    public bool YAxis_Retract;
    public bool Gripper_Open;
    public bool Gripper_Close;

    public bool XAxis_Extracted;
    public bool XAxis_Retracted;
    public bool YAxis_Extracted;
    public bool YAxis_Retracted;
    public bool Gripper_IsOpen;
    public bool Gripper_IsClosed;

    private float XPosition;
    private float YPosition;
    private float GripperPos;

    public GameObject Frame;
    public GameObject Lifter;
    public GameObject Gripper;
    public GameObject Gripper1;
    public GameObject Gripper2;

    public float XAxisMin;
    public float XAxisMax;
    public float YAxisMin;
    public float YAxisMax;
    public float GripperMin;
    public float GripperMax;

    public float XAxisSpeed;
    public float YAxisSpeed;
    public float GripperSpeed;

    private Vector3 xAxis;
    private Vector3 yAxis;
    private Vector3 gripperAxis;

    private Vector3 XAxisPosition;
    private Vector3 YAxisPosition;
    private Vector3 GripperPosition1;
    private Vector3 GripperPosition2;
    private Clamping clamping;

    private void Start()
    {
        XAxisPosition = Frame.transform.position;
        YAxisPosition = Lifter.transform.localPosition;
        xAxis = -Vector3.right;
        yAxis = Vector3.down;
        gripperAxis = Vector3.right;
        GripperPosition1 = Gripper1.transform.localPosition;
        GripperPosition2 = Gripper2.transform.localPosition;
        clamping = Gripper.GetComponent<Clamping>();
    }

    private void Update()
    {
        //Animate("FS02_ScrewUnit");

        if (XAxis_Extract)
        {
            XPosition += XAxisSpeed * Time.deltaTime;
        }
        else if (XAxis_Retract)
        {
            XPosition -= XAxisSpeed * Time.deltaTime;
        }
        XPosition = Mathf.Clamp(XPosition, XAxisMin, XAxisMax);
        Frame.transform.position = XAxisPosition + XPosition * xAxis;

        if (YAxis_Extract)
        {
            YPosition += YAxisSpeed * Time.deltaTime;
        }
        else if (YAxis_Retract)
        {
            YPosition -= YAxisSpeed * Time.deltaTime;
        }
        YPosition = Mathf.Clamp(YPosition, YAxisMin, YAxisMax);
        Lifter.transform.localPosition = YAxisPosition + YPosition * yAxis;

        if (clamping.Detected && GripperPos <= GripperMin & Gripper_Close)
        {
            if (clamping.DetectedObject != null)
            {
                clamping.DetectedObject.isKinematic = true;
                clamping.DetectedObject.transform.parent = Gripper.transform;
            }
        }
        else if (GripperPos >= GripperMax & Gripper_Open)
        {
            if (clamping.DetectedObject != null)
            {
                clamping.DetectedObject.isKinematic = true;
                clamping.DetectedObject.GetComponent<Screw>().Insert();
                clamping.DetectedObject = null;
            }
        }

        if (Gripper_Open)
        {
            GripperPos += GripperSpeed * Time.deltaTime;
        }
        if (Gripper_Close)
        {
            GripperPos -= GripperSpeed * Time.deltaTime;
        }
        GripperPos = Mathf.Clamp(GripperPos, GripperMin, GripperMax);
        Gripper1.transform.localPosition = GripperPosition1 - GripperPos * gripperAxis;
        Gripper2.transform.localPosition = GripperPosition2 + GripperPos * gripperAxis;

        Gripper_IsOpen = GripperPos >= GripperMax;
        Gripper_IsClosed = GripperPos <= GripperMin;

        XAxis_Extracted = XPosition >= XAxisMax;
        XAxis_Retracted = XPosition <= YAxisMin;

        YAxis_Extracted = YPosition >= YAxisMax;
        YAxis_Retracted = YPosition <= YAxisMin;
    }
}
