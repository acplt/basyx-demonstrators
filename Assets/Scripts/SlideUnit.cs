using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideUnit : MonoBehaviour
{
    public LockUnit Unit;
    public Vector3 Direction;
    public float Speed;

    private Dictionary<int, Rigidbody> workpieces;
    private int id;
    private bool stopped;

    private void Start()
    {
        workpieces = new Dictionary<int, Rigidbody>();
        stopped = false;
    }

    private void Update()
    {
        if (workpieces == null)
        {
            workpieces = new Dictionary<int, Rigidbody>();
        }

        if (Unit != null)
        {
            stopped = Unit.Detected;
        }
        if (!stopped)
        {
            foreach (Rigidbody piece in workpieces.Values)
            {
                piece.MovePosition(piece.position + Speed * Time.deltaTime * Direction);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        id = collision.gameObject.GetInstanceID();

        if (!workpieces.ContainsKey(id))
        {
            workpieces.Add(id, collision.rigidbody);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        id = collision.gameObject.GetInstanceID();

        if (!workpieces.ContainsKey(id))
        {
            workpieces.Add(id, collision.rigidbody);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        id = collision.gameObject.GetInstanceID();

        if (workpieces.ContainsKey(id))
        {
            workpieces.Remove(id);
        }
    }

}
