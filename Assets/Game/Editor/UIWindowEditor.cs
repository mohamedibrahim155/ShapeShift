#if UNITY_EDITOR
using Scripts.UI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIWindow),true)]
public class UIWindowEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector (your fields)
        DrawDefaultInspector();

        GUILayout.Space(10);

        UIWindow window = (UIWindow)target;

        GUI.enabled =  Application.isPlaying;
        if (GUILayout.Button("Open Window"))
        {
            window.Open();

            EditorUtility.SetDirty(window.gameObject);
        }

        if (GUILayout.Button("Close Window"))
        {
            window.Close();

            EditorUtility.SetDirty(window.gameObject);
        }
    }
}
#endif
