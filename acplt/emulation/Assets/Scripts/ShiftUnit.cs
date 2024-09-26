using UnityEngine;
using System.Collections;

public class ShiftUnit : SimBehaviour
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
    private float GripperAngle;

    public GameObject Frame;
    public GameObject Lifter;
    public GameObject Gripper;
    public GameObject Gripper1;
    public GameObject Gripper2;
    public GameObject Gripper3;

    public GameObject Track;

    public float XAxisMin;
    public float XAxisMax;
    public float YAxisMin;
    public float YAxisMid;
    public float YAxisMax;
    public float GripperMin;
    public float GripperMax;

    public float XAxisSpeed;
    public float YAxisSpeed;
    public float GripperSpeed;

    private Vector3 xAxis;
    private Vector3 yAxis;

    private Vector3 XAxisPosition;
    private Vector3 YAxisPosition;
    private Quaternion GripperRotation;
    private Clamping clamping;
    private GameObject workpieces;
    private float GripperLimit;

    private void Start()
    {
        workpieces = GameObject.Find("/Dynamics/WorkPieces");

        XAxisPosition = Frame.transform.position;
        YAxisPosition = Lifter.transform.localPosition;
        xAxis = Track.transform.forward;
        yAxis = Vector3.down;
        GripperRotation = Gripper1.transform.localRotation;
        clamping = Gripper.GetComponent<Clamping>();
    }

    private void Update()
    {
        //Animate("PP02_PickAndPlace");

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
        if (XPosition <= XAxisMin)
        {
            YPosition = Mathf.Clamp(YPosition, YAxisMin, YAxisMax);
        }
        else
        {
            YPosition = Mathf.Clamp(YPosition, YAxisMin, YAxisMid);
        }
        Lifter.transform.localPosition = YAxisPosition + YPosition * yAxis;

        if (Gripper_Open)
        {
            GripperAngle -= GripperSpeed * Time.deltaTime;
        }
        if (Gripper_Close)
        {
            GripperAngle += GripperSpeed * Time.deltaTime;
        }
        GripperAngle = Mathf.Clamp(GripperAngle, GripperMin, GripperLimit);
        Gripper1.transform.localRotation = GripperRotation * Quaternion.Euler(0, 0, -GripperAngle);
        Gripper2.transform.localRotation = GripperRotation * Quaternion.Euler(0, 0, -GripperAngle);
        Gripper3.transform.localRotation = GripperRotation * Quaternion.Euler(0, 0, -GripperAngle);

        if (GripperAngle <= 5.0f)
        {
            if (clamping.DetectedObject != null)
            {
                clamping.DetectedObject.isKinematic = false;
                clamping.DetectedObject.transform.parent = workpieces.transform;
                clamping.DetectedObject = null;
            }
            GripperLimit = GripperMax;
        }
        else if (clamping.Detected)
        {
            clamping.DetectedObject.isKinematic = true;
            clamping.DetectedObject.transform.parent = Gripper.transform;
            GripperLimit = 10.0f;
        }

        XAxis_Extracted = XPosition >= XAxisMax;
        XAxis_Retracted = XPosition <= YAxisMin;

        if (XPosition <= XAxisMin)
        {
            YAxis_Extracted = YPosition >= YAxisMax;
        }
        else
        {
            YAxis_Extracted = YPosition >= YAxisMid;
        }
        YAxis_Retracted = YPosition <= YAxisMin;

        Gripper_IsOpen = GripperAngle <= GripperMin;
        Gripper_IsClosed = (GripperAngle >= GripperLimit);
    }
}
