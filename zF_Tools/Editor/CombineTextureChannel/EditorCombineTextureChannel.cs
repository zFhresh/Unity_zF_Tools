using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class EditorCombineTextureChannel : EditorWindow
{

    const float ChannelImageSize = 75;
    const int FontSize = 20;
    Texture2D RedChannel;
    Texture2D GreenChannel;
    Texture2D BlueChannel;
    Texture2D AlphaChannel;
    [MenuItem("zF Tools/Combine Texture Channel")]
    private static void ShowWindow() {
        var window = GetWindow<EditorCombineTextureChannel>();
        window.titleContent = new GUIContent("Combine Texture Channels");

        window.minSize = new Vector2(400, 400);  // Minimum boyut
        window.maxSize = new Vector2(400, 800);  // Maksimum boyut


        window.Show();
    }
     private void OnGUI() {
        GUIStyle style = new GUIStyle();
        style.fontSize = FontSize;
        style.normal.textColor = Color.white;

        
        for (int i = 0; i < 4; i++) {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            string ChannelName = "";
            switch (i) {
                case 0:
                    RedChannel = (Texture2D) EditorGUILayout.ObjectField("", RedChannel, typeof (Texture2D), false, GUILayout.Width(ChannelImageSize), GUILayout.Height(ChannelImageSize));
                    ChannelName = "Red Channel";
                    break;
                case 1:
                    GreenChannel = (Texture2D) EditorGUILayout.ObjectField("", GreenChannel, typeof (Texture2D), false, GUILayout.Width(ChannelImageSize), GUILayout.Height(ChannelImageSize));
                    ChannelName = "Green Channel";
                    break;
                case 2:
                    BlueChannel = (Texture2D) EditorGUILayout.ObjectField("", BlueChannel, typeof (Texture2D), false, GUILayout.Width(ChannelImageSize), GUILayout.Height(ChannelImageSize));
                    ChannelName = "Blue Channel";
                    break;
                case 3:
                    AlphaChannel = (Texture2D) EditorGUILayout.ObjectField("", AlphaChannel, typeof (Texture2D), false, GUILayout.Width(ChannelImageSize), GUILayout.Height(ChannelImageSize));
                    ChannelName = "Alpha Channel";
                    break;
            }


            GUILayout.Label(ChannelName, style, GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();
        }

        if (RedChannel != null && GreenChannel != null && BlueChannel != null && AlphaChannel != null) {
            if (GUILayout.Button("Combine")) {
                CombineChannels();

         }

    }
}

    private void CombineChannels()
    {
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
