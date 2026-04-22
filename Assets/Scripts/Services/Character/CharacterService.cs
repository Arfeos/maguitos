using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CharacterService : ICharacterService
{
    private List<SpellBase> listaHechizos = new List<SpellBase>();
    private int slots = 1;
    private int life = 100;
    private int Curretlife = 100;
    private int mana = 100;
    private int manaActual = 100;
    public void AddMana(int manaAniadir)
    {
        if (manaActual + manaAniadir > mana) return;
        this.manaActual += manaAniadir;
    }

    public int CheckMana()
    {
        return manaActual;
    }
    public void RemoveMana(int manaToRemove)
    {
        if (manaActual - manaToRemove < 0) return;
        this.manaActual -= manaToRemove;

    }
    public bool addSpell(SpellBase spellToAdd)
    {
        if(CheckSpellCapacity() < spellToAdd.SlotCost) return false;

        listaHechizos.Add(spellToAdd);
        return true;
    }

    public int CheckSpellCapacity()
    {
        int costeActualHechizos = 0;
        foreach (SpellBase hechizoEnLista in listaHechizos)
        {
            costeActualHechizos += hechizoEnLista.SlotCost;
        }
        return slots - costeActualHechizos;
    }


    public SpellBase getSpell(string spellName)
    {
        if(spellName == null || spellName.Equals("") || spellName.Equals("Change Spell name")) return null;

        foreach (SpellBase hechizoEnLista in listaHechizos)
        {
            if(hechizoEnLista.Name.Equals(spellName)) return hechizoEnLista;
        }

        return null;
    }

    public SpellBase getSpell(int spellPosition)
    {
        if(spellPosition < 0) return null;
        if(spellPosition + 1 > listaHechizos.Count) return null;
        return listaHechizos[spellPosition];
    }

    public SpellBase getSpell(SpellBase spell)
    {
        if (spell == null) return null;

        foreach (SpellBase hechizoEnLista in listaHechizos)
        {
            if (hechizoEnLista.Name.Equals(spell.Name)) return hechizoEnLista;
        }

        return null ;
    }

    public bool removeSpell(SpellBase spellToRemove)
    {
        if (spellToRemove == null) return false;
        if(!listaHechizos.Contains(spellToRemove)) return false;
        listaHechizos.Remove(spellToRemove);
        return true;
    }
    public void Heal(int amountHealed)
    {
        if (Curretlife + amountHealed > life) Curretlife = life;
        else Curretlife += amountHealed;
    }
    public void TakeDamage(int damageTaken)
    {
        this.Curretlife -= damageTaken;
        if (this.Curretlife < 0) Die();
    }

    public void Die()
    {
        //TODO: Implemetar muerte real
        Debug.Log("Has muerto");
    }
}
