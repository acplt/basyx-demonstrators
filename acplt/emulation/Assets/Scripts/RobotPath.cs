using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RobotPath
{
    public AnimationCurve Axis1;
    public AnimationCurve Axis2;
    public AnimationCurve Axis3;
    public AnimationCurve Axis4;

    public float GetAxis1(float time)
    {
        return Axis1.Evaluate(time);
    }

    public float GetAxis2(float time)
    {
        return Axis2.Evaluate(time);
    }

    public float GetAxis3(float time)
    {
        return Axis3.Evaluate(time);
    }

    public float GetAxis4(float time)
    {
        return Axis4.Evaluate(time);
    }
}
