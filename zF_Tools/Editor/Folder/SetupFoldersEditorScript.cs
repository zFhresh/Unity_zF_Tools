using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;

public class SetupFoldersEditorScript : EditorWindow {
    private const string ItemsKey = "SetupFoldersEditor_FolderItems";

    [MenuItem("zF Tools/Folders/Setup Folders")]
    private static void ShowWindow() {
        var window = GetWindow<SetupFoldersEditorScript>();
        window.titleContent = new GUIContent("File Create Window");

        window.minSize = new Vector2(400, 400);  // Minimum boyut
        window.maxSize = new Vector2(400, 800);  // Maksimum boyut


        window.Show();
    }
    bool ShowError = false;
    string ErrorMessage = "Folders are already exist!";
    private void OnGUI() {

        // ReorderableList'i çiz
        reorderableList.DoLayoutList();
        GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Folders")) 
            {
                if (items.Count == 0)
                {
                    ErrorMessage = "Please enter folder names or click Sample Folders!";
                    ShowError = true;
                }
                else {
                    ShowError = false;
                }
                foreach (var item in items)
                {
                    CreateFiles();
                }
            }
            if(GUILayout.Button("Sample Folders"))
            {
                items = new List<string> {"Scripts", "Scenes", "Model&Materials", "Prefabs", "SO_Datas","Sprites", "fonts", "Animations"};
                reorderableList.list = items;
                Repaint();
            }
        GUILayout.EndHorizontal();
        // ad space
        GUILayout.Space(20);

        GUILayout.Label("Save and Load your folders", EditorStyles.boldLabel);
        
        GUILayout.BeginHorizontal();
        // ad label
            
            if (GUILayout.Button("Save folders"))
            {
                SaveData();
            }
            if (GUILayout.Button("Load folders"))
            {
                LoadData();
                reorderableList.list = items;
                Repaint();
            }
        GUILayout.EndHorizontal();


        

        if (ShowError)
        {
            EditorGUILayout.HelpBox(ErrorMessage, MessageType.Warning);
        }
    }
    private ReorderableList reorderableList;
    private List<string> items;


    private void OnEnable()
    {
        items = new List<string>(); // Listeyi başlat
        reorderableList = new ReorderableList(items, typeof(string), true, true, true, true);

        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Folder Names");
        };

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            items[index] = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), items[index]);
        };

        reorderableList.onAddCallback = (ReorderableList list) =>
        {
            items.Add("New Item");
        };

        reorderableList.onRemoveCallback = (ReorderableList list) =>
        {
            items.RemoveAt(list.index);
        };
    }

    public void CreateFiles()
    {
        // Create folders
        bool isExist = AnyFileIsExist();
        if (isExist)
        {
            ShowError = true;
        }
        else {
            ShowError = false;
        }
        foreach (var item in items)
        {
            string path = "Assets/" + item;
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets", item);
            }
        }
    }

    
    public bool AnyFileIsExist() {

        foreach (var item in items)
        {
            string path = "Assets/" + item;
            if (AssetDatabase.IsValidFolder(path))
            {
                ErrorMessage = "Folders are already exist! :" + item;
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void SaveData()
    {
        // Veriyi kaydet
        EditorPrefs.SetString(ItemsKey, string.Join(",", items));
    }

    private void LoadData()
    {
        // Veriyi yükle
        string savedItems = EditorPrefs.GetString(ItemsKey, string.Empty);
        if (!string.IsNullOrEmpty(savedItems))
        {
            items = new List<string>(savedItems.Split(','));
        }
        else
        {
            items = new List<string>();
        }
    }


}