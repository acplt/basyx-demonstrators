using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cable : MonoBehaviour
{
    public GameObject ScrewDriver;

    public float Slice;
    public float length;
    public Vector3 offset;

    private void Start()
    {
    }

    public void Refresh(float t)
    {
        length = Mathf.Lerp(0, 0.051f, t);
        offset = Vector3.Lerp(new Vector3(98, 0, 4), new Vector3(92, 2, 3), t);

        transform.LookAt(ScrewDriver.transform, Vector3.up);
        transform.localScale = new Vector3(1, 0.015f + length, 1);
        transform.rotation *= Quaternion.Euler(offset);
    }
}
