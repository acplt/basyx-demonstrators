using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MemoryMapping;

public class SHM : MonoBehaviour
{
    public bool Enabled;

    private SharedMemory shm;
    private string config;

    private void Start()
    {
        if (Application.isEditor)
        {
            config = Application.dataPath + "\\IO_Configuration.xml";
            Enabled = false;
        }
        else
        {
            config = Application.dataPath + "\\..\\" + "IO_Configuration.xml";
            Enabled = true;
        }

        SimBehaviour.Init(config);

        shm = new SharedMemory();
        shm.Load(config);
        shm.OpenOrCreate();

        if (shm.isOpen)
        {
            Debug.Log("SHM is active");
        }
    }

    private void OnApplicationQuit()
    {
        if (shm != null)
        {
            shm.Close();
        }
    }

    private void Update()
    {
        if (shm != null)
        {
            if (Enabled)
            {
                foreach (SimBehaviour behaviour in SimBehaviour.behaviours.Values)
                {
                    behaviour.Refresh(this);
                }
            }
        }
    }

    public bool GetBool(int index)
    {
        return shm.GetBool(index);
    }

    public int GetInt(int index)
    {
        return shm.GetInt(index);
    }

    public float GetFloat(int index)
    {
        return shm.GetFloat(index);
    }

    public void SetBool(int index, bool val)
    {
        shm.SetBool(index, val);
    }

    public void SetInt(int index, int val)
    {
        shm.SetInt(index, val);
    }

    public void SetFloat(int index, float val)
    {
        shm.SetFloat(index, val);
    }
}
