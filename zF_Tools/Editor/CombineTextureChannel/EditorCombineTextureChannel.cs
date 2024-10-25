using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class EditorCombineTextureChannel : EditorWindow
{

    ComputeShader CombineTextureShader;
    const float ChannelImageSize = 75;
    Texture2D RedChannel;
    Texture2D GreenChannel;
    Texture2D BlueChannel;
    Texture2D AlphaChannel;
    bool UseCPU;
    
    [MenuItem("zF Tools/Combine Texture Channel")]
    private static void ShowWindow() {
        var window = GetWindow<EditorCombineTextureChannel>();
        window.titleContent = new GUIContent("Combine Texture Channels");

        window.minSize = new Vector2(420, 150);  // Minimum boyut
        window.maxSize = new Vector2(420, 150);  // Maksimum boyut


        window.Show();
    }
     private void OnGUI() {
        
        // load asset from resources
        if (CombineTextureShader == null) {
            CombineTextureShader = Resources.Load<ComputeShader>("CombineChannels");
        }

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        
        GUILayout.FlexibleSpace();
        RedChannel = (Texture2D) EditorGUILayout.ObjectField("", RedChannel, typeof (Texture2D), false, GUILayout.Width(ChannelImageSize), GUILayout.Height(ChannelImageSize));

        GUILayout.FlexibleSpace();
        GreenChannel = (Texture2D) EditorGUILayout.ObjectField("", GreenChannel, typeof (Texture2D), false, GUILayout.Width(ChannelImageSize), GUILayout.Height(ChannelImageSize));

        GUILayout.FlexibleSpace();
        BlueChannel = (Texture2D) EditorGUILayout.ObjectField("", BlueChannel, typeof (Texture2D), false, GUILayout.Width(ChannelImageSize), GUILayout.Height(ChannelImageSize));

        GUILayout.FlexibleSpace();
        AlphaChannel = (Texture2D) EditorGUILayout.ObjectField("", AlphaChannel, typeof (Texture2D), false, GUILayout.Width(ChannelImageSize), GUILayout.Height(ChannelImageSize));




            //GUILayout.Label(ChannelName, style, GUILayout.Width(200));
            
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
        if (RedChannel != null && GreenChannel != null && BlueChannel != null && AlphaChannel != null) {
            UseCPU = GUILayout.Toggle(UseCPU, "Use CPU", GUILayout.Width(100), GUILayout.Height(30), GUILayout.ExpandWidth(false));
            


            // if texture sizes are not equal, show error message

            if (RedChannel.width != GreenChannel.width || RedChannel.height != GreenChannel.height ||
                GreenChannel.width != BlueChannel.width || GreenChannel.height != BlueChannel.height ||
                BlueChannel.width != AlphaChannel.width || BlueChannel.height != AlphaChannel.height) {
                    // textures resulotions are not equal if combine them, something can be wrong
                GUILayout.Label("texture resolution are not equal if you combine, something can be wrong.", style, GUILayout.Width(400));
            }
            else {
                // Create label and say "if you are get error, Use CPU"
                GUILayout.Label("If you are get error, Use CPU", GUILayout.Width(200));
            }






            if (GUILayout.Button("Combine")) {
                if (UseCPU) {
                    CombineChannels();
                }
                else {
                    CombineWithCompute();
                }

         }

    }
}
    private void CombineWithCompute() {
        Debug.Log("Combine with Compute");
        CombineTextureShader.SetTexture(0, "RedChannelTex", RedChannel);
        CombineTextureShader.SetTexture(0, "GreenChannelTex", GreenChannel);
        CombineTextureShader.SetTexture(0, "BlueChannelTex", BlueChannel);
        CombineTextureShader.SetTexture(0, "AlphaChannelTex", AlphaChannel);

        int kernel = CombineTextureShader.FindKernel("CSMain");

        int width = RedChannel.width;
        int height = RedChannel.height;

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

        if (!string.IsNullOrEmpty(path)) {
            byte[] bytes = newTexture.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
            Debug.Log("Texture saved to: " + path);

            AssetDatabase.Refresh();
        }
    }

    private void CombineChannels()
    {
        Debug.Log("Combine with CPU");
        // Tüm texture'lerin aynı boyutta olup olmadığını kontrol et
        if (RedChannel.width != GreenChannel.width || RedChannel.height != GreenChannel.height ||
            GreenChannel.width != BlueChannel.width || GreenChannel.height != BlueChannel.height ||
            BlueChannel.width != AlphaChannel.width || BlueChannel.height != AlphaChannel.height)
        {
            Debug.LogError("All textures must have the same dimensions to combine them.");
            return;
        }

        // Yeni bir Texture2D oluştur (aynı boyutta)
        Texture2D newTexture = new Texture2D(RedChannel.width, RedChannel.height, TextureFormat.RGBA32, false);

        // Tüm pikselleri karıştır
        for (int y = 0; y < RedChannel.height; y++)
        {
            for (int x = 0; x < RedChannel.width; x++)
            {
                // Her kanaldan piksel bilgilerini al
                float r = RedChannel.GetPixel(x, y).r;     // Red kanalındaki değer
                float g = GreenChannel.GetPixel(x, y).r;   // Green kanalındaki değer
                float b = BlueChannel.GetPixel(x, y).r;    // Blue kanalındaki değer
                float a = AlphaChannel.GetPixel(x, y).r;   // Alpha kanalındaki değer

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
            // Texture'u PNG formatında kaydet
            byte[] bytes = newTexture.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
            Debug.Log("Texture saved to: " + path);

            // Asset Database'i yenileyin ki Unity texture'u görsün
            AssetDatabase.Refresh();
        }
    }
}
