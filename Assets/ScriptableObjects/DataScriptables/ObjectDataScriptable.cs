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

    public string GetTypeKey() => spellData.spell_Type switch
    {
        SpellBase.SpellType.ray => "typeRay",
        SpellBase.SpellType.ball => "typeBall",
        SpellBase.SpellType.buff => "typeBuff",
        SpellBase.SpellType.structure => "typeStructure",
        _ => "typeRay"
    };

    public string GetImportanceKey() => spellData.spell_importance switch
    {
        Spellimportance.primary => "importance1",
        Spellimportance.secundary => "importance2",
        _ => "importance1"
    };
}
