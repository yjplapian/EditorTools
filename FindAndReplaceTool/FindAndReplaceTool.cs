using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FindAndReplaceTool : EditorWindow
{
    //Search objects in hierarchy by: Name, Type, Tag [x]
    //Draw Array of found objects [x]
    //Allow individual toggles in array of objects to replace [x]
    //Draw GameObject field to replace selection with [x]
    //Draw toggle to keep current rotation [x]
    //Draw toggle to keep current position [x]
    //Draw toggle to keep current scale [x]
    //Draw toggle to customize transform [x]
    //Draw Vector3fields position, rotation, scale [x] 
    //Draw Button to activate replacing function [x]
    //Draw button to clear settings [x]

    //Allow individual transform customization [ ]

    private List<RObject> _foundObjects = new List<RObject>();
    private List<bool> _foundObjectButtons = new List<bool> ();
    private GameObject _replacementGameObject;

    private string _searchForString;

    private int _searchByIndex;
    private string[] searchByOptions = { "Name", "Type", "Tag", "Selection" };

    private bool _keepPosition;
    private bool _keepRotation;
    private bool _keepScale;

    private bool _replaceAll;
    private bool _UseSameValuesForAll;

    private bool _customizeTransform;
    private Vector3 _customPosition;
    private Vector3 _customRotation;
    private Vector3 _customScale;

    private Vector2 _scrollView;

    [MenuItem("Tools/Level Design/Find and Replace...")]
    private static void ShowWindow() => GetWindow<FindAndReplaceTool>("Find and Replace");
    
    private void OnGUI()
    {
        OptionGUI();
        TransformGUI();
        ScrollViewGUI();
        ButtonGUI();
    }

    private void OptionGUI()
    {
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);

        GUILayout.Label("Search For: ", GUILayout.MaxWidth(80));
        _searchForString = EditorGUILayout.TextField(_searchForString);

        GUILayout.Space(5);

        GUILayout.Label("By: ", GUILayout.MaxWidth(30));
        _searchByIndex = EditorGUILayout.Popup(_searchByIndex, searchByOptions);

        GUILayout.Space(10);

        if (GUILayout.Button("Start Search"))  
            GetSearchResult(_searchByIndex);  

        GUILayout.Space(20);
        GUILayout.EndHorizontal();
        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUILayout.Label("Replacing Object: ", GUILayout.MaxWidth(120));
        _replacementGameObject = (GameObject)EditorGUILayout.ObjectField(_replacementGameObject, typeof(GameObject), false);

        GUILayout.Space(20);
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
    }

    private void ScrollViewGUI()
    {
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        GUILayout.Space(500);

        _scrollView = GUILayout.BeginScrollView(_scrollView, false, true);

        for (int i = 0; i < _foundObjects.Count; i++)
        {
            GUILayout.BeginHorizontal();

            if (_foundObjects[i] != null)
            {
                _foundObjects[i].selected = GUILayout.Button(_foundObjects[i].name);

                if (!_foundObjectButtons.Contains(_foundObjects[i].selected))               
                    _foundObjectButtons.Add(_foundObjects[i].selected);
                
                if (_foundObjects[i].selected && !_foundObjects[i].hasEditedTransform)
                {
                    _customPosition = _foundObjects[i].transform.position;
                    _customRotation = _foundObjects[i].transform.rotation.eulerAngles;
                    _customScale = _foundObjects[i].transform.localScale;
                }

                if (!_foundObjects[i].replace)         
                    _foundObjects[i].replace = GUILayout.Toggle(_foundObjects[i].replace, "No", GUI.skin.button, GUILayout.MaxWidth(100));
                
                else              
                    _foundObjects[i].replace = GUILayout.Toggle(_foundObjects[i].replace, "Yes", GUI.skin.button, GUILayout.MaxWidth(100));          
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        GUILayout.EndScrollView();
        GUILayout.Space(20);

        GUI.color = Color.black;
        var boxRect = new Rect(490, 80, position.width - 500, position.height - 90);
        GUI.Box(boxRect, "", GUI.skin.GetStyle("Helpbox"));
        GUI.color = Color.white;

        var titleStyle = new GUIStyle();
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.normal.textColor = Color.white;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.fontSize = 14;
    }

    private void ButtonGUI()
    {
        if (GUI.Button(new Rect(110, 300, 100, 20), "Replace"))
            ReplaceObject();
        
        if (GUI.Button(new Rect(230, 300, 100, 20), "Clear"))   
            ClearSettings();     
    }

    private void TransformGUI()
    {
        var boxRect = new Rect(20, 80, 400, 200);
        GUI.Box(boxRect, "", GUI.skin.GetStyle("Helpbox"));

        GUI.Label(new Rect(boxRect.xMin +10, boxRect.yMin + 10, 100, 20), "Keep Position ");
        GUI.Label(new Rect(boxRect.xMin + 10, boxRect.yMin + 35, 100, 20), "Keep Rotation ");
        GUI.Label(new Rect(boxRect.xMin + 10, boxRect.yMin + 60, 100, 20), "Keep Scale ");
        GUI.Label(new Rect(boxRect.xMin + 10, boxRect.yMin + 90, 100, 20), "Customize ");
        GUI.Label(new Rect(boxRect.xMin + 10, boxRect.yMin + 115, 100, 20), "Replace All");

        _replaceAll = EditorGUI.Toggle(new Rect(boxRect.xMin + 120, boxRect.yMin + 115, 20, 20), _replaceAll);

        if (!_customizeTransform)
        {
            _keepPosition = EditorGUI.Toggle(new Rect(boxRect.xMin + 120, boxRect.yMin + 10, 20, 20), _keepPosition);
            _keepRotation = EditorGUI.Toggle(new Rect(boxRect.xMin + 120, boxRect.yMin + 35, 20, 20), _keepRotation);
            _keepScale = EditorGUI.Toggle(new Rect(boxRect.xMin + 120, boxRect.yMin + 60, 20, 20), _keepScale);
        }

        _customizeTransform = EditorGUI.Toggle(new Rect(boxRect.xMin + 120, boxRect.yMin + 90, 20, 20), _customizeTransform);

        if (_customizeTransform)
        {
            var customizeBox = new Rect(boxRect.xMin + 150, boxRect.yMin + 30, 240, 160);
            GUI.Box(customizeBox, "", GUI.skin.window);

            _keepPosition = false;
            _keepRotation = false;
            _keepScale = false;

            GUI.enabled = false;

            _keepPosition = EditorGUI.Toggle(new Rect(boxRect.xMin + 120, boxRect.yMin + 10, 20, 20), _keepPosition);
            _keepRotation = EditorGUI.Toggle(new Rect(boxRect.xMin + 120, boxRect.yMin + 35, 20, 20), _keepRotation);
            _keepScale = EditorGUI.Toggle(new Rect(boxRect.xMin + 120, boxRect.yMin + 60, 20, 20), _keepScale);

            GUI.enabled = true;

            _UseSameValuesForAll = EditorGUI.Toggle(new Rect(boxRect.xMin + 170, boxRect.yMin + 35, 20, 20), "Use For All", _UseSameValuesForAll);
            _customPosition = EditorGUI.Vector3Field(new Rect(boxRect.xMin + 170, boxRect.yMin + 60, 200, 100), "Custom Position: ", _customPosition);
            _customRotation = EditorGUI.Vector3Field(new Rect(boxRect.xMin + 170, boxRect.yMin + 100, 200, 100), "Custom Rotation: ", _customRotation);
            _customScale = EditorGUI.Vector3Field(new Rect(boxRect.xMin + 170, boxRect.yMin + 140, 200, 100), "Custom Rotation: ", _customScale);
        }
    }

    /*
    private void SetTransform()
    {
        for (int i = 0; i < _foundObjects.Count; i++)
        {
            if (_foundObjects[i].selected)
                _foundObjects[i].active = true;
            
            else           
                _foundObjects[i].active = false;

            if (_foundObjects[i].active)
            {
                _foundObjects[i].position = _customPosition;
                _foundObjects[i].rotation = _customRotation;
                _foundObjects[i].scale = _customScale;

                Debug.Log(_foundObjects[i].rotation);
            }

            if(_foundObjects[i].position !=)
        }
    }
    */

    private void ReplaceObject()
    {
        for (int i = _foundObjects.Count - 1; i >= 0; i--)
        {
            if (_foundObjects[i].replace || _replaceAll)
            {
                var newObject = Instantiate(_replacementGameObject, _keepPosition ? _foundObjects[i].transform.position : _customPosition, _keepRotation ? _foundObjects[i].transform.rotation : Quaternion.Euler(_customRotation));
                newObject.transform.localScale = _keepScale ? _foundObjects[i].transform.localScale : _customScale;

                DestroyImmediate(_foundObjects[i].GO.gameObject);
                _foundObjects.Remove(_foundObjects[i]);
            }
        }
    }

    private void ClearSettings()
    {
        _replacementGameObject = null;
        _searchForString = string.Empty;
        _customizeTransform = false;
        _scrollView = Vector2.zero;
        _keepRotation = false;
        _keepPosition = false;
        _searchByIndex = 0;
        _keepScale = false;

        _foundObjectButtons.Clear();
        _foundObjects.Clear();
        Repaint();
    }

    private void GetSearchResult(int index)
    {
        _foundObjects.Clear();
        _foundObjectButtons.Clear();
        var gameObjects = FindObjectsOfType<GameObject>();

        switch (index)
        {
            case 0:

                foreach (var obj in gameObjects)
                {
                    if (obj.name.Contains(_searchForString))
                    {
                        var TrackObj = new RObject(obj);
                        _foundObjects.Add(TrackObj);
                    }
                }                   
                return;

            case 1:
                foreach (var obj in gameObjects)
                {
                    if (obj.GetComponent(_searchForString))
                    {
                        var TrackObj = new RObject(obj);
                        _foundObjects.Add(TrackObj);
                    }
                }
                return;

            case 2:
                foreach(var obj in gameObjects)
                {
                    if (obj.CompareTag(_searchForString))
                    {
                        var TrackObj = new RObject(obj);
                        _foundObjects.Add(TrackObj);
                    }
                }
                return;

            case 3:

                var GameObjects = Selection.gameObjects;
                foreach (var obj in GameObjects)
                {
                    if (obj.name.Contains(_searchForString))
                    {
                        var TrackObj = new RObject(obj);
                        _foundObjects.Add(TrackObj);
                    }
                }
                return;
        }
    }
}