using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public float PositionX;
    public float PositionY;
    public float PositionZ;

    public Vector3 XAxis;
    public Vector3 YAxis;
    public Vector3 ZAxis;

    private Vector3 startPosition;
    private Rigidbody body;

    private void Start()
    {
        startPosition = transform.position;
        body = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        body.MovePosition(startPosition + PositionX * XAxis + PositionY * YAxis - PositionZ * ZAxis);
    }
}
