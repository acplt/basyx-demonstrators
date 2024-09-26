using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robe : MonoBehaviour
{
    public float Length;
    [HideInInspector]
    public float Factor;
    public Transform Direction;

    private GameObject cylinder;
    private Vector3 startScale;
    
    private void Start()
    {
        cylinder = gameObject;
        startScale = cylinder.transform.localScale;
    }

    private void Update()
    {
        cylinder.transform.localScale = startScale + Factor * new Vector3(1, Length, 1);
        cylinder.transform.LookAt(Direction);
        cylinder.transform.Rotate(90, 0, 0, Space.Self);
    }
}
