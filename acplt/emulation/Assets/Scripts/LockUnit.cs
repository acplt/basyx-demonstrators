using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockUnit : MonoBehaviour
{
    public bool Unlock;
    public bool Detected;

    public bool Unlocked;

    public Vector3 Axis;
    public float Speed;
    public float MinPos;
    public float MaxPos;

    private float value;
    private Vector3 startPosition;
    private Clamping clamping;

    private void Start()
    {
        startPosition = transform.localPosition;
        clamping = gameObject.GetComponentInChildren<Clamping>();
    }

    private void Update()
    {
        if (Unlock)
        {
            value += Speed * Time.deltaTime;
        }
        else
        {
            value -= Speed * Time.deltaTime;
        }
        value = Mathf.Clamp(value, MinPos, MaxPos);
        transform.localPosition = startPosition + value * Axis;

        Detected = clamping.Detected;
        Unlocked = value >= MaxPos;
    }


}
