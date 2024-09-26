using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnUnit : SimBehaviour
{
    public bool On;
    public bool InPosition;

    public float Speed;
    public Vector3 Axis;
    public GameObject Frame;

    private float angle;
    private bool active;
    private bool startup;

    private Rigidbody body;
    private Quaternion startRotation;
    private Detection detection;

    private void Start()
    {
        body = Frame.GetComponent<Rigidbody>();
        detection = gameObject.GetComponentInChildren<Detection>();
        startRotation = Frame.transform.rotation;
    }

    private void Update()
    {
        //Animate("TS01_TurnTable");

        InPosition = angle % (360.0 / 7.0) < 0.5f;

        active = On & (!InPosition | startup);

        if (!On)
        {
            startup = true;
        }
        else if (!InPosition)
        {
            startup = false;
        }

        if (active)
        {
            angle += Speed * Time.deltaTime;
            if (angle > 360)
            {
                angle = 360 - angle;
            }
            body.MoveRotation(startRotation * Quaternion.Euler(angle * Axis));
            //transform.localRotation = Quaternion.Euler(angle * Axis);
        }

        detection.isKinematic = active;
    }
}
