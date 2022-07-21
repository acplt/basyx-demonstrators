using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public float Position;
    public Vector3 Axis;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;    
    }

    void Update()
    {
        transform.localPosition = startPosition + Position * Axis;
    }
}

/*
2	X position trolley in m	float
3	Y position bridge in m	float
4	X position load in m	float
5	Y position load in m	float
6	Z position load in m	float
 
*/
