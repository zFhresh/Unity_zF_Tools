using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class DistanceVisualizer
{
    private static bool isVisualizationEnabled = true; // Görsel gösterimin açık olup olmadığını kontrol eden değişken

    static DistanceVisualizer()
    {
        // Scene güncellenirken callback ekleyin
        SceneView.duringSceneGui += OnSceneGUI;
    }

    [MenuItem("zF Tools/Toggle Distance Visualization %t")] // %t -> Ctrl + T (Windows) / Cmd + T (Mac)
    public static void ToggleVisualization()
    {
        isVisualizationEnabled = !isVisualizationEnabled; // Görsel gösterimin durumunu tersine çevir
        SceneView.RepaintAll(); // Scene view'i yeniden çiz
    }

    static void OnSceneGUI(SceneView sceneView)
    {
        if (!isVisualizationEnabled)
        {
            return;
        }

        // Seçili objeleri al
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length <= 1)
        {
            return;
        }

        // Ortadaki noktayı hesapla
        Vector3 center = Vector3.zero;
        foreach (GameObject obj in selectedObjects)
        {
            center += obj.transform.position;
        }
        center /= selectedObjects.Length;

        // Her bir objeye olan mesafeyi göster
        Handles.color = Color.green;
        foreach (GameObject obj in selectedObjects)
        {
            float distance = Vector3.Distance(obj.transform.position, center);

            // Çizgi çiz
            Handles.DrawLine(obj.transform.position, center);
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.black;
            // Mesafeyi etikette göster
            Handles.Label(obj.transform.position, $"{distance:F2} units", style);

        }


        // Scene'yi yeniden çiz
        SceneView.RepaintAll();
    }
}
