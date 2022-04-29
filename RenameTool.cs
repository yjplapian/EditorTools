#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class RenameTool : EditorWindow
{
    #region Vars
    private Object[] _selection;

    private bool _useBaseName;
    private string _baseName;
    private bool _usePrefix;
    private string _prefix;
    private bool _useSuffix;
    private string _suffix;

    private bool _useNumbering;
    private int _numberedIndexOption;
    private string[] _numberedOptions = {"use (00)", "use .00", "use _00" };

    private bool _useReplacement;
    private string _toReplaceString;
    private string _replaceWithString;

    private Vector2 _scrollView;
    #endregion

    [MenuItem("Tools/Misc/Rename Tool")]
    private static void ShowWindow() => GetWindow<RenameTool>("Object Renaming Tool");

    #region GUI
    private void OnSelectionChange() => Repaint();
    
    private void OnGUI()
    {
        if(Selection.objects.Length > 0)
        _selection = Selection.objects;

        OptionsGUI();

        GUILayout.Space(50);
        ScrollViewGUI();
        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Apply Changes"))
            RenameSelectedobjects();    

        if (GUILayout.Button("Clear Settings"))   
            ClearSettings();
        
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
    }

    private void ScrollViewGUI()
    {
        _scrollView = GUILayout.BeginScrollView(_scrollView, true, true);

        if (Selection.objects != null)
        {
            for (int i = 0; i < _selection.Length; i++)
                GUILayout.Label(_selection[i].name);
        }
        GUILayout.EndScrollView();
    }

    private void OptionsGUI()
    {
        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Use Base Name", GUILayout.MaxWidth(150));
        _useBaseName = EditorGUILayout.Toggle(_useBaseName, GUILayout.MaxWidth(20));

        if (!_useBaseName) GUI.enabled = false;
        _baseName = EditorGUILayout.TextField(_baseName);
        GUI.enabled = true;
        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Use Prefix", GUILayout.MaxWidth(150));
        _usePrefix = EditorGUILayout.Toggle(_usePrefix, GUILayout.MaxWidth(20));

        if (!_usePrefix) GUI.enabled = false;
        _prefix = EditorGUILayout.TextField(_prefix);
        GUI.enabled = true;
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Use Suffix", GUILayout.MaxWidth(150));
        _useSuffix = EditorGUILayout.Toggle(_useSuffix, GUILayout.MaxWidth(20));

        if (!_useSuffix) GUI.enabled = false;
        _suffix = EditorGUILayout.TextField(_suffix);
        GUI.enabled = true;
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Use Numbering", GUILayout.MaxWidth(150));
        _useNumbering = EditorGUILayout.Toggle(_useNumbering, GUILayout.MaxWidth(20));

        if (!_useNumbering) GUI.enabled = false;
        _numberedIndexOption = EditorGUILayout.Popup(_numberedIndexOption, _numberedOptions);
        GUI.enabled = true;
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Use Text Replacer", GUILayout.MaxWidth(150));
        _useReplacement = EditorGUILayout.Toggle(_useReplacement, GUILayout.MaxWidth(20));

        if (!_useReplacement)
        {
            GUI.enabled = false;
            _toReplaceString = "Text to replace...";
            _replaceWithString = "Replace with...";
        }

        GUILayout.BeginVertical();
        _toReplaceString = EditorGUILayout.TextField(_toReplaceString);
        _replaceWithString = EditorGUILayout.TextField(_replaceWithString);
        GUILayout.EndVertical();

        GUI.enabled = true;
        GUILayout.EndHorizontal();
    }
    #endregion

    #region Functions


    private void RenameSelectedobjects()
    {
        for (int i = 0; i < _selection.Length; i++)
        {
            if (_useBaseName)
                _selection[i].name = _baseName;

            if(_usePrefix)
                _selection[i].name = _prefix + _selection[i].name;

            if (_useSuffix)
                _selection[i].name = _selection[i].name + _suffix;

            if (_useNumbering) 
                _selection[i].name = AddNumbering(_numberedIndexOption, _selection[i].name, i);

            if (_useReplacement)
                _selection[i].name = _selection[i].name.Replace(_toReplaceString, _replaceWithString);

            if (AssetDatabase.GetAssetPath(_selection[i]) != null)
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_selection[i]), _selection[i].name);
        }
    }

    private string AddNumbering(int index, string text, int value)
    {
        if(index < 0 || index > 2)
        {
            Debug.LogError("Value out of range.");
        }

        switch (index)
        {
            case 0:
                text += " (" + value + ")";
                break;

            case 1:
                text += "." + value;
                break;

            case 2:
                text += "_" + value;
                break;
        }

        return text;
    }

    private void ClearSettings()
    {
        _selection = null;
        Selection.objects = null;

        _useBaseName = false;
        _baseName = string.Empty;
        _usePrefix = false;
        _prefix = string.Empty;
        _useSuffix = false;
        _suffix = string.Empty;
        _numberedIndexOption = 0;
        _useReplacement = false;
        _useNumbering = false;
        _replaceWithString = string.Empty;
        _toReplaceString = string.Empty;
    }
    
    #endregion
}
#endif