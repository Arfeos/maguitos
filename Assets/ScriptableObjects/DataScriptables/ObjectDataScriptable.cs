using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "ObjectDataScriptable", menuName = "Scriptable Objects/ObjectDataScriptable")]
public class ObjectDataScriptable : ScriptableObject
{
    //TODO: Revisar campos necesarios
    public LocalizedString objectName;
    public LocalizedString objetDescription;
    public Sprite objectSprite;
}
