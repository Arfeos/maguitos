using UnityEngine;

[CreateAssetMenu(fileName = "ObjectDataScriptable", menuName = "Scriptable Objects/ObjectDataScriptable")]
public class ObjectDataScriptable : ScriptableObject
{
    //TODO: Revisar campos necesarios
    public string objectName;
    public string objetDescription;
    public Sprite objectSprite;
}
