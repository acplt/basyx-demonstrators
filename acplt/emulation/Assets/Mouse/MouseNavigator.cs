using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MouseNavigator : MonoBehaviour
{
    public float RotateSpeed = 5.0f;
    public float MoveSpeed = 1.0f;
    public float ZoomSpeed = 20.0f;
    public float PanSensitivity = 1.0f;
    public float RotSensitivity = 1.0f;
    public float ZoomSensitivity = 1.0f;
    public bool Active;
    public bool Reset;

    private Camera currentCamera;
    private float x;
    private float y;
    private Vector3 position;
    private Vector3 rotation;
    private Vector3 translation;
    private float zoomAmount;
    private Vector3 homePosition;
    private Vector3 homeRotation;
    private Vector2 lastAxis;
    private Vector2 axis;
    private Vector2 currentAxis;
    private Rect rect = new Rect(0, 0, 1, 1);

    void Start()
    {
        currentCamera = null;
        homePosition = Camera.main.transform.position;
        homeRotation = Camera.main.transform.rotation.eulerAngles;
    }

    void Update()
    {
        if (Reset)
        {
            Reset = false;
            if (currentCamera != null)
            {
                currentCamera.transform.position = homePosition;
                currentCamera.transform.rotation = Quaternion.Euler(homeRotation);
            }
        }

        if (Active)
        {
            Active = false;
            if (currentCamera != null)
            {
                currentCamera = null;
            }
            else
            {
                currentCamera = Camera.main;
            }
        }


        if (currentCamera != null)
        {
            currentAxis = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            if (Vector2.Distance(currentAxis, lastAxis) > 100)
            {
                axis = Vector2.zero;
            }
            {
                axis = new Vector2(-(lastAxis.x - currentAxis.x), -(lastAxis.y - currentAxis.y));
            }


            x = 0.1f * axis.x;
            y = 0.1f * axis.y;

            zoomAmount = 80.0f * Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
            if (zoomAmount != 0)
            {
                translation = -zoomAmount * ZoomSensitivity * currentCamera.transform.forward.normalized;
                currentCamera.transform.Translate(translation, Space.World);
            }

            if (Input.GetMouseButton(0))
            {
                float yaw = x * RotateSpeed * RotSensitivity;
                float pitch = y * RotateSpeed * RotSensitivity;
                float roll = 0;
                currentCamera.transform.Rotate(Vector3.up, yaw, Space.World);
                currentCamera.transform.Rotate(currentCamera.transform.right, -pitch, Space.World);
                currentCamera.transform.Rotate(currentCamera.transform.forward, -roll, Space.World);
            }

            if (Input.GetMouseButton(1))
            {
                translation = new Vector3(x * MoveSpeed * PanSensitivity, y * MoveSpeed * PanSensitivity, 0);
                currentCamera.transform.Translate(-translation, Space.Self);
            }

            if (Input.GetMouseButton(2))
            {
                if (currentAxis.y != lastAxis.y)
                {
                    zoomAmount = (currentAxis.y - lastAxis.y) * ZoomSpeed;
                }
                translation = -zoomAmount * ZoomSensitivity * currentCamera.transform.forward.normalized;
                currentCamera.transform.Translate(translation, Space.World);
            }

            lastAxis = currentAxis;
        }
    }

    void OnGUI()
    {
        if (currentCamera != null)
        {
            rect = currentCamera.rect;
            rect.width *= Screen.width;
            rect.height *= Screen.height;
            rect.x *= Screen.width;
            rect.y = (1 - rect.y) * Screen.height - rect.height;
            DrawFrame(rect);
        }
    }

    private void DrawFrame(Rect rect)
    {
        Color col = new Color(0, 1, 0.13f, 0.5f);
        Drawing.DrawLine(rect, col, 10);
    }

    private Texture2D MakeFrame(int width, int height)
    {
        Color col = new Color(0, 1, 0.13f, 0.2f);
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

    private Texture2D MakeBackground(int width, int height)
    {
        Color col = new Color(0.5f, 0.5f, 0.5f, 0.4f);
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
