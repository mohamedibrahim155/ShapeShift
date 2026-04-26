using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TypeSearchPopUpEditor : EditorWindow
{

    private Action<Type> onTypeSelected;
    private Vector2 scroll;
    private string search = "";

    private static List<Type> allTypes = null;

    private List<Type> filteredTypes = new();
    private int selectedIndex = 0;
    private const float RowHeight = 20f;
    private TypeFilter currentFilter = TypeFilter.All;
    private const string SearchControlName = "TypeSearchField";
    private bool shouldFocusSearch = true;
    private Vector2 lastMousePosition;
    public enum TypeFilter
    {
        All,
        UnityTypes,
        Class,
        SerializableTypes,
        Enum,
    }

    private void OnEnable()
    {
        wantsMouseMove = true;
    }

    public static void ShowWindow(Action<Type> onSelected, Vector2 windowPosition, TypeFilter typeFilter = TypeFilter.All)
    {
        var window = CreateInstance<TypeSearchPopUpEditor>();
        window.titleContent = new GUIContent("Select Type");
        window.currentFilter = typeFilter;
        window.position = new Rect(windowPosition.x, windowPosition.y, 300, 400);
        window.onTypeSelected = onSelected;
        window.Init();
        window.ShowPopup();

        Debug.Log("Window clicked");

    }

    [MenuItem("Tools/CloseAllOpened")]
    public static void CloseAllWindows()
    {

        object[] allWindows = Resources.FindObjectsOfTypeAll(typeof(TypeSearchPopUpEditor));

        foreach (var item in allWindows)
        {
            if (item != null)
            {
                TypeSearchPopUpEditor window = item as TypeSearchPopUpEditor;
                if (window != null)
                {
                    window.Close();
                }
            }
        }
    }

    private void Init()
    {
        if (allTypes != null) return;
        allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a =>
        {
        try { return a.GetTypes(); }
        catch { return new Type[0]; } })
            .Where(t => (t.IsClass || t.IsEnum) && !t.IsAbstract && !t.IsGenericType && t.IsPublic &&

        //  REMOVE COMPILER GENERATED
        !t.Name.StartsWith("<") &&
        !t.Name.Contains("AnonymousType") &&
        !Attribute.IsDefined(t, typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)) &&

        //  ONLY VALID UNITY TYPES
        (typeof(UnityEngine.Object).IsAssignableFrom(t) || t.IsSerializable) ) 
         .OrderBy(t => t.Name)
         .ToList();

    }

    private void OnLostFocus()
    {
        Close();
    }

    private void OnGUI()
    {
        HandleKeyboardInput();


        GUI.SetNextControlName(SearchControlName);

        string newSearch = EditorGUILayout.TextField(search);

        if (shouldFocusSearch)
        {
            EditorGUI.FocusTextInControl(SearchControlName);
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

        bool mouseMoved = e.type == EventType.MouseMove && e.mousePosition != lastMousePosition;
        if (mouseMoved)
        {
            lastMousePosition = e.mousePosition;
        }

        if (allTypes != null)
        {
            if (filteredTypes != null)
            {
                for (int i = 0; i < filteredTypes.Count; i++)
                {
                    Type type = filteredTypes[i];

                    Rect rowRect = GUILayoutUtility.GetRect(
                        GUIContent.none,
                        EditorStyles.label,
                        GUILayout.Height(RowHeight),
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
                        EditorGUI.DrawRect(rowRect, new Color(0.24f, 0.48f, 0.90f, 0.45f));
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

        }


        EditorGUILayout.EndScrollView();
    }


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

        if (allTypes == null)
        {
            filteredTypes = new List<Type>();
            return;
        }

        string lowerSearch = search.ToLower();

        filteredTypes = allTypes
        .Where(t => {
            // Text Search Match
            bool matchesSearch = string.IsNullOrEmpty(lowerSearch) || t.Name.ToLower().Contains(lowerSearch);
            if (!matchesSearch) return false;

            // Category Filter Match
            return currentFilter switch
            {
                TypeFilter.UnityTypes => typeof(UnityEngine.Object).IsAssignableFrom(t),
                TypeFilter.SerializableTypes => t.IsSerializable && !t.IsEnum,
                TypeFilter.Class => t.IsClass,
                TypeFilter.Enum => t.IsEnum && !t.IsClass,
                _ => true // TypeFilter.All
            };
        })
        .ToList();

        if (filteredTypes.Count == 0)
        {
            selectedIndex = 0;
            return;
        }

        selectedIndex = Mathf.Clamp(selectedIndex, 0, filteredTypes.Count - 1);
    }

    private void ScrollToSelected()
    {
        float selectedY = selectedIndex * RowHeight;

        if (selectedY < scroll.y)
        {
            scroll.y = selectedY;
        }
        else if (selectedY + RowHeight > scroll.y + position.height - 30f)
        {
            scroll.y = selectedY - position.height + 50f;
        }
    }

}
