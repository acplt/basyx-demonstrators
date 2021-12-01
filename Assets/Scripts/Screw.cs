using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Screw : MonoBehaviour
{
    private Rigidbody body;
    private List<MeshRenderer> renderers;
    private WorkPiece workPiece;
    private List<Collider> colliders;

    private void OnTriggerEnter(Collider collision)
    {
        workPiece = collision.transform.parent.gameObject.GetComponent<WorkPiece>();
        body = gameObject.GetComponent<Rigidbody>();
    }

    public void Fix()
    {
        transform.localPosition = Vector3.zero;
        colliders = gameObject.GetComponents<Collider>().ToList();
        colliders.ForEach(c => c.enabled = false);
        body.isKinematic = true;
    }

    public void Insert()
    {
        if (workPiece != null)
        {
            Hide();
            Destroy();

            workPiece.Insert();
        }
    }

    public void Show()
    {
        renderers = gameObject.GetComponentsInChildren<MeshRenderer>().ToList();
        renderers[0].enabled = true;

        colliders = gameObject.GetComponents<Collider>().ToList();
        colliders.ForEach(c => c.enabled = true);

        body = gameObject.GetComponent<Rigidbody>();
        body.isKinematic = false;
    }

    public void Hide()
    {
        renderers = gameObject.GetComponentsInChildren<MeshRenderer>().ToList();
        renderers.ForEach(r => r.enabled = false);

        colliders = gameObject.GetComponents<Collider>().ToList();
        colliders.ForEach(c => c.enabled = false);

        body = gameObject.GetComponent<Rigidbody>();
        body.isKinematic = true;
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
}
