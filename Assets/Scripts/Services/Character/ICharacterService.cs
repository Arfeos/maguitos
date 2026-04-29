using NUnit.Framework;
using UnityEngine;

public interface ICharacterService
{
    public int getIndex();
    public bool addSpell(SpellBase spellToAdd);
    public bool removeSpell(SpellBase spellToRemove);
    public bool removeSpell(int spellToRemove);
    public bool removeSpell(string spellToRemove);
    public SpellBase getSpell(string spellName);
    public SpellBase getSpell(int spellPosition);
    public void AddMana(int mana);
    public bool RemoveMana(int mana);
    public int CheckMana();
    public int getMaxMana();
    public int CheckSpellCapacity();
    public void TakeDamage(int damageTaken);
    public void Heal(int amountHealed);

    public void Die();

}
