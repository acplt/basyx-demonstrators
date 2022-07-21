using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.IO;

public class CoatingStation: SimBehaviour
{
    #region Variables
    public bool CoatingOn;
    public bool Color;
    public bool EntryDoorOpen;
    public bool EntryDoorClose;
    public bool ExitDoorOpen;
    public bool ExitDoorClose;
    public bool EntryDoorClosed;
    public bool EntryDoorOpened;
    public bool ExitDoorClosed;
    public bool ExitDoorOpened;
    public bool LightBarrierFree;
    public int PaintTime;
    public Material Blue;
    public Material Yellow;
    public BoxCollider trigger;
    public GameObject EntryDoor;
    public GameObject ExitDoor;
    public Coater Coater1;
    public Coater Coater2;
    public LightBarrier LightBarrier;

    public float DoorDistance;
    public float DoorSpeed;

    private bool started;
    private Vector3 entryDoorStartPosition;
    private Vector3 exitDoorStartPosition;
    private float entryDoorPosition;
    private float exitDoorPosition;
    private WorkPiece workPiece;
    private float coatingDuration;
    #endregion


    #region Methods

    private void Start ()
    {
        entryDoorStartPosition = EntryDoor.transform.position;
        exitDoorStartPosition = ExitDoor.transform.position;
    }

    private void Update()
    {
        MoveEntryDoor();
        MoveExitDoor();
        StartCoarting();
        Detection();
    }

    private void Detection()
    {
        LightBarrierFree = !LightBarrier.Detected;
    }

    private void MoveEntryDoor ()
    {
        if (EntryDoorOpen & ! EntryDoorClose)
        {
            entryDoorPosition += DoorSpeed * Time.deltaTime;
        }
        else if (EntryDoorClose & !EntryDoorOpen)
        {
            entryDoorPosition -= DoorSpeed * Time.deltaTime;
        }
        entryDoorPosition = Mathf.Clamp(entryDoorPosition, 0, DoorDistance);
        EntryDoorClosed = entryDoorPosition <= 0.0f;
        EntryDoorOpened = entryDoorPosition >= DoorDistance;

        EntryDoor.transform.position = entryDoorStartPosition + entryDoorPosition * Vector3.up;
    }

    private void MoveExitDoor()
    {
         if (ExitDoorOpen & !ExitDoorClose)
        {
            exitDoorPosition += DoorSpeed * Time.deltaTime;
        }
        else if (ExitDoorClose & !ExitDoorOpen)
        {
            exitDoorPosition -= DoorSpeed * Time.deltaTime;
        }
        exitDoorPosition = Mathf.Clamp(exitDoorPosition, 0, DoorDistance);

        ExitDoorClosed = exitDoorPosition <= 0.0f;
        ExitDoorOpened = exitDoorPosition >= DoorDistance;

        ExitDoor.transform.position = exitDoorStartPosition + exitDoorPosition * Vector3.up;

    }

    private void StartCoarting()
    {
        if (CoatingOn & !started)
        {
            InvokeRepeating("Coating", 0, 0.1f);
        }
        started = CoatingOn;

        if (!CoatingOn)
        {
            coatingDuration = 0.0f;
            CancelInvoke("Coating");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        workPiece = other.gameObject.GetComponentInParent<WorkPiece>();
    }

    private void OnTriggerExit(Collider other)
    {
        workPiece = null;
    }

    private void Coating()
    {
        coatingDuration += 0.1f;
        if (coatingDuration >= (float)PaintTime)
        {
            Coater1.SwitchOn = false;
            Coater2.SwitchOn = false;
        }
        else
        {
            Coater1.SwitchOn = Color;
            Coater2.SwitchOn = !Color;
        }

        if (workPiece != null)
        {
            if (Color)
            {
                workPiece.SetMaterial(Yellow);
            }
            else
            {
                workPiece.SetMaterial(Blue);
            }
        }
    }
    #endregion
}
