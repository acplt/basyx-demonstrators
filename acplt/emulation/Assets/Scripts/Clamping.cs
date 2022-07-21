using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clamping : MonoBehaviour
{
    public bool Ray;
    public Vector3 Offset;
    public Vector3 Direction;
    public float Distance;
    public bool Detected;
    public Rigidbody DetectedObject;

    private RaycastHit hit;
    private int layerMask;

    private void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Can");
    }

    private void Update()
    {
        if (Ray)
        {
            if (Physics.Raycast(transform.position + Offset, Direction, out hit, Distance, layerMask))
            {
                Detected = true;
                DetectedObject = hit.rigidbody;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!Ray)
        {
            Detected = true;
            DetectedObject = collision.rigidbody;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!Ray)
        {
            Detected = false;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!Ray)
        {
            Detected = true;
            DetectedObject = collision.attachedRigidbody;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (!Ray)
        {
            Detected = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position + Offset, Direction * Distance, Color.yellow);
    }
}
