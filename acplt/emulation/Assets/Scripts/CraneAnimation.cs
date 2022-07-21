using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneAnimation : MonoBehaviour
{
    public bool Enabled;
    public bool Run;
    public float Speed;
    public Crane crane;

    private int step;

    private void Start()
    {
        step = 10;
    }

    private void Update()
    {
        if (Enabled)
        {
            if (Run)
            {
                switch (step)
                {
                    //case 10: if (Move(ref crane.Hook_ZAxis_Position, Speed, 0.061f)) { step += 10; }; break;
                    //case 20: if (Move(ref crane.Hook_YAxis_Position, Speed, 2.017f)) { step += 10; }; break;
                    //case 30: if (Move(ref crane.Hook_XAxis_Position, Speed, 2.085f)) { step += 10; }; break;
                    //case 40: if (Move(ref crane.Hook_ZAxis_Position, Speed, 1.5f)) { step += 10; }; break;
                    //case 50: if (Move(ref crane.Hook_XAxis_Position, -Speed, 0.0f)) { step += 10; }; break;
                    //case 60: if (Move(ref crane.Hook_YAxis_Position, Speed, 10.723f)) { step += 10; }; break;
                    //case 70: if (Move(ref crane.Hook_XAxis_Position, Speed, 1.46f)) { step += 10; }; break;
                    //case 80: if (Move(ref crane.Hook_ZAxis_Position, -Speed, 0.285f)) { step += 10; }; break;
                    //case 90: if (Move(ref crane.Hook_XAxis_Position, -Speed, 0.0f)) { step += 10; }; break;

                    case 10: if (Move(ref crane.Hook_ZAxis_Position, Speed, 0.285f)) { step += 10; }; break;
                    case 20: if (Move(ref crane.Hook_YAxis_Position, Speed, 13.41f)) { step += 10; }; break;
                    case 30: if (Move(ref crane.Hook_XAxis_Position, Speed, 1.715f)) { step += 10; }; break;
                    case 40: if (Move(ref crane.Hook_ZAxis_Position, Speed, 1.50f)) { step += 10; }; break;
                    case 50: if (Move(ref crane.Hook_XAxis_Position, -Speed, 0.0f)) { step += 10; }; break;
                    case 60: if (Move(ref crane.Hook_ZAxis_Position, -Speed, 0.0f)) { step += 10; }; break;
                }
            }
        }
    }

    private bool Move(ref float value, float Speed, float target)
    {
        value += Speed * Time.deltaTime;
        if (Speed > 0)
        {
            if (value > target)
            {
                value = target;
            }
        }
        if (Speed < 0)
        {
            if (value < target)
            {
                value = target;
            }
        }
        return value == target;
    }
}
