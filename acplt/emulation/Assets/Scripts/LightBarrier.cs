using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBarrier : SimBehaviour
{
    public bool Detected;
    public Rigidbody DetectedObject;

    public Vector3 Direction;
    public Vector3 Offset;
    public float Distance;

    public GameObject Led;
    public GameObject Light;
    public Material Off;
    public Material On;

    private MeshRenderer lightRenderer;
    private MeshRenderer ledRenderer;
    private int layerMask;
    private RaycastHit hit;
    private MeshRenderer led;

    private void Start()
    {
        lightRenderer = Light.GetComponent<MeshRenderer>();
        ledRenderer = Led.GetComponent<MeshRenderer>();
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


        if (Detected)
        {
            ledRenderer.sharedMaterial = On;
        }
        else
        {
            ledRenderer.sharedMaterial = Off;
        }

        lightRenderer.enabled = !Detected;
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position + Offset, Direction * Distance, Color.yellow);
    }

}
