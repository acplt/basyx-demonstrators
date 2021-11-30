using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorkPiece : MonoBehaviour
{
    private Rigidbody body;
    private List<MeshRenderer> renderers;
    private WorkPiece workpiece;
    private Screw screw;
    private GameObject child;
    private Vector3 offset;
    private List<MeshCollider> colliders;

    public Vector3 Position
    {
        get { return transform.position; }
    }

    public void Init()
    {
        body = gameObject.GetComponent<Rigidbody>();
        screw = gameObject.GetComponent<Screw>();
    }

    private void FixedUpdate()
    {
        body = gameObject.GetComponent<Rigidbody>();

        if (body.IsSleeping())
        {
            body.WakeUp();
        }
    }

    public void Hide()
    {
        renderers = gameObject.GetComponentsInChildren<MeshRenderer>().ToList();
        renderers.ForEach(r => r.enabled = false);

        colliders = gameObject.GetComponentsInChildren<MeshCollider>().ToList();
        colliders.ForEach(r => r.enabled = false);

        body = gameObject.GetComponent<Rigidbody>();
        body.isKinematic = true;

        screw = gameObject.GetComponentInChildren<Screw>();
        screw.Hide();
    }


    public void Show(int loadType)
    {
        renderers = gameObject.GetComponentsInChildren<MeshRenderer>().ToList();
        colliders = gameObject.GetComponentsInChildren<MeshCollider>().ToList();

        if (loadType == 3)
        {
            renderers[0].enabled = true;
            colliders[0].enabled = true;
        }
        else if (loadType == 2)
        {
            renderers[0].enabled = true;
            renderers[2].enabled = true;
            colliders[0].enabled = true;
            colliders[1].enabled = true;
        }
        else if (loadType == 1)
        {
            renderers[2].enabled = true;
            colliders[1].enabled = true;
        }
        else
        {
            renderers.ForEach(r => r.enabled = true);
            colliders.ForEach(r => r.enabled = true);
        }

        body = gameObject.GetComponent<Rigidbody>();
        body.isKinematic = false;
    }

    public void Destroy()
    {
        if (gameObject.tag == "Created")
        {
            transform.position += new Vector3(0, 10, 0);
            Invoke("Delete", 1.0f);
        }
    }

    private void Delete()
    {
        GameObject.DestroyImmediate(gameObject);
    }

    public void Move(Vector3 direction)
    {
        body = gameObject.GetComponent<Rigidbody>();
        body.MovePosition(body.position + direction); 
    }

    public void Insert()
    {
        screw = gameObject.GetComponentInChildren<Screw>();
        screw.transform.position += 0.03f * Vector3.up;
        screw.gameObject.GetComponent<BoxCollider>().enabled = true;
        screw.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        screw.gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

    public void Screw()
    {
        screw = gameObject.GetComponentInChildren<Screw>();
        screw.Fix();
    }

    public void Fix(bool value)
    {
        body.useGravity = !value;
    }

    public void Convey(Vector3 value)
    {
        if (offset == Vector3.zero)
        {
            offset = value;
        }
        else
        {
            offset += value;
            offset = 0.5f * offset;
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (gameObject.transform.parent.gameObject.GetComponent<WorkpieceGenerator>() == null)
                {
                    body.isKinematic = true;
                    body.transform.position += 10.0f * Vector3.up;
                    Invoke("Remove", 1.0f);
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (!body.isKinematic)
        {
            body.MovePosition(body.position + offset);
        }
        offset = Vector3.zero;
    }

    private void Remove()
    {
        GameObject.Destroy(gameObject);
    }

    public void SetMaterial(Material color)
    {
        renderers = gameObject.GetComponentsInChildren<MeshRenderer>().ToList();
        renderers[0].sharedMaterial = color;
    }

    public Material GetMaterial()
    {
        renderers = gameObject.GetComponentsInChildren<MeshRenderer>().ToList();
        return renderers[0].sharedMaterial;
    }

    public void SetKinematic(bool value)
    {
        body.isKinematic = value;
    }
}
