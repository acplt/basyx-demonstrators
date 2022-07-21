using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extentions
{
    public static T FindComponent<T>(this GameObject g) where T : Component
    {
        foreach (Transform child in g.transform)
        {
            T cmp = child.gameObject.GetComponent<T>();
            if (cmp != null)
            {
                return cmp;
            }
        }


        foreach (Transform child in g.transform)
        {
            return child.gameObject.FindComponent<T>();
        }

        return null;
    }
}
