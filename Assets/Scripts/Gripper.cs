using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gripper : MonoBehaviour
{
    public bool IsDown;
    public float Stroke;
    public float Min;
    public float Max;
    public float Limit;

    public PneumaticCylinder CylinderLeft;
    public PneumaticCylinder CylinderRight;
    public Clamping ClampLeft;
    public Clamping ClampRight;
    public GameObject Joint;
    public LightBarrier Sensor;
    public ReedSensor Open;
    public ReedSensor Closed;

    private float limit;

    private void Start ()
    {
	}
	
	private void Update ()
    {
        limit = Min;
        if (Sensor.Detected & IsDown)
        {
            limit = Limit;
        }

        Stroke = Mathf.Clamp(Stroke, limit, Max);
        CylinderLeft.Stroke = Stroke;
        CylinderRight.Stroke = Stroke;

        if (IsDown)
        {
            if (Stroke <= limit)
            {
                if (Sensor.DetectedObject != null)
                {
                    Sensor.DetectedObject.isKinematic = true;
                    Sensor.DetectedObject.transform.parent = Joint.transform;
                    Sensor.DetectedObject.transform.position = Joint.transform.position - 0.08f * Vector3.up;
                }
            }
            else if (Stroke >= Max)
            {
                if (Joint.transform.childCount == 1)
                {
                    Joint.transform.GetChild(0).gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    Joint.transform.GetChild(0).parent = null;
                }
                ClampLeft.DetectedObject = null;
                ClampLeft.Detected = false;
                ClampRight.DetectedObject = null;
                ClampRight.Detected = false;
            }
        }
        Open.Detected = Stroke >= Max;
        Closed.Detected = Stroke <= limit;

    }
}
