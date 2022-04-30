using UnityEngine;

/// <summary>
/// 
/// An object created when searching for gameObjects in the hierarchy.
/// It allows individual tracking and editing.
/// 
/// </summary>
public class RObject
{
    public GameObject GO;
    public Transform transform;
    public bool replace;
    public bool selected;
    public bool active;
    public string name;

    public bool hasEditedTransform;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    public RObject(GameObject GO)
    {
        this.GO = GO;
        transform = GO.transform;
        name = GO.name;
    }

    public bool OnClick()
    {
        return true;
    }
}