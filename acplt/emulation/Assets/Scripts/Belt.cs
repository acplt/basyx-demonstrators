using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Belt : MonoBehaviour
{
    public float Velocity;

    private Vector2 offset;
    private Material material;

	private void Start ()
    {
        material = gameObject.GetComponent<MeshRenderer>().sharedMaterial;	
	}
	
	private void Update ()
    {
        offset = material.GetTextureOffset("_MainTex");
        offset.x -= Velocity;
        material.SetTextureOffset("_MainTex", offset);
	}
}
