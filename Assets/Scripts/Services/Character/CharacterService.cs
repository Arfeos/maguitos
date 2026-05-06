using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CharacterService : ICharacterService
{
    private List<SpellBase> listaHechizos = new List<SpellBase>();
    private int index = 0;
    private int slots = 10;
    private int life = 100;
    private int Curretlife = 100;
    private int mana = 100;
    private int manaActual = 100;
    public void AddMana(int manaAniadir)
    {
        if (manaActual + manaAniadir > mana)
        {
            manaActual = mana;
            return;
        }
        this.manaActual += manaAniadir;
    }
    public int getMaxMana()
    {
        return mana;
    }
    public int getIndex()
    {
        return index;
    }
    public void setActualSpell(int cambioDePosicion)
    {

        index += cambioDePosicion;
        if (index < 0) index = listaHechizos.Count - 1;
        if (index > listaHechizos.Count -1 ) index = 0;
        Debug.Log("Hechizo actual: " + listaHechizos[index].name);
    }

    public int CheckMana()
    {
        return manaActual;
    }
    public bool RemoveMana(int manaToRemove)
    {
        if (manaActual - manaToRemove < 0) return false;
        else
        {
            this.manaActual -= manaToRemove;
            return true;
        }
    }
    public bool addSpell(SpellBase spellToAdd)
    {
        spellToAdd.ResetSpellShot();
        if (CheckSpellCapacity() < spellToAdd.spell.CosteSlots)
        {
            Debug.Log("Lista de hechizos llena");
            return false;
        }
        if (getSpell(spellToAdd.spell.nombreHechizo) != null) return false;
        listaHechizos.Add(spellToAdd);
        return true;
    }

    public int CheckSpellCapacity()
    {
        int costeActualHechizos = 0;
        foreach (SpellBase hechizoEnLista in listaHechizos)
        {
            costeActualHechizos += hechizoEnLista.spell.CosteSlots;
        }
        return slots - costeActualHechizos;
    }


    public SpellBase getSpell(string spellName)
    {
        if(spellName == null || spellName.Equals("") || spellName.Equals("Change Spell name")) return null;

        foreach (SpellBase hechizoEnLista in listaHechizos)
        {
            if(hechizoEnLista.spell.nombreHechizo.Equals(spellName)) return hechizoEnLista;
        }

        return null;
    }

    public SpellBase getSpell(int spellPosition)
    {
        if(spellPosition < 0) return null;
        if(spellPosition + 1 > listaHechizos.Count) return null;
        if(listaHechizos.Count == 0) return null;
        return listaHechizos[spellPosition];
    }


    public bool removeSpell(SpellBase spellToRemove)
    {
        if (spellToRemove == null) return false;
        if(!listaHechizos.Contains(spellToRemove)) return false;
        listaHechizos.Remove(spellToRemove);
        return true;
    }

    public bool removeSpell(int spellToRemove)
    {
        if (spellToRemove < 0) return false;
        if (spellToRemove + 1 > listaHechizos.Count) return false;
        listaHechizos.RemoveAt(spellToRemove);
        return true;
    }

    public bool removeSpell(string spellToRemove)
    {
        if (spellToRemove == null || spellToRemove.Equals("") || spellToRemove.Equals("Change Spell name")) return false;

        for(int i = 0; listaHechizos.Count >= i ;i++){ 
            if (listaHechizos[i].spell.nombreHechizo.Equals(spellToRemove))
            {
                listaHechizos.RemoveAt(i);
                return true;
            }
        }

        return false;
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
