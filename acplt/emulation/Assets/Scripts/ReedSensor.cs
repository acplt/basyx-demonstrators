using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReedSensor : MonoBehaviour
{
    public PneumaticCylinder Cylinder;
    public float Position;
    public float Window;
    public bool Detected;

    public GameObject Led;
    public Material Off;
    public Material On;

    private MeshRenderer ledRenderer;

	private void Start ()
    {
        ledRenderer = Led.GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (Cylinder != null)
        {
            Detected = false;

            if (Cylinder.Stroke >= Position - Window)
            {
                if (Cylinder.Stroke <= Position + Window)
                {
                    Detected = true;
                }
            }
        }

        if (Detected)
        {
            ledRenderer.sharedMaterial = On;
        }
        else
        {
            ledRenderer.sharedMaterial = Off;
        }
    }
}
