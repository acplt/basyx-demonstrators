using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : SimBehaviour
{
    public GameObject FillLevel;
    public float Level;
    public float MaxLevel;

    private float scale;
    private Vector3 localScale;
    private MeshRenderer fillRenderer;

    private void Start()
    {
        localScale = FillLevel.transform.localScale;
        fillRenderer = FillLevel.transform.GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        scale = Level / MaxLevel;
        fillRenderer.enabled = scale > 0.001f;
        FillLevel.transform.localScale = new Vector3(localScale.x, localScale.y, scale * localScale.z);
    }
}
