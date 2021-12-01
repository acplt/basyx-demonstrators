using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Replacement : SimBehaviour
{
    public bool Replace;
    public float Speed;

    public GameObject PP01;
    public GameObject PP04;

    public GameObject PP01_Location1;
    public GameObject PP01_Location2;
    public GameObject PP04_Location1;
    public GameObject PP04_Location2;


    private float position1;
    private float position2;
    private float position1Max;
    private float position2Max;
    private Vector3 startPosition1;
    private Vector3 startPosition2;

    private void Start()
    {
        position1Max = (PP01_Location2.transform.position - PP01_Location1.transform.position).magnitude;
        position2Max = (PP04_Location2.transform.position - PP04_Location1.transform.position).magnitude;
        startPosition1 = PP01.transform.position;
        startPosition2 = PP04.transform.position;
    }

    private void Update()
    {
        if (Replace)
        {
            position1 += Speed * Time.deltaTime;
            position1 = Mathf.Clamp(position1, 0, position1Max);
            if (position1 >= position1Max)
            {
                position2 += Speed * Time.deltaTime;
                position2 = Mathf.Clamp(position2, 0, position2Max);
            }
        }
        else
        {
            position2 -= Speed * Time.deltaTime;
            position2 = Mathf.Clamp(position2, 0, position2Max);
            if (position2 <= 0)
            {
                position1 -= Speed * Time.deltaTime;
                position1 = Mathf.Clamp(position1, 0, position1Max);
            }
        }

        PP01.transform.position = startPosition1 + position1 * (PP01_Location1.transform.position - PP01_Location2.transform.position).normalized;
        PP04.transform.position = startPosition2 - position2 * (PP04_Location1.transform.position - PP04_Location2.transform.position).normalized;

        if (Replace)
        {
            PP04.gameObject.SetActive(true);
            PP01.gameObject.SetActive(false);
        }
        else
        {
            PP04.gameObject.SetActive(false);
            PP01.gameObject.SetActive(true);
        }
    }
}
