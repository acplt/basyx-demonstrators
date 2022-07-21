using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class QR_Label : MonoBehaviour
{
    public string Content;
    public bool Modify;

    private void Update()
    {
        if (Modify)
        {
            Modify = false;
            QR_Code.Size = 256;
            gameObject.GetComponent<MeshRenderer>().sharedMaterial = QR_Code.Encode(Content);
        }
    }

    private void OnMouseDown()
    {
        //Debug.Log("Mousebutton clicked");
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Application.OpenURL(Content);
        }
    }

}
