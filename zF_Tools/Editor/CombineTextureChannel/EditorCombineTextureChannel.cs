using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class EditorCombineTextureChannel : EditorWindow
{
    #region  RBGA Channel Options
    private enum ChannelOption
    {
        R = 0,
        G = 1,
        B = 2,
        A = 3
    }

    #endregion

    private ChannelOption Image_1_ChannelOption = ChannelOption.R;
    private ChannelOption Image_2_ChannelOption = ChannelOption.R;
    private ChannelOption Image_3_ChannelOption = ChannelOption.R;
    private ChannelOption Image_4_ChannelOption = ChannelOption.R;



    ComputeShader CombineTextureShader;
    const float ChannelImageSize = 75;
    Texture2D Image1;
    Texture2D Image2;
    Texture2D Image3;
    Texture2D Image4;
    bool UseCPU;

    [MenuItem("zF Tools/Combine Texture Channel")]
    private static void ShowWindow()
    {
        var window = GetWindow<EditorCombineTextureChannel>();
        window.titleContent = new GUIContent("Combine Texture Channels");

        window.minSize = new Vector2(420, 190);  // Minimum boyut
        window.maxSize = new Vector2(420, 190);  // Maksimum boyut


        window.Show();
    }
    private void OnGUI()
    {

        // load asset from resources
        if (CombineTextureShader == null)
        {
            CombineTextureShader = Resources.Load<ComputeShader>("CombineChannels");
        }

        GUIStyle RGBATextStyle = new GUIStyle();
        RGBATextStyle.alignment = TextAnchor.MiddleCenter;
        RGBATextStyle.fontStyle = FontStyle.Bold;
        RGBATextStyle.fontSize = 15;
        RGBATextStyle.normal.textColor = Color.white;

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;

        // R G B A Options
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUILayout.FlexibleSpace();
        //Label
        GUILayout.Label("R", RGBATextStyle, GUILayout.Width(100));
        GUILayout.FlexibleSpace();
        GUILayout.Label("G", RGBATextStyle, GUILayout.Width(100));

        GUILayout.FlexibleSpace();
        GUILayout.Label("B", RGBATextStyle, GUILayout.Width(100));

        GUILayout.FlexibleSpace();
        GUILayout.Label("A", RGBATextStyle, GUILayout.Width(100));
        //GUILayout.Label(ChannelName, style, GUILayout.Width(200));

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUILayout.FlexibleSpace();
        Image1 = (Texture2D)EditorGUILayout.ObjectField("", Image1, typeof(Texture2D), false, GUILayout.Width(ChannelImageSize), GUILayout.Height(ChannelImageSize));

        GUILayout.FlexibleSpace();
        Image2 = (Texture2D)EditorGUILayout.ObjectField("", Image2, typeof(Texture2D), false, GUILayout.Width(ChannelImageSize), GUILayout.Height(ChannelImageSize));

        GUILayout.FlexibleSpace();
        Image3 = (Texture2D)EditorGUILayout.ObjectField("", Image3, typeof(Texture2D), false, GUILayout.Width(ChannelImageSize), GUILayout.Height(ChannelImageSize));

        GUILayout.FlexibleSpace();
        Image4 = (Texture2D)EditorGUILayout.ObjectField("", Image4, typeof(Texture2D), false, GUILayout.Width(ChannelImageSize), GUILayout.Height(ChannelImageSize));

        //GUILayout.Label(ChannelName, style, GUILayout.Width(200));

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUILayout.FlexibleSpace();
        Image_1_ChannelOption = (ChannelOption)EditorGUILayout.EnumPopup(Image_1_ChannelOption, GUILayout.Width(75));

        GUILayout.FlexibleSpace();
        Image_2_ChannelOption = (ChannelOption)EditorGUILayout.EnumPopup(Image_2_ChannelOption, GUILayout.Width(75));
        GUILayout.FlexibleSpace();
        Image_3_ChannelOption = (ChannelOption)EditorGUILayout.EnumPopup(Image_3_ChannelOption, GUILayout.Width(75));
        GUILayout.FlexibleSpace();
        Image_4_ChannelOption = (ChannelOption)EditorGUILayout.EnumPopup(Image_4_ChannelOption, GUILayout.Width(75));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
        if (Image1 != null && Image2 != null && Image3 != null && Image4 != null)
        {
            UseCPU = GUILayout.Toggle(UseCPU, "Use CPU", GUILayout.Width(100), GUILayout.Height(30), GUILayout.ExpandWidth(false));



            // if texture sizes are not equal, show error message

            if (Image1.width != Image2.width || Image1.height != Image2.height ||
                Image2.width != Image3.width || Image2.height != Image3.height ||
                Image3.width != Image4.width || Image3.height != Image4.height)
            {
                // textures resulotions are not equal if combine them, something can be wrong
                GUILayout.Label("texture resolution are not equal if you combine, something can be wrong.", style, GUILayout.Width(400));
            }
            else
            {
                // Create label and say "if you are get error, Use CPU"
                GUILayout.Label("If you are get error, Use CPU", GUILayout.Width(200));
            }






            if (GUILayout.Button("Combine"))
            {
                if (UseCPU)
                {
                    CombineChannels();
                }
                else
                {
                    CombineWithCompute();
                }

            }

        }
    }
    private void CombineWithCompute()
    {
        Debug.Log("Combine with Compute");
        CombineTextureShader.SetTexture(0, "Image1", Image1);
        CombineTextureShader.SetTexture(0, "Image2", Image2);
        CombineTextureShader.SetTexture(0, "Image3", Image3);
        CombineTextureShader.SetTexture(0, "Image4", Image4);

        CombineTextureShader.SetInt("Image_1_ChannelOption", (int)Image_1_ChannelOption);
        CombineTextureShader.SetInt("Image_2_ChannelOption", (int)Image_2_ChannelOption);
        CombineTextureShader.SetInt("Image_3_ChannelOption", (int)Image_3_ChannelOption);
        CombineTextureShader.SetInt("Image_4_ChannelOption", (int)Image_4_ChannelOption);


        int kernel = CombineTextureShader.FindKernel("CSMain");

        int width = Image1.width;
        int height = Image1.height;

        RenderTexture result = new RenderTexture(width, height, 0);
        result.enableRandomWrite = true;
        result.Create();

        CombineTextureShader.SetTexture(kernel, "Result", result);
        CombineTextureShader.Dispatch(kernel, width / 8, height / 8, 1);

        RenderTexture.active = result;
        Texture2D newTexture = new Texture2D(width, height);
        newTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        newTexture.Apply();

        RenderTexture.active = null;
        result.Release();

        string path = EditorUtility.SaveFilePanel("Save Combined Texture", Application.dataPath, "CombinedTexture.png", "png");

        if (!string.IsNullOrEmpty(path))
        {
            SaveTextureAsPNG(newTexture, path);
        }
        // if (!string.IsNullOrEmpty(path)) {
        //     byte[] bytes = newTexture.EncodeToPNG();
        //     System.IO.File.WriteAllBytes(path, bytes);
        //     Debug.Log("Texture saved to: " + path);

        //     AssetDatabase.Refresh();
        // }
    }

    private void CombineChannels()
    {
        Debug.Log("Combine with CPU");
        // Tüm texture'lerin aynı boyutta olup olmadığını kontrol et
        if (Image1.width != Image2.width || Image1.height != Image2.height ||
            Image2.width != Image3.width || Image2.height != Image3.height ||
            Image3.width != Image4.width || Image3.height != Image4.height)
        {
            Debug.LogError("All textures must have the same dimensions to combine them.");
            return;
        }

        // Yeni bir Texture2D oluştur (aynı boyutta)
        Texture2D newTexture = new Texture2D(Image1.width, Image1.height, TextureFormat.RGBA32, false);

        // Tüm pikselleri karıştır
        for (int y = 0; y < Image1.height; y++)
        {
            for (int x = 0; x < Image1.width; x++)
            {
                // Her kanaldan piksel bilgilerini al
                float r = 0; //= RedChannel.GetPixel(x, y).r;     // Red kanalındaki değer
                if (Image_1_ChannelOption == ChannelOption.R)
                {
                    r = Image1.GetPixel(x, y).r;
                }
                else if (Image_1_ChannelOption == ChannelOption.G)
                {
                    r = Image1.GetPixel(x, y).g;
                }
                else if (Image_1_ChannelOption == ChannelOption.B)
                {
                    r = Image1.GetPixel(x, y).b;
                }
                else if (Image_1_ChannelOption == ChannelOption.A)
                {
                    r = Image1.GetPixel(x, y).a;
                }


                float g = 0;// = GreenChannel.GetPixel(x, y).r;   // Green kanalındaki değer
                if (Image_2_ChannelOption == ChannelOption.R)
                {
                    g = Image2.GetPixel(x, y).r;
                }
                else if (Image_2_ChannelOption == ChannelOption.G)
                {
                    g = Image2.GetPixel(x, y).g;
                }
                else if (Image_2_ChannelOption == ChannelOption.B)
                {
                    g = Image2.GetPixel(x, y).b;
                }
                else if (Image_2_ChannelOption == ChannelOption.A)
                {
                    g = Image2.GetPixel(x, y).a;
                }


                float b = 0;// = BlueChannel.GetPixel(x, y).r;    // Blue kanalındaki değer
                if (Image_3_ChannelOption == ChannelOption.R)
                {
                    b = Image3.GetPixel(x, y).r;
                }
                else if (Image_3_ChannelOption == ChannelOption.G)
                {
                    b = Image3.GetPixel(x, y).g;
                }
                else if (Image_3_ChannelOption == ChannelOption.B)
                {
                    b = Image3.GetPixel(x, y).b;
                }
                else if (Image_3_ChannelOption == ChannelOption.A)
                {
                    b = Image3.GetPixel(x, y).a;
                }


                float a = 0;// = AlphaChannel.GetPixel(x, y).r;   // Alpha kanalındaki değer

                if (Image_4_ChannelOption == ChannelOption.R)
                {
                    a = Image4.GetPixel(x, y).r;
                }
                else if (Image_4_ChannelOption == ChannelOption.G)
                {
                    a = Image4.GetPixel(x, y).g;
                }
                else if (Image_4_ChannelOption == ChannelOption.B)
                {
                    a = Image4.GetPixel(x, y).b;
                }
                else if (Image_4_ChannelOption == ChannelOption.A)
                {
                    a = Image4.GetPixel(x, y).a;
                }
                // Yeni renk oluştur ve piksele ata
                Color newColor = new Color(r, g, b, a);
                newTexture.SetPixel(x, y, newColor);
            }
        }

        // Değişiklikleri uygula
        newTexture.Apply();

        // Yeni texture'ü bir dosya olarak kaydetmek için path seçtir
        string path = EditorUtility.SaveFilePanel("Save Combined Texture", Application.dataPath, "CombinedTexture.png", "png");


        if (!string.IsNullOrEmpty(path))
        {
            SaveTextureAsPNG(newTexture, path);
        }


        // if (!string.IsNullOrEmpty(path))
        // {
        //     // Texture'u PNG formatında kaydet
        //     byte[] bytes = newTexture.EncodeToPNG();
        //     System.IO.File.WriteAllBytes(path, bytes);
        //     Debug.Log("Texture saved to: " + path);

        //     // Asset Database'i yenileyin ki Unity texture'u görsün
        //     AssetDatabase.Refresh();
        // }
    }

    public void SaveTextureAsPNG(Texture2D _texture, string _path)
    {
        byte[] _bytes = _texture.EncodeToPNG();
        File.WriteAllBytes(_path, _bytes);
        AssetDatabase.Refresh();
    }


}