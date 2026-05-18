using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "ObjectDataScriptable", menuName = "Scriptable Objects/ObjectDataScriptable")]
public class ObjectDataScriptable : ScriptableObject
{
    //TODO: Revisar campos necesarios
    public LocalizedString objectName;
    public LocalizedString objetDescription;
    public Sprite objectSprite;

    // Referencia al hechizo para mostrar sus stats
    public SpellBaseScriptable spellData;

    // Contructor para la LocalizedString de stats si hay hechizo asignado
    public LocalizedString GetStatsDescription()
    {
        if (spellData == null) return objetDescription;

        return new LocalizedString
        {
            TableReference = "InfoPanel",
            TableEntryReference = "spellStats",
            Arguments = new object[]
            {
                spellData.damage,
                spellData.manaCost,
                spellData.lifeTime
            }
        };
    }
}
