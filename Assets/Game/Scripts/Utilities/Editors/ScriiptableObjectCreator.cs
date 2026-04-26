using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
public class ScriiptableObjectCreator : EditorWindow
{
    private string scriptableObjectName = "NewScriptableObject";
    private string namespaceName = "YourNamespace";
    private string scriptFolder = "Assets/Scripts/ScriptableObjects";
    private string assetFolder = "Assets/Data";
    private string className = "NewScriptableObject";
    private string menuName = "Scriptable Objects";
    private Vector2 scrollPosition;
    private FieldDefinition[] fieldDefinitions = new FieldDefinition[0];
    private FieldDefinition copiedField;

    private static ScriiptableObjectCreator currentWindow;

    [MenuItem("Tools/Scriptable Object Generator")]
    public static void Open()
    {
        currentWindow = GetWindow<ScriiptableObjectCreator>("SO Generator");
    }

 


    private void OnGUI()
    {
        GUILayout.Label("Create ScriptableObject Script + Asset", EditorStyles.boldLabel);

        className = EditorGUILayout.TextField("Class Name", className);
        namespaceName = EditorGUILayout.TextField("Namespace", namespaceName);

        scriptFolder = EditorGUILayout.TextField("Script Folder", scriptFolder);
        assetFolder = EditorGUILayout.TextField("Asset Folder", assetFolder);
        GUILayout.Space(10);

        DrawFieldHeader();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawFieldDefinitions();

        EditorGUILayout.EndScrollView();

        GUILayout.FlexibleSpace();

        DrawBottomButtons();
    }


    private string GetFieldType(FieldDefinition sOFieldDefinition)
    {

        string fieldType = ConvertFieldType(sOFieldDefinition.fieldType, ConvertTypeToName(sOFieldDefinition.customClassType));
        string containerType = sOFieldDefinition.collectionType switch
        {
            SOFieldCollectionType.List => $"List<{fieldType}>",
            SOFieldCollectionType.Array => $"{fieldType}[]",
            SOFieldCollectionType.Queue => $"Queue<{fieldType}>",
            SOFieldCollectionType.Stack => $"Stack<{fieldType}>",
            _ => fieldType
        };

        string accessModifier = ConvertAccessModifier(sOFieldDefinition.fieldAccessModifier);
        string fieldDeclaration = $"{accessModifier} {containerType} {sOFieldDefinition.fieldName};";

        return fieldDeclaration;


    }

    public string ConvertTypeToName(Type type)
    {
        if (type == null)
            return "";

        if (type == typeof(int)) return "int";
        if (type == typeof(float)) return "float";
        if (type == typeof(string)) return "string";
        if (type == typeof(bool)) return "bool";

        return type.Name;
    }

    private string ConvertFieldType(SOFieldType type, string customTypeName)
    {
        string fieldType = type switch
        {
            SOFieldType.String => "string",
            SOFieldType.Int => "int",
            SOFieldType.Float => "float",
            SOFieldType.Bool => "bool",
            SOFieldType.Vector2 => "Vector2",
            SOFieldType.Vector3 => "Vector3",
            SOFieldType.Color => "Color",
            SOFieldType.Enum => "Enum",
            SOFieldType.CustomClass => customTypeName,
            SOFieldType.GameObject => "GameObject",
            SOFieldType.MonoBehaviour => "MonoBehaviour",
            SOFieldType.Transform => "Transform",
            SOFieldType.Component => "Component",
            SOFieldType.Sprite => "Sprite",
            SOFieldType.Material => "Material",
            SOFieldType.AudioClip => "AudioClip",
            _ => "string"


        };

        return fieldType;
    }

    private string ConvertAccessModifier(FieldAccessModifier accessModifier)
    {
        return accessModifier switch
        {
            FieldAccessModifier.Public => "public",
            FieldAccessModifier.Private => "private",
            FieldAccessModifier.Protected => "protected",
            FieldAccessModifier.Internal => "internal",
            _ => "public"
        };
    }

