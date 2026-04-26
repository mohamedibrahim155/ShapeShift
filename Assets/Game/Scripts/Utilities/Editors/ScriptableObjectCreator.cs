using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
public class ScriptableObjectCreator : EditorWindow
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

    private static ScriptableObjectCreator currentWindow;

    private GUIStyle headerStyle;
    private GUIStyle sectionStyle;
    private GUIContent createButtonContent;
    private GUIStyle centeredButton;

    [MenuItem("Tools/Scriptable Object Generator")]
    public static void Open()
    {
        currentWindow = GetWindow<ScriptableObjectCreator>("SO Generator");
        currentWindow.InitStyles();
    }

 


    private void OnGUI()
    {
        DrawHeader();

        DrawMainSettings();

        GUILayout.Space(8);

        DrawFieldHeader();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        DrawFieldDefinitions();
        EditorGUILayout.EndScrollView();

        GUILayout.FlexibleSpace();

        DrawBottomBar();
        GUILayout.Space(10);

    }

    private void InitStyles()
    {
        if (headerStyle == null)
        {
            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleLeft
            };
        }

        if (sectionStyle == null)
        {
            sectionStyle = new GUIStyle("box")
            {
                padding = new RectOffset(10, 10, 8, 8),
                margin = new RectOffset(5, 5, 5, 5)
            };
        }

        if (createButtonContent == null)
        {
            Texture icon = EditorGUIUtility.IconContent("ScriptableObject Icon").image;

            if (icon == null)
                icon = EditorGUIUtility.IconContent("d_CreateAddNew").image;

            createButtonContent = new GUIContent(" Create ScriptableObject", icon);
        }
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginVertical(sectionStyle);

        EditorGUILayout.LabelField("ScriptableObject Generator", headerStyle);

        EditorGUILayout.LabelField(
            "Create ScriptableObject scripts and assets from custom field definitions.",
            EditorStyles.miniLabel
        );

        EditorGUILayout.EndVertical();
    }

    private void DrawMainSettings()
    {
        EditorGUILayout.BeginVertical(sectionStyle);

        EditorGUILayout.LabelField("Create ScriptableObject Script + Asset", EditorStyles.boldLabel);

        className = EditorGUILayout.TextField(
            new GUIContent("Class Name", "Name of the generated ScriptableObject class."),
            className
        );

        namespaceName = EditorGUILayout.TextField(
            new GUIContent("Namespace", "Optional namespace for the generated class."),
            namespaceName
        );

        scriptFolder = EditorGUILayout.TextField(
            new GUIContent("Script Folder", "Folder where the .cs script will be created."),
            scriptFolder
        );

        assetFolder = EditorGUILayout.TextField(
            new GUIContent("Asset Folder", "Folder where the .asset file will be created."),
            assetFolder
        );

        EditorGUILayout.EndVertical();
    }

    private void DrawFieldHeader()
    {
        EditorGUILayout.BeginHorizontal(sectionStyle);

        EditorGUILayout.LabelField("Field Definitions", EditorStyles.boldLabel);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button(
            new GUIContent(" Add Field", EditorGUIUtility.IconContent("Toolbar Plus").image),
            GUILayout.Width(120),
            GUILayout.Height(26)))
        {
            AddFieldDefinition();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawBottomBar()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button(
            createButtonContent,
            GUILayout.Height(36),
            GUILayout.Width(260)))
        {
            CreateScriptableObjectScript();
        }

        GUILayout.FlexibleSpace();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private string GetFieldType(FieldDefinition sOFieldDefinition)
    {
        string enumTypeName = sOFieldDefinition.isCustomEnum ? ConvertTypeToName(sOFieldDefinition.customEnum) : "";
        string classTypeName = sOFieldDefinition.isCustomClass ? ConvertTypeToName(sOFieldDefinition.customClassType) : "";

        string fieldType =  sOFieldDefinition.fieldType switch 
        {
            SOFieldType.Enum=> enumTypeName,
            SOFieldType.CustomClass => classTypeName,
            _ => ConvertFieldType(sOFieldDefinition.fieldType)
        };
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

    private string ConvertFieldType(SOFieldType type)
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


    private void DrawFieldDefinitions()
    {
        for (int i = 0; i < fieldDefinitions.Length; i++)
        {
            FieldDefinition fieldDef = fieldDefinitions[i];

            Rect nodeRect = EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();

            fieldDef.isExpanded = EditorGUILayout.Foldout(
                fieldDef.isExpanded,
                string.IsNullOrEmpty(fieldDef.fieldName) ? $"Field {i + 1}" : SOCodeGenerator.GetFieldLine(fieldDef),
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
                        }, screenPosition, TypeSearchPopUpEditor.TypeFilter.Class);
                    }

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    fieldDef.isCustomClass = false;
                }


                if (fieldDef.fieldType == SOFieldType.Enum)
                {
                    fieldDef.isCustomEnum = true;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Custom Enum Name");



                    string typeName = fieldDef.customEnum != null
                        ? fieldDef.customEnum.Name
                        : "Select Type";

                    if (GUILayout.Button(typeName, EditorStyles.popup))
                    {
                        Rect buttonRect = GUILayoutUtility.GetLastRect();

                        Vector2 screenPosition = GUIUtility.GUIToScreenPoint(
                            new Vector2(buttonRect.x, buttonRect.yMax)
                        );

                        TypeSearchPopUpEditor.ShowWindow((selectedType) =>
                        {
                            fieldDef.customEnum = selectedType;
                            Repaint();
                        }, screenPosition, TypeSearchPopUpEditor.TypeFilter.Enum);
                    }

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    fieldDef.isCustomEnum = false;
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


                bool  isFieldValid = SOValidator.ValidateField(fieldDef, out string errotField);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel, GUILayout.Width(80));

                string icon = isFieldValid ? "✅" : "❌";
                GUILayout.Label(icon, GUILayout.Width(25));

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.HelpBox(SOCodeGenerator.GetFieldLine(fieldDef), MessageType.None);

                if (!isFieldValid)
                {
                    EditorGUILayout.HelpBox(errotField, MessageType.Error);
                }


                EditorGUI.indentLevel--;
            }

           



            EditorGUILayout.EndVertical();
        }
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

    private bool ValidateBeforeCreate()
    {

      bool validation  = SOValidator.ValidateAll(
            className,
            namespaceName,
            fieldDefinitions,
            out string error
        );
        if (!validation)
        {
            EditorUtility.DisplayDialog("Validation Error", error, "OK");
            return false;
        }

        //Script folder check
        if (!SOValidator.ValidateFolder(scriptFolder, out string folderError) &&  !string.IsNullOrEmpty(folderError))
        {
            if (EditorUtility.DisplayDialog("Validation Error", $"Script folder path is invalid.. Do you want to create folder at {scriptFolder}?", "Create", "Cancel"))
            {
                CreateFolderIfNeeded(scriptFolder);
            }
            return false;
        }

        //Asset folder check
        if (!SOValidator.ValidateFolder(assetFolder, out string assetFolderError) && !string.IsNullOrEmpty(assetFolderError))
        {
            if (EditorUtility.DisplayDialog("Validation Error", $"Asset folder path is invalid.. Do you want to create folder at {assetFolder}?", "Create", "Cancel"))
            {
                CreateFolderIfNeeded(assetFolder);
            }
            return false;
        }

        string scriptPath = $"{scriptFolder}/{className}.cs";

        if (!SOValidator.ValidateFile(scriptPath, out string fileErrror))
        {
            EditorUtility.DisplayDialog( "Validation Error", $"{folderError}", "OK" );
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

        if (!SOValidator.ValidateFile(scriptPath,out string error))
        {
            EditorUtility.DisplayDialog("Error", $"{error}", "OK");
            return;
        }

       // string scriptContent = GenerateScriptContent(className, menuName,namespaceName);
        string scriptContent = SOCodeGenerator.Generate(className, namespaceName, menuName, fieldDefinitions);

        System.IO.File.WriteAllText(scriptPath, scriptContent);

        EditorPrefs.SetString(PendingClassKey, className);
        EditorPrefs.SetString(PendingNamespaceKey, namespaceName);
        EditorPrefs.SetString(PendingAssetFolderKey, assetFolder);

        AssetDatabase.Refresh();

        Debug.Log($"Created script: {scriptPath}. Waiting for Unity to compile...");
    }

}


//Scripts  that auto trigger after script compilation to check
//if there is pending SO to create
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
