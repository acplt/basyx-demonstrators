using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualScrewDriver : SimBehaviour
{
    public bool Screw;
    public bool Placed;

    public GameObject Sensor;
    public GameObject Holder;
    public GameObject ScrewDriver;
    public GameObject Cable;

    private Vector3 Direction;
    private Vector3 Offset;
    private float Distance;

    private float s;
    private float t;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private int layerMask;
    private RaycastHit hit;

    private Vector3 path;
    private Cable cable;

    public WorkPiece workpiece;

    private void Start()
    {
        startPosition = ScrewDriver.transform.position;
        startRotation = ScrewDriver.transform.localRotation;
        layerMask = 1 << LayerMask.NameToLayer("Can");

        Direction = Vector3.down;
        Offset = Vector3.zero;
        Distance = 0.2f;

        cable = Cable.GetComponent<Cable>();
    }

    private void Update()
    {
        if (Screw)
        {
            s += 0.8f * Time.deltaTime;
        }
        else
        {
            s -= 0.8f * Time.deltaTime;
        }
        s = Mathf.Clamp(s, 0, 1);

        if (s > 0 & s < 1)
        {
            ScrewDriver.transform.position = Vector3.Lerp(startPosition, Holder.transform.position + 0.15f * Vector3.up, s);
            ScrewDriver.transform.localRotation = Quaternion.Lerp(startRotation, Quaternion.Euler(-22,60,-78), s);
            cable.Refresh(s);
        }

        if (Screw & s >= 1)
        {
            if (workpiece != null)
            {
                workpiece.Screw();
            }
        }

        if (Placed)
        {
            if (Physics.Raycast(Sensor.transform.position, Vector3.down, out hit, 0.05f, layerMask))
            {
                workpiece = hit.collider.attachedRigidbody.gameObject.GetComponent<WorkPiece>();
            }
        }
        else
        {
            if (!Placed & t <= 0)
            {
                workpiece = null;
            }
        }

        if (workpiece != null)
        {
            if (Placed & t < 1)
            {
                t += 0.8f * Time.deltaTime;
                t = Mathf.Clamp(t, 0, 1);

                workpiece.Fix(t < 1);

                workpiece.transform.position = Vector3.Lerp(Sensor.transform.position, Holder.transform.position, t);
                workpiece.transform.rotation = Quaternion.Euler(0, -180, 0);
            }
            
            if (!Placed & t > 0)
            {
                t -= 0.8f * Time.deltaTime;
                t = Mathf.Clamp(t, 0, 1);

                workpiece.Fix(t > 0);

                workpiece.transform.position = Vector3.Lerp(Sensor.transform.position, Holder.transform.position, t);
                workpiece.transform.rotation = Quaternion.Euler(0, -180, 0);
            }
        }
    }
}
