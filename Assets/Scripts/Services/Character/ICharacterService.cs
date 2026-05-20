using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterService
{
    public int getIndex();
    public void setActualSpell(int cambioDePosicion);
    public bool addSpell(SpellBase spellToAdd);
    public bool removeSpell(SpellBase spellToRemove);
    public bool removeSpell(int spellToRemove);
    public void ResetCharacter();
    public bool removeSpell(string spellToRemove);
    public SpellBase getSpell(string spellName);
    public SpellBase getSpell(int spellPosition);
    public List<SpellBase> getPrimarySpell();
    public List<SpellBase> getSecundarySpell();
    public void AddMana(int mana);
    public bool RemoveMana(int mana);
    public int CheckMana();
    public int getMaxMana();
    public int CheckSpellCapacity();
    public void TakeDamage(int damageTaken);
    public void Heal(int amountHealed);
    public void SyncHealth(int newHealth);

    public void Die();
}
