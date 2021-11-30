using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Xml;
using System.Globalization;

public class BaSysApplication : MonoBehaviour
{
    public string Url;
    public List<GameObject> CameraPositions;

    private float accum;
    private int frames;
    private float timeleft;
    private float refreshValue;
    private float absolutValue;
    private float fps;
    private float updateInterval;
    private GUIStyle boxStyle;
    private GUIStyle buttonRedStyle;
    private GUIStyle buttonGrayStyle;
    private List<Pallet> pallets;
    private List<WorkPiece> pieces;
    private List<Conveying> conveyors;
    private List<Detection> detections;
    private List<Screw> screws;
    private MouseNavigator mouseNavigator;
    private SHM shm;
    private int cameraIndex;
    private int cameraOldIndex;

    private void Awake()
    {
        cameraIndex = -1;

        shm = GameObject.FindObjectOfType<SHM>();
        if (!Application.isEditor)
        {
            shm.enabled = true;
        }

        string[] args = Environment.GetCommandLineArgs();

        if (!Application.isEditor)
        {
            if (args != null)
            {
                if (args.Length >= 1)
                {
                    for (int i = 1; i < args.Length; i++)
                    {
                        if (args[i].Contains("QUALITY"))
                        {
                            switch (args[i])
                            {
                                case "QUALITY-FASTEST": QualitySettings.SetQualityLevel(0); break;
                                case "QUALITY-FAST": QualitySettings.SetQualityLevel(1); break;
                                case "QUALITY-SIMPLE": QualitySettings.SetQualityLevel(2); break;
                                case "QUALITY-GOOD": QualitySettings.SetQualityLevel(3); break;
                                case "QUALITY-BEAUTIFUL": QualitySettings.SetQualityLevel(4); break;
                                case "QUALITY-FANTASTIC": QualitySettings.SetQualityLevel(5); break;
                            }
                        }
                        else if (args[i].Contains("RESOLUTION"))
                        {
                            List<string> items = args[i].Split('-').ToList();
                            if (items.Count() == 4)
                            {
                                int width = int.Parse(items[1]);
                                int heigth = int.Parse(items[2]);
                                bool fullscreen = items[3] == "FULL";
                                Screen.SetResolution(width, heigth, fullscreen);
                            }
                        }
                        else if (args[i].Contains("REGISTRY"))
                        {
                            Url = Url.Replace("localhost", args[i].Replace("REGISTRY-", ""));
                        }
                    }
                }
            }
        }

        List<SimBehaviour> simBehaviors = GameObject.FindObjectsOfType<SimBehaviour>().ToList();
        foreach (SimBehaviour simBehavior in simBehaviors)
        {
            QR_Label label = simBehavior.gameObject.FindComponent<QR_Label>();
            if (label != null)
            {
                label.Content = Url.Replace("{{aasId}}", simBehavior.gameObject.name.Split('_')[0]);
                label.Modify = true;
            }
        }
    }

    private void Start()
    {
        updateInterval = 0.5f;
        mouseNavigator = GameObject.FindObjectOfType<MouseNavigator>();
    }

    private void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.deltaTime / Time.timeScale;
        frames++;

        if (timeleft <= 0.0)
        {
            fps = frames / accum;
            refreshValue = 1000 / fps;
            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }

        if (Time.frameCount % 200 == 0)
        {
            GC.Collect();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void OnGUI()
    {
        if (boxStyle == null)
        {
            boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.background = MakeBackground(10, 10, new Color(0.5f, 0.5f, 0.5f, 0.6f));
            boxStyle.normal.textColor = Color.white;
            boxStyle.alignment = TextAnchor.MiddleLeft;
        }

        if (buttonRedStyle == null)
        {
            buttonRedStyle = new GUIStyle(GUI.skin.button);
            buttonRedStyle.normal.background = MakeBackground(10, 10, new Color(1.0f, 0.0f, 0.0f, 0.6f));
            buttonRedStyle.normal.textColor = Color.white;
            buttonRedStyle.alignment = TextAnchor.MiddleLeft;
        }

        if (buttonGrayStyle == null)
        {
            buttonGrayStyle = new GUIStyle(GUI.skin.button);
            buttonGrayStyle.normal.background = MakeBackground(10, 10, new Color(0.25f, 0.25f, 0.25f, 0.6f));
            buttonGrayStyle.normal.textColor = Color.white;
            buttonGrayStyle.alignment = TextAnchor.MiddleLeft;
        }

        if (fps < 50)
        {
            boxStyle.normal.textColor = Color.red;
        }
        else
        {
            if (fps < 80)
            {
                boxStyle.normal.textColor = Color.yellow;
            }
            else
            {
                boxStyle.normal.textColor = Color.green;
            }
        }
        GUI.Box(new Rect(0, 0, 120, 20), System.String.Format("Update: {0:F1} ms", refreshValue), boxStyle);

        boxStyle.normal.textColor = Color.yellow;
        string txt = "";
        if (shm.Enabled)
        {
            txt += "SHM  ";
        }
        GUI.Box(new Rect(120, 0, 100, 20), System.String.Format("{0}", txt), boxStyle);

        if (GUI.Button(new Rect(220, 0, 60, 20), "Reset", buttonRedStyle))
        {
            pallets = GameObject.FindObjectsOfType<Pallet>().ToList();
            foreach (Pallet p in pallets)
            {
                p.Reset();
            }

            pieces = GameObject.FindObjectsOfType<WorkPiece>().ToList();
            foreach (WorkPiece p in pieces)
            {
                p.Destroy();
            }

            screws = GameObject.FindObjectsOfType<Screw>().ToList();
            foreach (Screw s in screws)
            {
                s.Destroy();
            }

            conveyors = GameObject.FindObjectsOfType<Conveying>().ToList();
            foreach (Conveying c in conveyors)
            {
                c.loads.Clear();
            }

            detections = GameObject.FindObjectsOfType<Detection>().ToList();
            foreach (Detection d in detections)
            {
                d.loads.Clear();
            }
        }

        if (GUI.Button(new Rect(280, 0, 60, 20), "Mouse", buttonGrayStyle))
        {
            mouseNavigator.Active = true;
        }

        string[] toolbarStrings = {"Home", "KR01", "RB01", "AS01", "PP01", "RB02", "PF01", "Top"};

        cameraIndex = GUI.Toolbar(new Rect(350, 0, 500, 20), cameraIndex, toolbarStrings);
        if (cameraIndex != cameraOldIndex)
        {
            if (cameraIndex >= 0 && cameraIndex <= CameraPositions.Count - 1)
            {
                Camera.main.transform.position = CameraPositions[cameraIndex].transform.position;
                Camera.main.transform.rotation = CameraPositions[cameraIndex].transform.rotation;
            }
        }
        cameraOldIndex = cameraIndex;

        GUI.contentColor = Color.white;
        GUI.color = Color.white;

    }

    private Texture2D MakeBackground(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

}
