using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector : MonoBehaviour
{
    private Rigidbody connectedBody;
    private Vector3 offset;

    public void Connect(Rigidbody body)
    {
        if (connectedBody == null)
        {
            connectedBody = body;
            body.freezeRotation = true;
            connectedBody.useGravity = false;
            offset = transform.position - body.transform.position;
        }
    }

    public void Disconnect()
    {
        if (connectedBody != null)
        {
            connectedBody.freezeRotation = false;
            connectedBody.useGravity = true;
            connectedBody = null;
        }
    }

    private void Update()
    {
        if (connectedBody != null)
        {
            connectedBody.MovePosition(transform.position - offset);
            //connectedBody.transform.position = transform.position - offset;
        }
    }
}
