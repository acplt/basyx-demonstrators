using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PneumaticCylinder : MonoBehaviour
{
    public float Stroke;
    public Vector3 Axis;
    public float Min;
    public float Max;
    public GameObject Rod;

    private float Value;
    private Vector3 startPosition;

	private void Start ()
    {
        Axis = Axis.normalized;
        startPosition = Rod.transform.localPosition;
    }
	
	private void Update ()
    {
        Value = Stroke;
        Value = Mathf.Clamp(Value, Min, Max);
        Rod.transform.localPosition = startPosition + Value * Axis;	

    }

    private Vector3 Clamp(Vector3 value, Vector3 Min, Vector3 Max)
    {
        value.x = Mathf.Clamp(value.x, Min.x, Max.x);
        value.y = Mathf.Clamp(value.y, Min.y, Max.y);
        value.z = Mathf.Clamp(value.z, Min.z, Max.z);
        return value;
    }
}
