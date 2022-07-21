using UnityEngine;
using System.Collections;

public class Coater : MonoBehaviour
{
    public bool SwitchOn;
    public GameObject Spray;

    #region Methods
    private void Start ()
    {
        
	} 
	
	private void Update ()
    {
        Spray.SetActive(SwitchOn);
    }
    #endregion

}
