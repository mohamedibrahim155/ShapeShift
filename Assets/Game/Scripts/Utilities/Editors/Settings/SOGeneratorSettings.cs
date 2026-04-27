using UnityEngine;

namespace Scripts.Editor.ScriptableObjectGenerator
{
    public static class SOGeneratorSettings
    {
        /// Default values for the generator form field
        public const string defaultNamespaceName = "YourNamespace";
        public const string defaultScriptFolder = "Assets/Scripts/ScriptableObjects";
        public const string defaultAssetFolder = "Assets/Data";
        public const string defaultClassName = "NewScriptableObject";
        public const string defaultMenuName = "Scriptable Objects";

        /// EditorPrefs keys for pending asset creation after script compilation
        public const string PendingSODataKey = "PendingSOData";

        //windows settings
        public const string GeneratorWindowTitle = "Scriptable Object Creator";
        public const string TypeSearchWindowTitle = "Select Type";
        public const string TypeSearchControlName = "TypeSearchField";

        public static Color SelectedColor = new Color(0.24f, 0.48f, 0.90f, 0.45f);

        public const float RowHeight = 20f;
    }
}
