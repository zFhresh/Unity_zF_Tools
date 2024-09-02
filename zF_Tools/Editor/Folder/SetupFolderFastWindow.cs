using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;

public class SetupFolderFastWindow : EditorWindow {
    [MenuItem("zF Tools/Folders/[FAST] Setup Folders")]
    private static void ShowWindow()
    {
        // Pencereyi göster
        var window = GetWindow<SetupFolderFastWindow>();
        window.titleContent = new GUIContent("Auto Run Function");
        window.Show();
    }
    List<string> items;
    private void OnEnable()
    {
        // Pencere açıldığında çalıştırmak istediğin fonksiyon
        RunFunction();
    }

    private void RunFunction()
    {
        // Buraya istediğin fonksiyonu yaz
        items = new List<string>  {"Scripts", "Scenes", "Model&Materials", "Prefabs", "SO_Datas","Sprites", "fonts", "Animations"};
        foreach (var item in items)
        {
            CreateFiles();
        }
        Debug.Log("All folders created!");
    }
    public void CreateFiles()
    {

        foreach (var item in items)
        {
            string path = "Assets/" + item;
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets", item);
            }
        }
    }

    private void OnGUI()
    {
        // OnGUI'de pencereyi kapat
        Close();
    }
}