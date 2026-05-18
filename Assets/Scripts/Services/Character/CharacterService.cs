using System.Collections.Generic;
using UnityEngine;

public class CharacterService : ICharacterService
{
    private IEventService _eventService;
    private List<SpellBase> listaHechizos = new List<SpellBase>();
    private int index = 0;
    private int slots = 10;
    private int life = 100;
    private int Curretlife = 100;
    private int mana = 100;
    private int manaActual = 100;

    private PlayerNetworkHealth _networkHealth;

    private PlayerNetworkHealth GetNetworkHealth()
    {
        if (_networkHealth == null)
            _networkHealth = GameObject.FindFirstObjectByType<PlayerNetworkHealth>();
        return _networkHealth;
    }

    public void TakeDamage(int damageTaken)
    {
        if (_eventService == null) _eventService = AppContainer.Get<IEventService>();

        var networkHealth = GetNetworkHealth();
        if (networkHealth != null)
        {
            networkHealth.TakeDamageRpc(damageTaken);
            return;
        }
        Curretlife -= damageTaken;
        PublishHPEvent();
        if (Curretlife < 1) Die();
    }

    public void Heal(int amountHealed)
    {
        if (_eventService == null) _eventService = AppContainer.Get<IEventService>();

        var networkHealth = GetNetworkHealth();
        if (networkHealth != null)
        {
            networkHealth.HealRpc(amountHealed);
            return;
        }

        if (Curretlife + amountHealed > life) Curretlife = life;
        else Curretlife += amountHealed;
        PublishHPEvent();
    }

    public void SyncHealth(int newHealth)
    {
        Curretlife = newHealth;
        PublishHPEvent();
        if (Curretlife < 1) Die();
    }

    private void PublishHPEvent()
    {
        HPEvent hpEvent = new HPEvent();
        hpEvent.HPToChange = Curretlife;
        _eventService.Publish(hpEvent);
    }

    //public void Heal(int amountHealed)
    //{
    //    if (_eventService == null) _eventService = AppContainer.Get<IEventService>();
    //    if (Curretlife + amountHealed > life) Curretlife = life;
    //    else Curretlife += amountHealed;
    //    HPEvent hpEvent = new HPEvent();
    //    hpEvent.HPToChange = Curretlife;
    //    _eventService.Publish(hpEvent);
    //}
    //public void TakeDamage(int damageTaken)
    //{
    //    if (_eventService == null) _eventService = AppContainer.Get<IEventService>();
    //    this.Curretlife -= damageTaken;
    //    HPEvent hpEvent = new HPEvent();
    //    hpEvent.HPToChange = Curretlife;
    //    _eventService.Publish(hpEvent);
    //    if (this.Curretlife < 1) Die();
    //}

    public void AddMana(int manaAniadir)
    {
        if (manaActual + manaAniadir > mana)
        {
            manaActual = mana;
            return;
        }
        this.manaActual += manaAniadir;
        if(_eventService == null) _eventService = AppContainer.Get<IEventService>();
        
        ManaEvent ManaEvent = new ManaEvent();
        ManaEvent.ManaToChange = manaActual;
        _eventService.Publish(ManaEvent);

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
        if (_eventService == null) _eventService = AppContainer.Get<IEventService>();
        if (manaActual - manaToRemove < 0) return false;
        else
        {
            this.manaActual -= manaToRemove;
            ManaEvent ManaEvent = new ManaEvent();
            ManaEvent.ManaToChange = manaActual;
            _eventService.Publish(ManaEvent);
            return true;
        }


    }
    public bool addSpell(SpellBase spellToAdd)
    {
        spellToAdd.ResetSpellShot();
        //Esto seria para un sistema futuro de spell slots
        //if (CheckSpellCapacity() < spellToAdd.spell.CosteSlots)
        //{
        //    Debug.Log("Lista de hechizos llena");
        //    return false;
        //}
        if (getSpell(spellToAdd.spell.nombreHechizo) != null) return false;
        if (spellToAdd.spell.spell_importance == Spellimportance.primary)
        {
            List<SpellBase> listaDeHechizosPrimarios = getPrimarySpell();
            if(listaDeHechizosPrimarios.Count == 0)
            {
                listaHechizos.Add(spellToAdd);
            }
            else
            {
                foreach (SpellBase spell in listaDeHechizosPrimarios)
                {
                    removeSpell(spell);
                }
                listaHechizos.Add(spellToAdd);
            }
                
        }
        else
        {
            List<SpellBase> listaDeHechizosSecundarios = getSecundarySpell();
            if (listaDeHechizosSecundarios.Count == 0)
            {
                listaHechizos.Add(spellToAdd);
            }
            else
            {
                foreach (SpellBase spell in listaDeHechizosSecundarios)
                {
                    removeSpell(spell);
                }
                listaHechizos.Add(spellToAdd);
            }
                
        }
        if (_eventService == null) _eventService = AppContainer.Get<IEventService>();
        SpellChangeOnPageEvent _eventSpellChangeOnPageEvent = new SpellChangeOnPageEvent();
        _eventSpellChangeOnPageEvent.nombre = spellToAdd.spell.name;
        _eventSpellChangeOnPageEvent.mana = spellToAdd.spell.manaCost;
        _eventSpellChangeOnPageEvent.importance = spellToAdd.spell.spell_importance;
        _eventService.Publish(_eventSpellChangeOnPageEvent);
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

    public List<SpellBase> getPrimarySpell()
    {
        List<SpellBase> listaDeHechizosPrimarios = new List<SpellBase>();
        if (listaHechizos.Count == 0) return listaDeHechizosPrimarios;
        foreach (SpellBase hechizoEnLista in listaHechizos)
        {
            if (hechizoEnLista.spell.spell_importance == Spellimportance.primary)  listaDeHechizosPrimarios.Add(hechizoEnLista);
        }

        return listaDeHechizosPrimarios;
    }

    public List<SpellBase> getSecundarySpell()
    {
        List<SpellBase> listaDeHechizosSecundarios = new();
        if (listaHechizos.Count == 0) return listaDeHechizosSecundarios;
        foreach (SpellBase hechizoEnLista in listaHechizos)
        {
            if (hechizoEnLista.spell.spell_importance == Spellimportance.secundary) listaDeHechizosSecundarios.Add(hechizoEnLista);
        }

        return listaDeHechizosSecundarios;
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

    public void Die()
    {
        //TODO: Implemetar muerte real
        Debug.Log("Has muerto");
    }

}
