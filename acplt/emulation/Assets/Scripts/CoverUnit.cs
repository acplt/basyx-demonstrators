using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverUnit : SimBehaviour
{
    public bool Up;
    public bool Down;
    public bool Unlock;

    public bool Extracted;
    public bool Retracted;
    public bool Unlocked;
    public bool Detected;

    public Vector3 Direction;
    public Vector3 Offset;
    public float Distance;

    public GameObject Sensor;
    public GameObject Lifter;
    public Clamping Gripper;
    public LockUnit Lock;
    public Vector3 Axis;
    public GameObject Detector;

    public float Speed;
    public float MinPos;
    public float MaxPos;

    private float value;
    private Vector3 startPosition;
    private int layerMask;
    private RaycastHit hit;
    public WorkPiece workPiece;

    private void Start()
    {
        startPosition = Lifter.transform.localPosition;
        layerMask = 1 << LayerMask.NameToLayer("Can");
    }

    private void Update()
    {
        //Animate("FS01_CoverUnit");

        if (Up)
        {
            value -= Speed * Time.deltaTime;
        }
        else if (Down)
        {
            value += Speed * Time.deltaTime;
        }
        value = Mathf.Clamp(value, MinPos, MaxPos);
        Lifter.transform.localPosition = startPosition + value * Axis;

        if (Gripper.Detected & Down)
        {
            Gripper.DetectedObject.isKinematic = true;
            Gripper.DetectedObject.transform.parent = Gripper.transform;
        }

        if (Up)
        {
            if (Gripper.DetectedObject != null)
            {
                if (workPiece != null)
                {
                    Gripper.DetectedObject.gameObject.GetComponent<WorkPiece>().Destroy();
                    workPiece.Show(2);
                }
                //Gripper.DetectedObject.isKinematic = false;
                Gripper.DetectedObject.transform.parent = null;
                Gripper.DetectedObject = null;
            }
        }

        Lock.Unlock = Unlock;

        Extracted = value >= MaxPos;
        Retracted = value <= MinPos;
        Unlocked = Lock.Unlocked;

        if (Physics.Raycast(Sensor.transform.position + Offset, Direction, out hit, Distance, layerMask))
        {
            Detected = true;
        }
        else
        {
            Detected = false;
        }

        if (Physics.Raycast(Detector.transform.position - 0.1f * Vector3.right, Vector3.right, out hit, 0.2f, layerMask))
        {
            workPiece = hit.rigidbody.gameObject.GetComponent<WorkPiece>();
        }
        else
        {
            workPiece = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Debug.DrawRay(Detector.transform.position - 0.1f * Vector3.right, Vector3.right * 0.2f, Color.yellow);
    }
}
