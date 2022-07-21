using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltConveyor : SimBehaviour
{
    public bool Forward;
    public bool Backward;
    public float Velocity;
    public float Acceleration;

    private Conveying conveying;
    private MeshRenderer beltRenderer;
    private float speed;
    private float value;
    private Vector2 offset;

    private void Start()
    {
        beltRenderer = transform.Find("Rubber").gameObject.GetComponent<MeshRenderer>();
        conveying = gameObject.GetComponentInChildren<Conveying>();
    }

    private void Update()
    {
        if (Forward)
        {
            speed += Acceleration * Time.deltaTime;
        }
        else if (Backward)
        {
            speed -= Acceleration * Time.deltaTime;
        }
        else
        {
            if (speed < 0)
            {
                speed += Acceleration * Time.deltaTime;
            }
            else if (speed > 0)
            {
                speed -= Acceleration * Time.deltaTime;
            }
            if (Mathf.Abs(speed) < 0.01f)
            {
                speed = 0.0f;
            }
        }


        speed = Mathf.Clamp(speed, -Velocity, Velocity);
        conveying.Velocity = speed;

        value += 10.0f * speed * Time.deltaTime;
        if (conveying.Direction.z != 0)
        {
            offset.x = value;
        }
        else if (conveying.Direction.x != 0)
        {
            offset.x = -value;
        }
        beltRenderer.material.SetTextureOffset("_MainTex", offset);
    }
}
