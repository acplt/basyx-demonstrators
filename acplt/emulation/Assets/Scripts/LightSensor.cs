using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSensor : MonoBehaviour
{
    public bool Detected;
    public Rigidbody DetectedObject;

    public Vector3 Direction;
    public Vector3 Offset;
    public float Distance;

    private int layerMask;
    private RaycastHit hit;

    private void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Can");
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position + Offset, Direction, out hit, Distance, layerMask))
        {
            Detected = true;
            DetectedObject = hit.rigidbody;
        }
        else
        {
            Detected = false;
            DetectedObject = null;
        }

        //Debug.DrawRay(transform.position + Offset, Direction * Distance, Color.yellow);
    }
}
