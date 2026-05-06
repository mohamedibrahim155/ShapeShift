using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;

namespace Scripts.Editor.ScriptableObjectGenerator
{
    public class TypeSearchPopUpEditor : EditorWindow
    {
        [Header("Runtime Data")]
        private Action<Type> onTypeSelected;
        private Vector2 scroll;
        private int selectedIndex = 0;


        [Header("Filtering Data")]
        private bool shouldFocusSearch = true;
        private string search = "";
        private Vector2 lastMousePosition;
        private TypeFilter currentFilter = TypeFilter.All;
        private List<Type> filteredTypes = new();

      

        private void OnEnable()
        {
            wantsMouseMove = true;
        }

        public static void ShowWindow(Action<Type> onSelected, Vector2 windowPosition, TypeFilter typeFilter = TypeFilter.All)
        {
            var window = CreateInstance<TypeSearchPopUpEditor>();
            window.titleContent = new GUIContent(SOGeneratorSettings.TypeSearchWindowTitle);
            window.currentFilter = typeFilter;
            window.position = new Rect(windowPosition.x, windowPosition.y, 300, 400);
            window.onTypeSelected = onSelected;
            window.ShowPopup();
        }

        private void OnLostFocus()
        {
            Close();
        }

        private void OnGUI()
        {

            HandleKeyboardInput();


            GUI.SetNextControlName(SOGeneratorSettings.TypeSearchControlName);

            string newSearch = EditorGUILayout.TextField(search);

            if (shouldFocusSearch)
            {
                EditorGUI.FocusTextInControl(SOGeneratorSettings.TypeSearchControlName);
                shouldFocusSearch = false;
            }

            if (newSearch != search)
            {
                search = newSearch;
                selectedIndex = 0;
            }

            UpdateFilteredTypes();

            scroll = EditorGUILayout.BeginScrollView(scroll);
            Event e = Event.current;

            // mouse selection
            bool mouseMoved = e.type == EventType.MouseMove && e.mousePosition != lastMousePosition;
            if (mouseMoved)
            {
                lastMousePosition = e.mousePosition;
            }

            if (filteredTypes != null)
            {
                for (int i = 0; i < filteredTypes.Count; i++)
                {
                    Type type = filteredTypes[i];

                    Rect rowRect = GUILayoutUtility.GetRect(
                        GUIContent.none,
                        EditorStyles.label,
                        GUILayout.Height(SOGeneratorSettings.RowHeight),
                        GUILayout.ExpandWidth(true)
                    );



                    bool isHovering = rowRect.Contains(e.mousePosition);
                    bool isSelected = i == selectedIndex;

                    if (mouseMoved && isHovering && selectedIndex != i)
                    {
                        selectedIndex = i;
                        Repaint();
                    }

                    if (e.type == EventType.Repaint && isSelected)
                    {
                        EditorGUI.DrawRect(rowRect, SOGeneratorSettings.SelectedColor);
                    }

                    GUI.Label(
                        new Rect(rowRect.x + 6, rowRect.y, rowRect.width - 6, rowRect.height),
                        type.Name,
                        EditorStyles.label
                    );

                    if (isHovering && e.type == EventType.MouseDown && e.button == 0)
                    {
                        selectedIndex = i;
                        SelectType(type);
                        e.Use();
                    }
                }
            }


            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Handles the keyboard input for navigating the type list and selecting a type. 
        /// Arrow keys are used for navigation,
        /// Enter for selection, and Escape to close the window.
        /// </summary>
        private void HandleKeyboardInput()
        {
            Event e = Event.current;

            if (e.type != EventType.KeyDown)
                return;

            if (filteredTypes == null || filteredTypes.Count == 0)
                return;

            switch (e.keyCode)
            {
                case KeyCode.DownArrow:
                    selectedIndex = Mathf.Min(selectedIndex + 1, filteredTypes.Count - 1);
                    ScrollToSelected();
                    e.Use();
                    Repaint();
                    break;

                case KeyCode.UpArrow:
                    selectedIndex = Mathf.Max(selectedIndex - 1, 0);
                    ScrollToSelected();
                    e.Use();
                    Repaint();
                    break;

                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    SelectType(filteredTypes[selectedIndex]);
                    e.Use();
                    break;

                case KeyCode.Escape:
                    Close();
                    e.Use();
                    break;
            }
        }

        private void SelectType(Type type)
        {
            onTypeSelected?.Invoke(type);
            Close();
        }

        private void UpdateFilteredTypes()
        {

            filteredTypes = TypeSearchService.GetTypes(search, currentFilter);

            if (filteredTypes.Count == 0)
            {
                selectedIndex = 0;
                return;
            }

            selectedIndex = Mathf.Clamp(selectedIndex, 0, filteredTypes.Count - 1);
        }

        private void ScrollToSelected()
        {
            float selectedY = selectedIndex * SOGeneratorSettings.RowHeight;

            if (selectedY < scroll.y)
            {
                scroll.y = selectedY;
            }
            else if (selectedY + SOGeneratorSettings.RowHeight > scroll.y + position.height - 30f)
            {
                scroll.y = selectedY - position.height + 50f;
            }
        }

    }
}
