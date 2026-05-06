#if UNITY_EDITOR
using Scripts.UI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIWindow),true)]
public class UIWindowEditor : Editor
{
    private const int BUTTON_WIDTH = 150;
    private const int BUTTON_HEIGHT = 20;
    public override void OnInspectorGUI()
    {
        // Draw default inspector (your fields)
        DrawDefaultInspector();

        GUILayout.Space(10);

        UIWindow window = (UIWindow)target;

        GUI.enabled =  Application.isPlaying;

        // draws open button
        if (GUILayout.Button("Open Window", GUILayout.Width(BUTTON_WIDTH), GUILayout.Height(BUTTON_HEIGHT)))
        {
            window.Open();

            EditorUtility.SetDirty(window.gameObject);
        }

        // draws Close button
        if (GUILayout.Button("Close Window", GUILayout.Width(BUTTON_WIDTH), GUILayout.Height(BUTTON_HEIGHT)))
        {
            window.Close();

            EditorUtility.SetDirty(window.gameObject);
        }
    }
}
#endif