    private bool IsValidIdentifier(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;

        // First character must be letter or underscore
        if (!(char.IsLetter(name[0]) || name[0] == '_'))
            return false;

        // Remaining characters: letter, digit, underscore
        for (int i = 1; i < name.Length; i++)
        {
            if (!(char.IsLetterOrDigit(name[i]) || name[i] == '_'))
                return false;
        }

        return true;
    }
    private FieldValidationResult ValidateField(FieldDefinition fieldDef)
    {
        if (string.IsNullOrWhiteSpace(fieldDef.fieldName))
        {
            return new FieldValidationResult
            {
                Status = FieldValidationStatus.Error,
                Message = "Field name is empty."
            };
        }

        if (!IsValidIdentifier(fieldDef.fieldName))
        {
            return new FieldValidationResult
            {
                Status = FieldValidationStatus.Error,
                Message = "Invalid C# field name."
            };
        }

        if (fieldDef.fieldType == SOFieldType.CustomClass && fieldDef.customClassType == null)
        {
            return new FieldValidationResult
            {
                Status = FieldValidationStatus.Error,
                Message = "Custom class type is missing."
            };
        }

        string preview = GetFieldType(fieldDef);

        if (string.IsNullOrWhiteSpace(preview))
        {
            return new FieldValidationResult
            {
                Status = FieldValidationStatus.Error,
                Message = "Preview is empty."
            };
        }

        if (!preview.TrimEnd().EndsWith(";"))
        {
            return new FieldValidationResult
            {
                Status = FieldValidationStatus.Error,
                Message = "Field declaration must end with semicolon."
            };
        }

        return new FieldValidationResult
        {
            Status = FieldValidationStatus.Valid,
            Message = "Valid field."
        };
    }

