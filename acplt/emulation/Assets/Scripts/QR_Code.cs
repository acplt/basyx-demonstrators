using UnityEngine;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

using System;

public static class QR_Code
{
    public static Color32[] color32;
    public static int Size;
    private static Texture2D encoded;
    private static Material material;

    public static Material Encode(string text)
    {
        encoded = new Texture2D(Size, Size);
        color32 = Generate(text, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();

        Material material = new Material(Shader.Find("Diffuse"));
        material.mainTexture = encoded;
        return material;
    }

    private static Color32[] Generate(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width,
                ErrorCorrection = ErrorCorrectionLevel.H
            }
        };
        return writer.Write(textForEncoding);
    }

    public static string Decode(Color32[] pixels, int width, int height)
    {
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            var result = barcodeReader.Decode(pixels, width, height);
            if (result != null)
            {
                return result.Text;
            }
            return "";
        }
        catch (Exception ex)
        {
            return "";
        }
    }
}
