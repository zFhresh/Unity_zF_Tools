using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class ReadMeshScrpt : EditorWindow
{

    private GameObject selectedGameObject;
    GameObject LastSelectedGameObject;
    Editor gameObjectEditor;

    int triangleCount;
    string MeshInfoText = "";

    private const int TrisSafeZone = 10000;
    private const int TrisWarningZone = 50000;
    private const int TrisDangerZone = 100000;

    bool CalculateWithAllChilds = false;

    

   [MenuItem("zF Tools/Show Triangle Count")]
    private static void ShowWindow()
    {
        var window = GetWindow<ReadMeshScrpt>();
        window.titleContent = new GUIContent("Triangle Count");
        window.minSize = new Vector2(400, 400);
        window.maxSize = new Vector2(400, 800);
        window.Show();
    }
    public void OnGUI()
    {
        // add padding to text
        GUIStyle style = new GUIStyle();
        
        style.fontSize = 16;
        style.normal.textColor = Color.white;
        // center the text
        style.alignment = TextAnchor.MiddleCenter;
        
        GUILayout.Label("Show Triangle Count", style);
        
        GUILayout.Space(10);
        // Add white Line
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
    
        // Gameobject seçme butonu
        selectedGameObject = (GameObject)EditorGUILayout.ObjectField("Choose a GameObject", selectedGameObject, typeof(GameObject), true);
        CalculateWithAllChilds = EditorGUILayout.Toggle("Calculate with all childs", CalculateWithAllChilds);
        // ObjectField is variable is changed, it will be updated in the inspector
        if(selectedGameObject != null) {

            if (CalculateWithAllChilds) {
                CalculateTriangleCountWithAllChilds();
            } 
            else { CalculateTriangleCount(); }
        }



        GUIStyle bgColor = new GUIStyle();
        bgColor.normal.background = EditorGUIUtility.whiteTexture;

        if (selectedGameObject != null)
        {
            if (selectedGameObject != LastSelectedGameObject)
            {
                DestroyImmediate(gameObjectEditor);
                gameObjectEditor = Editor.CreateEditor(selectedGameObject);
                LastSelectedGameObject = selectedGameObject;
            }    

            if (gameObjectEditor == null)
                gameObjectEditor = Editor.CreateEditor(selectedGameObject);
            // add zoom  to the preview
        
            gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(256, 256), bgColor);
        }


        GUIStyle MeshAdviceTextStyle = new GUIStyle();
        
        MeshAdviceTextStyle.fontSize = 16;
        
        // center the text
        MeshAdviceTextStyle.alignment = TextAnchor.MiddleCenter;

        string AdviceText = "";
        if (triangleCount > 0) {
            if (triangleCount < TrisSafeZone) {
                AdviceText = "This mesh is safe to use.";
                MeshAdviceTextStyle.normal.textColor = Color.green;
            } else if (triangleCount < TrisWarningZone) {
                AdviceText = "Mesh may impact performance.";
                MeshAdviceTextStyle.normal.textColor = Color.yellow;
            } else if (triangleCount < TrisDangerZone) {
                AdviceText = "Significant optimization may be needed.";
                MeshAdviceTextStyle.normal.textColor = Color.yellow;
            } else {
                AdviceText = "Performance issues expected; major optimization needed.";
                MeshAdviceTextStyle.fontSize = 11;
                MeshAdviceTextStyle.normal.textColor = Color.red;
            }
        }


        GUILayout.Space(20);
        GUILayout.Label(MeshInfoText, style);
        GUILayout.Space(10);
        GUILayout.Label(AdviceText, MeshAdviceTextStyle);



        // if (GUILayout.Button("Show Triangle Count"))
        // {
        //     CalculateTriangleCount();
        // }
    }

    public void CalculateTriangleCount()
    {
        if (selectedGameObject == null)
        {
            Debug.LogError("GameObject seçilmedi.");
            return;
        }

        MeshFilter meshFilter = selectedGameObject.GetComponent<MeshFilter>();

        if (meshFilter != null)
        {
            Mesh mesh = meshFilter.sharedMesh;

            if (mesh != null)
            {
                int _triangleCount = mesh.triangles.Length / 3;
                MeshInfoText = "Triangle Count: " + _triangleCount;
                triangleCount = _triangleCount;
            }
            else
            {
                MeshInfoText = "Mesh Not Found!";
            }
        }
        else
        {
            
            MeshInfoText = "MeshFilter Component Not Found!";
        }
    }

    public void CalculateTriangleCountWithAllChilds() {
        Debug.Log("CalculateTriangleCountWithAllChilds");
        if (selectedGameObject == null)
        {
            Debug.LogError("GameObject seçilmedi.");
            return;
        }

        MeshFilter[] meshFilters = selectedGameObject.GetComponentsInChildren<MeshFilter>();

        if (meshFilters != null)
        {
            int _triangleCount = 0;
            foreach (MeshFilter meshFilter in meshFilters) {
                Mesh mesh = meshFilter.sharedMesh;
                if (mesh != null)
                {
                    _triangleCount += mesh.triangles.Length / 3;
                }
            }
            MeshInfoText = "Triangle Count: " + _triangleCount;
            triangleCount = _triangleCount;
        }
        else
        {
            MeshInfoText = "Mesh Not Found!";
        }
    }



    // static void ShowTriangleCount()
    // {
    //     // Kullanıcıdan bir FBX veya OBJ dosyası seçmesini istiyoruz
    //     string path = EditorUtility.OpenFilePanel("FBX veya OBJ dosyasını seçin", "Assets", "fbx,obj");

    //     if (string.IsNullOrEmpty(path))
    //     {
    //         Debug.LogError("Dosya seçilmedi.");
    //         return;
    //     }

    //     // Path'i 'Assets/' ile başlatmamız gerekiyor
    //     string relativePath = "Assets" + path.Substring(Application.dataPath.Length);

    //     // Asset olarak model dosyasını yüklüyoruz
    //     GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(relativePath);

    //     if (model == null)
    //     {
    //         Debug.LogError("Model dosyası yüklenemedi.");
    //         return;
    //     }

    //     // MeshFilter'i alıyoruz
    //     MeshFilter meshFilter = model.GetComponentInChildren<MeshFilter>();

    //     if (meshFilter != null)
    //     {
    //         Mesh mesh = meshFilter.sharedMesh;

    //         if (mesh != null)
    //         {
    //             int triangleCount = mesh.triangles.Length / 3;
    //             Debug.Log("Modelin üçgen sayısı: " + triangleCount);
    //         }
    //         else
    //         {
    //             Debug.LogError("Mesh bulunamadı!");
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogError("MeshFilter bileşeni bulunamadı!");
    //     }
    // }
}
