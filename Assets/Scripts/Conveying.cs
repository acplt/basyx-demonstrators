using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Conveying: MonoBehaviour
{
    public float Velocity;
    public Vector3 Direction;

    public List<WorkPiece> loads;
    private Vector3 position;
    private BoxCollider collider;
    private WorkPiece workPiece;
    private List<Conveying> conveyors;
    private Detection detection;

	private void Start ()
    {
        loads = new List<WorkPiece>();
        collider = gameObject.GetComponent<BoxCollider>();
        conveyors = GameObject.FindObjectsOfType<Conveying>().ToList();
        detection = GameObject.FindObjectOfType<Detection>();
    }
	
	private void Update()
    {
        for (int i = 0; i < loads.Count; i++)
        {
            if (loads[i] != null)
            {
                if (Vector3.Distance(loads[i].Position, collider.transform.position) > 0.5f * collider.size.z + 0.1f)
                {
                    loads.Remove(loads[i]);
                }
                else
                {
                    if (Velocity != 0)
                    {
                        loads[i].Convey(Velocity * Time.deltaTime * Direction);
                    }
                }
            }
        }
        loads.RemoveAll(l => l == null);
    }

    private void OnCollisionEnter(Collision collision)
    {
        workPiece = collision.gameObject.GetComponent<WorkPiece>();

        foreach (Conveying conveyor in conveyors)
        {
            if (conveyor.gameObject.GetInstanceID() != gameObject.GetInstanceID())
            {
                if (conveyor.loads.Contains(workPiece))
                {
                    conveyor.loads.Remove(workPiece);
                }
            }
        }

        if (detection.loads.Contains(workPiece))
        {
            detection.loads.Remove(workPiece);
        }

        if (!loads.Contains(workPiece))
        {
            loads.Add(workPiece);
        }
    }
}
