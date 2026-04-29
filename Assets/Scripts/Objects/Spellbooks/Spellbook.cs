using System;
using UnityEngine;

public class Spellbook : MonoBehaviour, ICollectable
{
    [SerializeField] private SpellBase Spell;
    private ICharacterService _characterService;
    public void Collect()
    {
        if(_characterService == null) _characterService = AppContainer.Get<ICharacterService>();
        _characterService.addSpell(Spell);
    }
}