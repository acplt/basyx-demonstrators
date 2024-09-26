using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalletFeeder : SimBehaviour
{
    public int WorkPieceCount;
    public Vector3 Direction;
    public Vector3 Offset;
    public float Distance;

    private int layerMask;
    private RaycastHit hit;
    public Pallet Pallet;

    private void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Pallet");
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position + Offset, Direction, out hit, Distance, layerMask))
        {
            Pallet = hit.collider.transform.parent.gameObject.GetComponent<Pallet>();
        }
        else
        {
            Pallet = null;
        }

        if (Pallet != null)
        {
            WorkPieceCount = Pallet.WorkPieceCount;
        }
        else
        {
            WorkPieceCount = 0;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position + Offset, Direction * Distance, Color.yellow);
    }
}
