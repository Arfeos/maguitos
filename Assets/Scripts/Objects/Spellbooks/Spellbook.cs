using System;
using UnityEngine;

public class Spellbook : MonoBehaviour, ICollectable
{
    [SerializeField] private SpellBase Spell;
    private ICharacterService _characterService;
    public void Collect()
    {
        _characterService.addSpell(Spell);
    }

    private void Awake()
    {
        _characterService = AppContainer.Get<CharacterService>();
    }
}