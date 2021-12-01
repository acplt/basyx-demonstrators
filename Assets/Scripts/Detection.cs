using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Detection : MonoBehaviour
{
    public bool isKinematic;
    private bool oldKinematic;
    private WorkPiece workPiece;

    public List<WorkPiece> loads;

    private void Start()
    {
        loads = new List<WorkPiece>();
    }

    private void OnCollisionEnter(Collision other)
    {
        workPiece = other.gameObject.GetComponent<WorkPiece>();
        if (!loads.Contains(workPiece))
        {
            if (loads.Count > 7)
            {
                loads.RemoveAt(0);
            }
            other.transform.parent = transform;
            loads.Add(workPiece);
        }
    }

    private void Update()
    {
        if (isKinematic != oldKinematic)
        {
            foreach (WorkPiece obj in loads)
            {
                if (obj.transform.parent = transform)
                {
                    obj.GetComponent<Rigidbody>().isKinematic = isKinematic;
                }
            }
        }
        oldKinematic = isKinematic;
     }
}
 