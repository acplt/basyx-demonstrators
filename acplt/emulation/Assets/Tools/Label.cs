using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class Label : MonoBehaviour
{
    public enum COLOR
    {
        BLACK,
        YELLOW,
        ORANGE,
        WHITE,
        BLUE,
        GREEN,
        RED,
        GRAY
    }

    public string Text;
    public COLOR ForeColor;
    public COLOR BackColor;
    public int FontSize = 96;
    public bool Modify;

    private void Update()
    {
        if (Modify)
        {
            Modify = false;
            StringToTexture(gameObject, Text, GetForeColor(), GetBackColor(), FontSize);
        }

    }
    private void StringToTexture(GameObject obj, string Text, System.Drawing.Color foreColor, System.Drawing.Color backColor, int fontSize)
    {
        if (string.IsNullOrEmpty(Text))
        {
            Text = " ";
        }
        System.Drawing.Font font = new System.Drawing.Font("Arial", fontSize, System.Drawing.FontStyle.Bold);

        System.Drawing.Graphics objGraphics = System.Drawing.Graphics.FromImage(new Bitmap(1, 1));
        SizeF size = objGraphics.MeasureString(Text, font);

        int Width = (int)size.Width;
        int Height = (int)size.Height;
        SolidBrush objBrushForeColor = new SolidBrush(foreColor);
        SolidBrush objBrushBackColor = new SolidBrush(backColor);

        Point objPoint = new Point(0, 0);

        Bitmap objBitmap = new Bitmap(Width, Height);
        objGraphics = System.Drawing.Graphics.FromImage(objBitmap);
        objGraphics.FillRectangle(objBrushBackColor, 0, 0, Width, Height);
        objGraphics.DrawString(Text, font, objBrushForeColor, objPoint);
        MemoryStream stream = new MemoryStream();
        objBitmap.Save(stream, ImageFormat.Png);
        byte[] imageData = stream.GetBuffer();
        Texture2D texture = new Texture2D(Width, Height);
        Material material = new Material(Shader.Find("Diffuse"));
        texture.LoadImage(imageData);
        material.mainTexture = texture;
        obj.GetComponent<Renderer>().sharedMaterial = material;
    }

    public System.Drawing.Color GetBackColor()
    {
        switch (BackColor)
        {
            case COLOR.BLACK: return System.Drawing.Color.Black;
            case COLOR.YELLOW: return System.Drawing.Color.Yellow;
            case COLOR.ORANGE: return System.Drawing.Color.Orange;
            case COLOR.WHITE: return System.Drawing.Color.White;
            case COLOR.BLUE: return System.Drawing.Color.Blue;
            case COLOR.GREEN: return System.Drawing.Color.Green;
            case COLOR.RED: return System.Drawing.Color.Red;
            case COLOR.GRAY: return System.Drawing.Color.Gray;
        }
        return System.Drawing.Color.Transparent;
    }

    public System.Drawing.Color GetForeColor()
    {
        switch (ForeColor)
        {
            case COLOR.BLACK: return System.Drawing.Color.Black;
            case COLOR.YELLOW: return System.Drawing.Color.Yellow;
            case COLOR.ORANGE: return System.Drawing.Color.Orange;
            case COLOR.WHITE: return System.Drawing.Color.White;
            case COLOR.BLUE: return System.Drawing.Color.Blue;
            case COLOR.GREEN: return System.Drawing.Color.Green;
            case COLOR.RED: return System.Drawing.Color.Red;
            case COLOR.GRAY: return System.Drawing.Color.Gray;
        }
        return System.Drawing.Color.Transparent;
    }

}
