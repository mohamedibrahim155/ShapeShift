using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Scripts.Editor.ScriptableObjectGenerator
{
    //Scripts  that auto trigger after script compilation to check
    //if there is pending SO to create
    [InitializeOnLoad]
    public static class PendingScriptableObjectAssetCreatorService
    {

        static PendingScriptableObjectAssetCreatorService()
        {
            EditorApplication.delayCall += TryCreatePendingAsset;
        }

        private static void TryCreatePendingAsset()
        {
            if (!EditorPrefs.HasKey(SOGeneratorSettings.PendingSODataKey))
                return;

            string data =  EditorPrefs.GetString(SOGeneratorSettings.PendingSODataKey);

            PendingSOData jsonData = JsonUtility.FromJson<PendingSOData>(data);

            string className = jsonData.ClassName;
            string namespaceName = jsonData.NamespaceName;
            string assetFolder = jsonData.AssetFolder;


            string fullClassName = string.IsNullOrWhiteSpace(namespaceName)
                ? className
                : $"{namespaceName}.{className}";

            Type type = FindType(fullClassName);

            if (type == null)
            {
                Debug.LogWarning($"Type not found yet: {fullClassName}");
                return;
            }

            ScriptableObject asset = ScriptableObject.CreateInstance(type);

            string assetPath = $"{assetFolder}/{className}.asset";
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();

            EditorPrefs.DeleteKey(SOGeneratorSettings.PendingSODataKey);

            Debug.Log($"Created ScriptableObject asset: {assetPath}");
        }

        private static Type FindType(string fullClassName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(fullClassName);

                if (type != null)
                    return type;
            }

            return null;
        }
    }
}
