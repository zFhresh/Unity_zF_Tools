using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class ReadMeshScrpt : EditorWindow
{
    private const int TrisSafeZone = 10000;
    private const int TrisWarningZone = 50000;
    private const int TrisDangerZone = 100000;




    private GameObject selectedGameObject;
    GameObject LastSelectedGameObject;
    Editor gameObjectEditor;

    int triangleCount;
    string MeshInfoText = "";



    

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
        GUIStyle style = CreateGUIStyle(Color.white, 16, TextAnchor.MiddleCenter);
        
        GUILayout.Label("Show Triangle Count", style);
        
        GUILayout.Space(10);
        // Add white Line
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
    
        // Gameobject seçme butonu
        selectedGameObject = (GameObject)EditorGUILayout.ObjectField("Choose a GameObject", selectedGameObject, typeof(GameObject), true);
        // ObjectField is variable is changed, it will be updated in the inspector
        if(selectedGameObject != null) {
            // Get all meshfilters component in the selectedGameObject and its childrens
            MeshFilter[] meshFilters = selectedGameObject.GetComponentsInChildren<MeshFilter>();
            CalculateTriangleCount(meshFilters);
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


        GUIStyle MeshAdviceTextStyle = CreateGUIStyle(Color.white, 16, TextAnchor.MiddleCenter);
    

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


    }
    public GUIStyle CreateGUIStyle(Color color , int fontSize, TextAnchor textAnchor)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = color;
        style.fontSize = fontSize;
        style.alignment = textAnchor;
        return style;
    }


     public void CalculateTriangleCount(params MeshFilter[] filters)
 {
     if (filters != null || filters.Length == 0)
     {
         int _triangleCount = 0;

         foreach (MeshFilter meshFilter in filters)
         {
             Mesh mesh = meshFilter.sharedMesh;

             if (mesh is not null)
                 _triangleCount += mesh.triangles.Length / 3;
         }
         MeshInfoText = "Triangle Count: " + _triangleCount;
         triangleCount = _triangleCount;
     }
     else
     {
         MeshInfoText = "Mesh Not Found!";
     }
 }

}