    private void DrawFieldHeader()
    {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Field Definitions", EditorStyles.boldLabel);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Add Field", GUILayout.Width(100)))
        {
            AddFieldDefinition();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawFieldDefinitions()
    {
        for (int i = 0; i < fieldDefinitions.Length; i++)
        {
            FieldDefinition fieldDef = fieldDefinitions[i];

            Rect nodeRect = EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();

            fieldDef.isExpanded = EditorGUILayout.Foldout(
                fieldDef.isExpanded,
                string.IsNullOrEmpty(fieldDef.fieldName) ? $"Field {i + 1}" : GetFieldType(fieldDef),
                true
            );

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("x", GUILayout.Width(25)))
            {
                RemoveFieldDefinition(i);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break;
            }

            EditorGUILayout.EndHorizontal();

            if (fieldDef.isExpanded)
            {
                EditorGUI.indentLevel++;

                fieldDef.fieldName = EditorGUILayout.TextField("Field Name", fieldDef.fieldName);
               

                fieldDef.fieldType = (SOFieldType)EditorGUILayout.EnumPopup(
                    "Field Type",
                    fieldDef.fieldType
                );

                fieldDef.fieldAccessModifier = (FieldAccessModifier)EditorGUILayout.EnumPopup(
                    "Field Access Modifier",
                    fieldDef.fieldAccessModifier
                );

                if (fieldDef.fieldType == SOFieldType.CustomClass)
                {
                    fieldDef.isCustomClass = true;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Custom Class Name" );

              

                    string typeName = fieldDef.customClassType != null
                        ? fieldDef.customClassType.Name
                        : "Select Type";

                    if (GUILayout.Button(typeName, EditorStyles.popup))
                    {
                        Rect buttonRect = GUILayoutUtility.GetLastRect();

                        Vector2 screenPosition = GUIUtility.GUIToScreenPoint(
                            new Vector2(buttonRect.x, buttonRect.yMax)
                        );

                        TypeSearchPopUpEditor.ShowWindow((selectedType) =>
                        {
                            fieldDef.customClassType = selectedType;
                            Repaint();
                        }, screenPosition);
                    }

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    fieldDef.isCustomClass = false;
                    TypeSearchPopUpEditor.CloseWindow();
                }

                fieldDef.collectionType = (SOFieldCollectionType)EditorGUILayout.EnumPopup(
                    "Collection Type",
                    fieldDef.collectionType
                );

                EditorGUILayout.Space(4);

                // Right-click context menu for field options
                if (Event.current.type == EventType.ContextClick &&
                   nodeRect.Contains(Event.current.mousePosition))
                {
                    ShowFieldContextMenu(i);
                    Event.current.Use();
                }

                //EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
                //EditorGUILayout.HelpBox(GetFieldType(fieldDef), MessageType.None);


                FieldValidationResult validation = ValidateField(fieldDef);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel, GUILayout.Width(80));

                string icon = validation.IsValid ? "✅" : "❌";
                GUILayout.Label(icon, GUILayout.Width(25));

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.HelpBox(GetFieldType(fieldDef), MessageType.None);

                if (!validation.IsValid)
                {
                    EditorGUILayout.HelpBox(validation.Message, MessageType.Error);
                }


                EditorGUI.indentLevel--;
            }

           



            EditorGUILayout.EndVertical();
        }
    }
    private void DrawBottomButtons()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Create ScriptableObject", GUILayout.Height(35), GUILayout.Width(220)))
        {
             CreateScriptableObjectScript();
        }

        GUILayout.FlexibleSpace();

        EditorGUILayout.EndHorizontal();
    }

    private void ShowFieldContextMenu(int index)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Copy"), false, () =>
        {
            copiedField = fieldDefinitions[index].Clone();
        });

        menu.AddItem(new GUIContent("Paste"), false, () =>
        {
            if (copiedField != null)
            {
                fieldDefinitions[index] = copiedField.Clone();
            }
        });

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("Reset"), false, () =>
        {
            fieldDefinitions[index] = new FieldDefinition();
        });

        menu.ShowAsContext();
    }

    private void AddFieldDefinition()
    {
        ArrayUtility.Add(ref fieldDefinitions, new FieldDefinition());
    }

    private void RemoveFieldDefinition(int index)
    {
        if (index >= 0 && index < fieldDefinitions.Length)
        {
            ArrayUtility.RemoveAt(ref fieldDefinitions, index);
        }

    }



 

    private string GenerateScriptContent(string className, string menuName, string namespaceName)
    {
        string usingStatements = GenerateUsings();
        string fields = GenerateFields();

        if (string.IsNullOrWhiteSpace(namespaceName))
        {
            return
    $@"{usingStatements}

[CreateAssetMenu(fileName = ""{className}"", menuName = ""{menuName}/{className}"")]
public class {className} : ScriptableObject
{{
{fields}
}}";
        }

        return
    $@"{usingStatements}

namespace {namespaceName}
{{
    [CreateAssetMenu(fileName = ""{className}"", menuName = ""{menuName}/{className}"")]
    public class {className} : ScriptableObject
    {{
{fields}
    }}
}}";
    }

    private string GenerateUsings()
    {
        HashSet<string> usings = new HashSet<string>();

        usings.Add("using UnityEngine;");

        foreach (var field in fieldDefinitions)
        {
            string typeName = ConvertTypeToName(field.customClassType);

            if (string.IsNullOrEmpty(typeName))
                continue;

            if ( field.collectionType != SOFieldCollectionType.None)
                usings.Add("using System.Collections.Generic;");

            if (field.customClassType != null && !string.IsNullOrEmpty(field.customClassType.Namespace))
            {
                usings.Add($"using {field.customClassType.Namespace};");
            }
        }

        return string.Join("\n", usings);
    }

    private string GenerateFields()
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();

        foreach (var field in fieldDefinitions)
        {
            if (!ValidateField(field).IsValid)
                continue; // skip invalid fields

            builder.AppendLine($"        {GetFieldType(field)}");
        }

        if (builder.Length == 0)
        {
            builder.AppendLine("    // No fields defined");
        }

        return builder.ToString();
    }

    private bool IsValidNamespace(string namespaceName)
    {
        string[] parts = namespaceName.Split('.');

        foreach (string part in parts)
        {
            if (!IsValidIdentifier(part))
                return false;
        }

        return true;
    }
    private bool ValidateBeforeCreate()
    {
        if (string.IsNullOrWhiteSpace(className))
        {
            EditorUtility.DisplayDialog("Validation Error", "Class name is empty.", "OK");
            return false;
        }

        if (!IsValidIdentifier(className))
        {
            EditorUtility.DisplayDialog("Validation Error", "Class name is not a valid C# class name.", "OK");
            return false;
        }

        if (!string.IsNullOrWhiteSpace(namespaceName) && !IsValidNamespace(namespaceName))
        {
            EditorUtility.DisplayDialog("Validation Error", "Namespace is invalid.", "OK");
            return false;
        }

        if (fieldDefinitions == null || fieldDefinitions.Length == 0)
        {
            bool createWithoutFields = EditorUtility.DisplayDialog(
                "No Fields",
                "No fields were added. Do you still want to create the ScriptableObject?",
                "Create",
                "Cancel"
            );

            if (!createWithoutFields)
                return false;
        }

        HashSet<string> fieldNames = new HashSet<string>();

        foreach (FieldDefinition field in fieldDefinitions)
        {
            FieldValidationResult result = ValidateField(field);

            if (!result.IsValid)
            {
                EditorUtility.DisplayDialog(
                    "Validation Error",
                    $"Field '{field.fieldName}' is invalid.\n\n{result.Message}",
                    "OK"
                );

                return false;
            }

            if (!fieldNames.Add(field.fieldName))
            {
                EditorUtility.DisplayDialog(
                    "Validation Error",
                    $"Duplicate field name found: {field.fieldName}",
                    "OK"
                );

                return false;
            }
        }

        if (!AssetDatabase.IsValidFolder(scriptFolder))
        {
            EditorUtility.DisplayDialog("Validation Error", "Script folder path is invalid.", "OK");
            return false;
        }

        if (!AssetDatabase.IsValidFolder(assetFolder))
        {
            EditorUtility.DisplayDialog("Validation Error", "Asset folder path is invalid.", "OK");
            return false;
        }

        string scriptPath = $"{scriptFolder}/{className}.cs";

        if (System.IO.File.Exists(scriptPath))
        {
            EditorUtility.DisplayDialog(
                "Validation Error",
                $"A script with this class name already exists:\n{scriptPath}",
                "OK"
            );

            return false;
        }

        return true;
    }

    private static void CreateFolderIfNeeded(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
            return;

        string[] folders = folderPath.Split('/');
        string currentPath = folders[0];

        for (int i = 1; i < folders.Length; i++)
        {
            string nextPath = $"{currentPath}/{folders[i]}";

            if (!AssetDatabase.IsValidFolder(nextPath))
            {
                AssetDatabase.CreateFolder(currentPath, folders[i]);
            }

            currentPath = nextPath;
        }
    }

    private const string PendingClassKey = "PendingSOClassName";
    private const string PendingNamespaceKey = "PendingSONamespace";
    private const string PendingAssetFolderKey = "PendingSOAssetFolder";
    private void CreateScriptableObjectScript()
    {
        if (!ValidateBeforeCreate())
            return;

        CreateFolderIfNeeded(scriptFolder);
        CreateFolderIfNeeded(assetFolder);

        string scriptPath = $"{scriptFolder}/{className}.cs";

        if (System.IO.File.Exists(scriptPath))
        {
            EditorUtility.DisplayDialog(
                "Error",
                $"Script already exists:\n{scriptPath}",
                "OK"
            );
            return;
        }

        string scriptContent = GenerateScriptContent(className, menuName,namespaceName);

        System.IO.File.WriteAllText(scriptPath, scriptContent);

        EditorPrefs.SetString(PendingClassKey, className);
        EditorPrefs.SetString(PendingNamespaceKey, namespaceName);
        EditorPrefs.SetString(PendingAssetFolderKey, assetFolder);

        AssetDatabase.Refresh();

        Debug.Log($"Created script: {scriptPath}. Waiting for Unity to compile...");
    }

}


[InitializeOnLoad]
public static class PendingScriptableObjectAssetCreator
{
    private const string PendingClassKey = "PendingSOClassName";
    private const string PendingNamespaceKey = "PendingSONamespace";
    private const string PendingAssetFolderKey = "PendingSOAssetFolder";

    static PendingScriptableObjectAssetCreator()
    {
        EditorApplication.delayCall += TryCreatePendingAsset;
    }

    private static void TryCreatePendingAsset()
    {
        if (!EditorPrefs.HasKey(PendingClassKey))
            return;

        string className = EditorPrefs.GetString(PendingClassKey);
        string namespaceName = EditorPrefs.GetString(PendingNamespaceKey);
        string assetFolder = EditorPrefs.GetString(PendingAssetFolderKey);

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

        EditorPrefs.DeleteKey(PendingClassKey);
        EditorPrefs.DeleteKey(PendingNamespaceKey);
        EditorPrefs.DeleteKey(PendingAssetFolderKey);

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
