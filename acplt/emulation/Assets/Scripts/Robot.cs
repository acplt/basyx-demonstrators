using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : SimBehaviour
{
    public bool PickUp;
    public bool DropOff;
    public bool Home;
    public bool Open;
    public bool Close;
    public bool Teach;

    public bool InPosition;
    public bool IsOpen;
    public bool IsClosed;

    public float Axis1Value;
    public float Axis2Value;
    public float Axis3Value;
    public float Axis4Value;

    public GameObject Axis1;
    public GameObject Axis2;
    public GameObject Axis3;
    public GameObject Axis4;

    public GameObject Gripper;
    public GameObject Gripper1;
    public GameObject Gripper2;
    public float GripperSpeed;
    public float GripperMin;
    public float GripperMax;
    public Vector3 GripperAxis;

    public WorkpieceGenerator Generator;
    public PalletFeeder PalletFeeder;
    public int Mode;

    public List<RobotPath> Pathes;

    private int Position;
    private float value;
    private float time;
    private float angle;
    private Quaternion gripper1Rotation;
    private Quaternion gripper2Rotation;
    private Clamping clamping;
    private GameObject workpieces;
    private WorkPiece workPiece;

    private bool created;
    private bool deleted;

    private void Start()
    {
        workpieces = GameObject.Find("/Dynamics/WorkPieces");
        gripper1Rotation = Gripper1.transform.localRotation;
        gripper2Rotation = Gripper2.transform.localRotation;
        clamping = gameObject.GetComponentInChildren<Clamping>();
        angle = GripperMax;
    }

    private void Update()
    {
        if (Teach)
        {
            Axis1.transform.localRotation = Quaternion.Euler(0, Axis1Value, 0);
            Axis2.transform.localRotation = Quaternion.Euler(0, 0, Axis2Value);
            Axis3.transform.localRotation = Quaternion.Euler(0, 0, Axis3Value);
            Axis4.transform.localRotation = Quaternion.Euler(0, 0, Axis4Value);
        }
        else
        {
            if (Home)
            {
                Position = 2;
            }
            else if (PickUp)
            {
                Position = 0;
            }
            else if (DropOff)
            {
                Position = 1;
            }
            else
            {
                time = 0.0f;
                Position = -1;
            }
        }

        if (Position >= 0)
        {
            time += Time.deltaTime;
            time = Mathf.Clamp(time, 0, 1.0f);
            Axis1.transform.localRotation = Quaternion.Euler(0, Pathes[Position].GetAxis1(time), 0);
            Axis2.transform.localRotation = Quaternion.Euler(0, 0, Pathes[Position].GetAxis2(time));
            Axis3.transform.localRotation = Quaternion.Euler(0, 0, Pathes[Position].GetAxis3(time));
            Axis4.transform.localRotation = Quaternion.Euler(0, 0, Pathes[Position].GetAxis4(time));
        }

        if (Open)
        {
            angle += GripperSpeed * Time.deltaTime;
        }
        else if (Close)
        {
            angle -= GripperSpeed * Time.deltaTime;
        }
        angle = Mathf.Clamp(angle, GripperMin, GripperMax);
        Gripper1.transform.localRotation = gripper1Rotation * Quaternion.Euler(angle * GripperAxis);
        Gripper2.transform.localRotation = gripper2Rotation * Quaternion.Euler(angle * GripperAxis);

        if (Close && angle <= GripperMin)
        {
            if (clamping.DetectedObject != null)
            {
                clamping.DetectedObject.isKinematic = true;
                clamping.DetectedObject.transform.parent = Gripper.transform;
            }
        }

        if (Open & angle >= GripperMax)
        {
            WorkPiece piece = Gripper.FindComponent<WorkPiece>();
            if (piece != null)
            {
                piece.SetKinematic(false);
                piece.transform.parent = workpieces.transform;
            }
        }

        IsOpen = angle >= GripperMax;
        IsClosed = angle <= GripperMin;

        InPosition = (PickUp | DropOff | Home) & time >= 1.0f;

        if (Mode == 1)
        {
            if (InPosition & PickUp)
            {
                if (Close & !created)
                {
                    Generator.Generate();
                    created = true;
                    if (PalletFeeder.Pallet != null)
                    {
                        PalletFeeder.Pallet.RemoveWorkPiece();
                    }
                }
            }

            if (InPosition & DropOff)
            {
                if (Open & created)
                {
                    created = false;
                    clamping.DetectedObject = null;
                }
            }
        }

        if (Mode == 2)
        {
            if (InPosition & DropOff)
            {
                if (clamping.DetectedObject != null)
                {
                    if (Open & !deleted)
                    {
                        deleted = true;
                        if (PalletFeeder.Pallet != null)
                        {
                            workPiece = clamping.DetectedObject.GetComponent<WorkPiece>();
                            PalletFeeder.Pallet.AddWorkPiece(Generator.LoadType, workPiece.GetMaterial());
                            workPiece.Hide();
                            workPiece.Destroy();
                        }
                        clamping.DetectedObject = null;
                    }
                }
            }
        }

        if (InPosition & PickUp)
        {
            deleted = false;
        }

    }
}
