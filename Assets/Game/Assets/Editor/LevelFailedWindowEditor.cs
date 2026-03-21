#if UNITY_EDITOR
using Scripts.UI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelFailedWindow))]
public class LevelFailedWindowEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector (your fields)
        DrawDefaultInspector();

        GUILayout.Space(10);

        LevelFailedWindow script = (LevelFailedWindow)target;

        GUI.enabled =  Application.isPlaying;
        if (GUILayout.Button("Open Window"))
        {
            script.OpenLevelFailedWindow();
        }

        if (GUILayout.Button("Close Window"))
        {
            script.Close();
        }
    }
}
#endif
