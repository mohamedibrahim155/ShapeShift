using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.Rendering;
using UnityEngine;

namespace Scripts.Editor.ScriptableObjectGenerator
{
    public class ScriptableObjectCreator : EditorWindow
    {
        [Header("Main Settings")]
        private string namespaceName = SOGeneratorSettings.defaultNamespaceName;
        private string scriptFolder = SOGeneratorSettings.defaultScriptFolder;
        private string assetFolder = SOGeneratorSettings.defaultAssetFolder;
        private string className = SOGeneratorSettings.defaultClassName;
        private string menuName = SOGeneratorSettings.defaultMenuName;

        private SORequestData requestData;

        private Vector2 scrollPosition;

        [Header("Field Definitions")]
        private FieldDefinition[] fieldDefinitions = new FieldDefinition[0];
        private FieldDefinition copiedField;


        [Header("UI- Styles")]
        private GUIStyle headerStyle;
        private GUIStyle sectionStyle;
        private GUIContent createButtonContent;
        private static ScriptableObjectCreator currentWindow;

        [MenuItem("Tools/Scriptable Object Generator")]
        public static void Open()
        {
            currentWindow = GetWindow<ScriptableObjectCreator>(SOGeneratorSettings.GeneratorWindowTitle);
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

            menuName = EditorGUILayout.TextField(
              new GUIContent("Menu Name", "Menu name for the ScriptableObject to be created <menuName>/<Class name>."),
              menuName
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



        private void DrawFieldDefinitions()
        {
            for (int i = 0; i < fieldDefinitions.Length; i++)
            {
                FieldDefinition fieldDef = fieldDefinitions[i];

                Rect nodeRect = EditorGUILayout.BeginVertical("box");

                EditorGUILayout.BeginHorizontal();

                fieldDef.isExpanded = EditorGUILayout.Foldout(
                    fieldDef.isExpanded,
                    string.IsNullOrEmpty(fieldDef.fieldName) ? $"Field {i + 1}" : SOCodeGeneratorService.GetFieldLine(fieldDef),
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
                        EditorGUILayout.LabelField("Custom Class Name");



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
                            }, screenPosition, TypeFilter.Class);
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
                            }, screenPosition, TypeFilter.Enum);
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


                    bool isFieldValid = SOValidator.ValidateField(fieldDef, out string errotField);

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel, GUILayout.Width(80));

                    string icon = isFieldValid ? "✅" : "❌";
                    GUILayout.Label(icon, GUILayout.Width(25));

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.HelpBox(SOCodeGeneratorService.GetFieldLine(fieldDef), MessageType.None);

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

        /// <summary>
        /// Ensure the folder exists  and offer to create it if it doesn't.
        /// Returns true if the folder exists or was created successfully, false if the user canceled.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool EnsureFolderExist(string label, string path)
        {
            if (SOValidator.ValidateFolder(path, out _))
                return true;

            bool create = EditorUtility.DisplayDialog(
                "Missing Folder",
                $"{label} does not exist:\n\n{path}\n\nDo you want to create it?",
                "Create",
                "Cancel"
            );

            if (!create)
                return false;

            SOValidator.CreateFolderIfNeeded(path, out string createError);

            if (!string.IsNullOrEmpty(createError))
            {
                EditorUtility.DisplayDialog("Folder Creation Failed", createError, "OK");
                return false;
            }

            return true;
        }
        private bool ValidateBeforeCreate()
        {
            if (!SOValidator.ValidateAll(className, namespaceName, fieldDefinitions, out string error))
            {
                EditorUtility.DisplayDialog("Validation Error", error, "OK");
                return false;
            }

            if (!EnsureFolderExist("Script Folder", scriptFolder))
                return false;

            if (!EnsureFolderExist("Asset Folder", assetFolder))
                return false;

            return true;

        }


        private SORequestData BuildRequestData()
        {
            return new SORequestData
                (className, 
                namespaceName,
                menuName,
                scriptFolder,
                assetFolder, 
                fieldDefinitions);

        }

        /// <summary>
        ///  Functionality for creating the ScriptableObject script 
        ///  and asset based on the user input and field definitions.
        /// </summary>
        private void CreateScriptableObjectScript()
        {
            if (!ValidateBeforeCreate())
                return;

            var requestData = BuildRequestData();
            var result = SOGeneratorService.Create(requestData);

            if (!result.Success)
            {
                EditorUtility.DisplayDialog("Error", $"{result.Message}", "OK");
                return;
            }

            Debug.Log($"{result.Message}. Waiting for Unity to compile...");
           
        }

    }
}
