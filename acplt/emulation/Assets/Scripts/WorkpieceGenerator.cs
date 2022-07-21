using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorkpieceGenerator : SimBehaviour
{
    public bool Restart;
    public bool Enabled;
    public bool Create;
    public bool Delete;

    public int LoadType;
    public int DynType;

    public WorkPiece WorkPiece;
    private bool done;

    private GameObject newPiece;
    private WorkPiece workpiece;
    private List<WorkPiece> pieces;
    private GameObject workpieces;

    private void Start()
    {
        workpieces = GameObject.Find("/Dynamics/WorkPieces");
        pieces = new List<WorkPiece>();
        WorkPiece.Init();
        WorkPiece.Hide();

        if (Restart)
        {
            Invoke("Generate", 1.0f);
        }
    }

    private void Update()
    {
        if (Enabled)
        {
            if (Create & !done)
            {
                Generate();
            }

            if (Delete & !done)
            {
                Destroy();
            }

            done = Create | Delete;
        }
    }

    public void Generate()
    {
        newPiece = GameObject.Instantiate(WorkPiece.gameObject);
        newPiece.transform.position = transform.position;
        newPiece.tag = "Created";
        newPiece.name = "WorkPiece";
        workpiece = newPiece.GetComponent<WorkPiece>();
        workpiece.Show(LoadType);
        workpiece.transform.parent = workpieces.transform;
        pieces.Add(workpiece);
        if (DynType == 1)
        {
            newPiece.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    public void Destroy()
    {
        foreach (WorkPiece p in pieces)
        {
            p.Destroy();
        }
        pieces.Clear();
    }
}
