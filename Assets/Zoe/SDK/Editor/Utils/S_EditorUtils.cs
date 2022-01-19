using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class S_EditorUtils
{
    private static int m_LastFontsize;
    private static bool m_initializedFontType = false;
    private static FontStyle m_LastFontStyle;
    private static EditorBuildSettingsScene[] m_Scenes;
    private static List<string> m_SceneNames;

    public static List<string> GetScenesFromBuildSettings()
    {
        // get all scenes
        m_Scenes = EditorBuildSettings.scenes;

        // store name of the scenes
        m_SceneNames = new List<string>();
        for (int i = 0; i < m_Scenes.Length; i++)
        {
            if (m_Scenes[i].enabled)
                m_SceneNames.Add(GetSceneName(m_Scenes[i]));
        }

        return m_SceneNames;
    }

    private static string GetSceneName(EditorBuildSettingsScene S)
    {
        string name = S.path.Substring(S.path.LastIndexOf('/') + 1);
        return name.Substring(0, name.Length - 6);
    }

    public static void DisplayBuildSettingsSceneNames()
    {
        GetScenesFromBuildSettings();
        for (int i = 0; i < m_SceneNames.Count; i++)
        {
            Debug.Log("Build settings scene : " + m_SceneNames[i]);
        }
    }

    public static void StoreLastStyles()
    {
        InitFontType();
        m_LastFontsize = EditorStyles.label.fontSize;
        m_LastFontStyle = EditorStyles.label.fontStyle;
    }

    public static void RestoreLastStyles()
    {
        EditorStyles.label.fontStyle = m_LastFontStyle;
        EditorStyles.label.fontSize = m_LastFontsize;
    }

    public static void InitFontType()
    {
        if (!m_initializedFontType)
        {
            m_initializedFontType = true;
            // Arial, Cambria, Century, Comic Sans MS
            // string[] OSFonts = Font.GetOSInstalledFontNames();
            EditorStyles.label.font = Font.CreateDynamicFontFromOSFont("Arial", 1);
        }
    }

    public static void DrawSectionTitle(string _title, FontStyle _fontStyle = FontStyle.Bold, int _fontSize = 12)
    {
        StoreLastStyles();
        EditorStyles.label.fontStyle = _fontStyle;
        EditorStyles.label.fontSize = _fontSize;
        EditorGUILayout.LabelField(_title);
        RestoreLastStyles();
    }

    public static void DrawSectionTitle(string _title, int _labelWidth, FontStyle _fontStyle = FontStyle.Bold, int _fontSize = 12)
    {
        StoreLastStyles();
        EditorStyles.label.fontStyle = _fontStyle;
        EditorStyles.label.fontSize = _fontSize;
        EditorGUILayout.LabelField(new GUIContent(_title), GUILayout.MaxWidth(_labelWidth));
        RestoreLastStyles();
    }

    public static void DrawEditorHint(string _hint, bool _spaceAfterHint = true, FontStyle _fontStyle = FontStyle.Italic, int _fontSize = 10)
    {
        StoreLastStyles();
        EditorStyles.label.fontStyle = _fontStyle;
        EditorStyles.label.fontSize = _fontSize;
        EditorGUILayout.LabelField(_hint);
        RestoreLastStyles();
        if (_spaceAfterHint)
            EditorGUILayout.Space();
    }

    public static int HintPopupOptions(string _title, int _currentPopupIndex, string[] _options, string _hint, int _labelWidth)
    {
        int index;
        GUILayout.BeginHorizontal();
        if (((_title != null) && (_title.Length>0)) || ((_hint != null) && (_hint.Length > 0)))
        {
            EditorGUILayout.LabelField(new GUIContent(_title, _hint), GUILayout.MaxWidth(_labelWidth));
        }        
        index = EditorGUILayout.Popup("", _currentPopupIndex, _options);
        GUILayout.EndHorizontal();
        return index;
    }

    public static void SubTitle(string _text)
    {
        EditorGUILayout.LabelField(_text, EditorStyles.boldLabel);
    }

    public static void ShowIndentedBlock(Action _renderMethod, int _indentLevel)
    {
        int lastIndentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = _indentLevel;
        _renderMethod();
        EditorGUI.indentLevel = lastIndentLevel;
    }

    /// <summary>
    /// Used to delete elements from arrays as they where lists, its cool in order to
    /// keep arrays instead of lists on the code, but use it only for editor code because
    /// it is pretty slow.
    /// </summary>
    /// <typeparam name="T"> List tyle </typeparam>
    /// <param name="_array"> Target array </param>
    /// <param name="_index"> Target index </param>
    /// <returns>A copy of the array without the element and shortened by one</returns>
    public static T[] RemoveElementFromArrayByIndex<T>(T[] _array, int _index)
    {
        // If the array length is 1 or less just return an empty array
        if (_array.Length <= 1)
            return new T[0];

        List<T> temp = new List<T>();
        for (int i = 0; i < _array.Length; i++)
        {
            T element = _array[i];
            if (i != _index)
                temp.Add(element);
        }
        return temp.ToArray();
    }

    internal static Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}