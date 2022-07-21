using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScrewGenerator : SimBehaviour
{
    public bool Restart;
    public bool Create;
    public bool Delete;

    public int LoadType;

    public Screw Screw;
    private bool done;

    private GameObject newScrew;
    private Screw screw;
    private List<Screw> screws;
    private GameObject screwContainer;

    private void Start()
    {
        screwContainer = GameObject.Find("/Dynamics/Screws");
        screws = new List<Screw>();
        Screw.Hide();

        if (Restart)
        {
            Invoke("Generate", 1.0f);
        }
    }

    private void Update()
    {
        if (Create & !done)
        {
            Generate();
        }

        if (Delete & !done)
        {
            Destroy();
        }

        done = Create | Delete;
    }

    public void Generate()
    {
        newScrew = GameObject.Instantiate(Screw.gameObject);
        newScrew.transform.position = transform.position;
        newScrew.tag = "Created";
        newScrew.name = "Screw";
        screw = newScrew.GetComponent<Screw>();
        screw.Show();
        newScrew.transform.parent = screwContainer.transform;
        screws.Add(screw);
    }

    public void Destroy()
    {
        screws = GameObject.FindObjectsOfType<Screw>().ToList();
        foreach (Screw s in screws)
        {
            s.Destroy();
        }
        screws.Clear();

    }
}
