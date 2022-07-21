using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Trolley : MonoBehaviour
{
    public float Position;
    public Vector3 Axis;
    public float Length;

    private Vector3 startPosition;
    private List<Robe> robes;

    private void Start()
    {
        startPosition = transform.localPosition;
        robes = gameObject.GetComponentsInChildren<Robe>().ToList();
    }

    private void Update()
    {
        transform.localPosition = startPosition - Position * Axis;
        robes.ForEach(r => r.Length = Length);
    }
}
