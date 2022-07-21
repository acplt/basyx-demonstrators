using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrewDriver : SimBehaviour
{
    public bool Up;
    public bool Down;
    public bool On;
    public bool Reached;
    public bool InUpperPos;
    public bool InLowerPos;

    public GameObject Lifter;
    public GameObject Driver;

    public Vector3 LiftAxis;
    public Vector3 RotAxis;

    public float LiftMin;
    public float LiftMax;
    public float LiftSpeed;
    public float RotSpeed;
    public float RotAngle;

    private float lift;
    private float rot;
    private Vector3 liftPosition;
    private Quaternion driverRotation;
    private Screw screw;
    private Clamping clamping;

    private void Start()
    {
        liftPosition = Lifter.transform.localPosition;
        driverRotation = Driver.transform.localRotation;
        clamping = gameObject.GetComponentInChildren<Clamping>();
    }

    private void Update()
    {
        if (Up)
        {
            lift -= LiftSpeed * Time.deltaTime;
        }
        else if (Down)
        {
            lift += LiftSpeed * Time.deltaTime;
        }
        lift = Mathf.Clamp(lift, LiftMin, LiftMax);
        Lifter.transform.localPosition = liftPosition + lift * LiftAxis;

        if (On & !Reached)
        {
            rot += RotSpeed * Time.deltaTime;
            Driver.transform.localRotation = driverRotation * Quaternion.Euler(rot * RotAxis);
        }

        if (!On)
        {
            rot = 0.0f;
        }

        Reached = rot >= RotAngle;

        if (Reached && clamping.DetectedObject != null)
        {
            screw = clamping.DetectedObject.GetComponent<Screw>();
            screw.Fix();
            clamping.DetectedObject = null;
        }

        InUpperPos = lift == LiftMin;
        InLowerPos = lift == LiftMax;
    }

}
