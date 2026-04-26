using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TypeSearchPopUpEditor : EditorWindow
{

    private Action<Type> onTypeSelected;
    private Vector2 scroll;
    private string search = "";

    private List<Type> allTypes;
    private static TypeSearchPopUpEditor Instance;

    private List<Type> filteredTypes = new();
    private int selectedIndex = 0;
    private const float RowHeight = 20f;

    public static void ShowWindow(Action<Type> onSelected, Vector2 windowPosition)
    {
        //var window = GetWindow<TypeSearchPopUpEditor>();
        //if (window != null)
        //{
        //    window.titleContent = new GUIContent("Select Type");
        //    window.position = new Rect(windowPosition.x, windowPosition.y, 300, 400);

        //    window.onTypeSelected = onSelected;
        //    window.Init();
        //    window.ShowPopup();
        //    return;
        //}
        var window = CreateInstance<TypeSearchPopUpEditor>();
        window.titleContent = new GUIContent("Select Type");
        window.position = new Rect(windowPosition.x, windowPosition.y, 300, 400);
        Instance = window;
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

    public static void CloseWindow()
    {
        
        

    }

    private void Init()
    {

        allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a =>
        {
        try { return a.GetTypes(); }
        catch { return new Type[0]; } })
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType && 

        //  REMOVE COMPILER GENERATED
        !t.Name.StartsWith("<") &&
        !t.Name.Contains("AnonymousType") &&
        !Attribute.IsDefined(t, typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)) &&

        //  ONLY VALID UNITY TYPES
        (typeof(UnityEngine.Object).IsAssignableFrom(t) || t.IsSerializable) ) 
         .OrderBy(t => t.Name)
         .ToList();

    }

    private void OnGUI()
    {
        HandleKeyboardInput();

        string newSearch = EditorGUILayout.TextField(search);

        if (newSearch != search)
        {
            search = newSearch;
            selectedIndex = 0;
        }

        UpdateFilteredTypes();

        scroll = EditorGUILayout.BeginScrollView(scroll);

        if (allTypes != null)
        {
            if (filteredTypes != null)
            {
                for (int i = 0; i < filteredTypes.Count; i++)
                {
                    Type type = filteredTypes[i];

                    Rect rowRect = EditorGUILayout.BeginHorizontal();

                    bool isSelected = i == selectedIndex;

                    if (isSelected)
                    {
                        EditorGUI.DrawRect(rowRect, new Color(0.25f, 0.45f, 0.85f, 0.5f));
                    }

                    if (GUILayout.Button(type.Name, EditorStyles.label))
                    {
                        SelectType(type);
                    }

                    EditorGUILayout.EndHorizontal();

                    if (Event.current.type == EventType.MouseMove && rowRect.Contains(Event.current.mousePosition))
                    {
                        selectedIndex = i;
                        Repaint();
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
            .Where(t =>
                t != null &&
                (string.IsNullOrEmpty(lowerSearch) ||
                 t.Name.ToLower().Contains(lowerSearch)))
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
