using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pallet : MonoBehaviour
{
    public int LoadType;

    private Rigidbody body;
    private Connector joint;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private List<Collider> colliders;
    private List<GameObject> workPieces;

    private void Start()
    {
        workPieces = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.tag == "WorkPiece")
            {
                workPieces.Add(child.gameObject);
            }
        }

        foreach (GameObject workPiece in workPieces)
        {
            if (LoadType == 0)
            {
                workPiece.transform.GetChild(0).gameObject.SetActive(false);
                workPiece.transform.GetChild(1).gameObject.SetActive(false);
                workPiece.transform.GetChild(2).gameObject.SetActive(false);
            }
            else if (LoadType == 1)
            {
                workPiece.transform.GetChild(0).gameObject.SetActive(false);
                workPiece.transform.GetChild(1).gameObject.SetActive(false);
                workPiece.transform.GetChild(2).gameObject.SetActive(true);
            }
        }
        body = gameObject.GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        colliders = transform.Find("Frame").GetComponentsInChildren<Collider>().ToList();
        colliders.Add(transform.Find("Cage").GetComponentInChildren<Collider>());
    }

    private void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Lifter")
        {
            joint = collision.transform.parent.gameObject.GetComponentInChildren<Connector>();
        }

        if (collision.gameObject.tag == "Ground")
        {
            if (joint != null)
            {
                joint.Disconnect();
                joint = null;
            }
        }

        if (collision.gameObject.tag == "Guide")
        {
            if (joint != null)
            {
                joint.Disconnect();
                joint = null;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Lifter")
        {
            joint = collision.transform.parent.gameObject.GetComponentInChildren<Connector>();
        }

        if (collision.gameObject.tag == "Ground")
        {
            if (joint != null)
            {
                joint.Connect(body);
            }
        }
    }

    public void Reset()
    {
        body.isKinematic = true;
        body.useGravity = false;
        colliders.ForEach(c => c.enabled = false);
        transform.position = startPosition + 0.05f * Vector3.up;
        transform.rotation = startRotation;
        colliders.ForEach(c => c.enabled = true);
        body.isKinematic = false;
        body.useGravity = true;
    }

    public void Destroy()
    {
        if (gameObject.tag != "Locked")
        {
            transform.position += new Vector3(0, 10, 0);
            Invoke("Delete", 1.0f);
        }
    }

    private void Delete()
    {
        GameObject.DestroyImmediate(gameObject);
    }

    public void RemoveWorkPiece()
    {
        for (int i = 0; i < workPieces.Count; i++)
        {
            if (workPieces[i].transform.GetChild(2).gameObject.activeSelf)
            {
                workPieces[i].transform.GetChild(0).gameObject.SetActive(false);
                workPieces[i].transform.GetChild(1).gameObject.SetActive(false);
                workPieces[i].transform.GetChild(2).gameObject.SetActive(false);
                break;
            }
        }
    }

    public void AddWorkPiece(int loadType, Material color)
    {
        for (int i = 0; i < workPieces.Count; i++)
        {
            if (!workPieces[i].transform.GetChild(2).gameObject.activeSelf)
            {
                if (loadType == 3)
                {
                    workPieces[i].transform.GetChild(0).gameObject.SetActive(true);
                    workPieces[i].transform.GetChild(1).gameObject.SetActive(true);
                    workPieces[i].transform.GetChild(2).gameObject.SetActive(true);
                }
                else if (loadType == 2)
                {
                    workPieces[i].transform.GetChild(0).gameObject.SetActive(true);
                    workPieces[i].transform.GetChild(2).gameObject.SetActive(true);
                }
                else if (loadType == 1)
                {
                    workPieces[i].transform.GetChild(2).gameObject.SetActive(true);
                }

                if (color != null)
                {
                    var renderers = workPieces[i].gameObject.GetComponentsInChildren<MeshRenderer>().ToList();
                    renderers[0].sharedMaterial = color;
                }
                break;
            }
        }
    }
}
