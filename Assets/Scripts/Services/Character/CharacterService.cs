using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;
/// <summary>
/// Servicio encargado de gestionar toda la lógica relacionada con el personaje, incluyendo salud, maná, inventario de hechizos, selección de habilidades y estados especiales. 
/// Se comunica con <see cref="IEventService"/> para notificar cambios y utiliza objetos <see cref="SpellBase"/> para administrar los hechizos del jugador
/// </summary>
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
    private bool _pacifista = true;
    /// <summary>
    /// Constructor encargado de obtener una referencia al servicio <see cref="IEventService"/> mediante <see cref="AppContainer"/>
    /// </summary>
    public CharacterService()
    {
        _eventService = AppContainer.Get<IEventService>();
    }
    /// <summary>
    /// Añade una cantidad de maná al personaje y publica un evento <see cref="ManaEvent"/> mediante <see cref="IEventService"/> para actualizar el estado del sistema
    /// </summary>
    /// <param name="manaAniadir">Cantidad de maná que será añadida</param>    
    public void AddMana(int manaAniadir)
    {
        if (manaActual + manaAniadir > mana)
        {
            manaActual = mana;
            return;
        }
        this.manaActual += manaAniadir;
        if (_eventService == null) _eventService = AppContainer.Get<IEventService>();
        
        ManaEvent ManaEvent = new ManaEvent();
        ManaEvent.ManaToChange = manaActual;
        _eventService.Publish(ManaEvent);

    }
    /// <summary>
    /// Obtiene la cantidad máxima de maná disponible para el personaje
    /// </summary>
    /// <returns>Cantidad máxima de maná</returns>
    public int getMaxMana()
    {
        return mana;
    }
    /// <summary>
    /// Obtiene la posición actual del hechizo seleccionado
    /// </summary>
    /// <returns>Índice del hechizo seleccionado</returns>
    public int getIndex()
    {
        return index;
    }
    /// <summary>
    /// Cambia el hechizo seleccionado actualmente desplazándose por la lista de hechizos almacenada
    /// </summary>
    /// <param name="cambioDePosicion">Cantidad de posiciones que se desplazará la selección</param>
    public void setActualSpell(int cambioDePosicion)
    {
        if (listaHechizos.Count <=0) return;
        index += cambioDePosicion;
        if (index < 0) index = listaHechizos.Count - 1;
        if (index > listaHechizos.Count - 1) index = 0;
        Debug.Log("Hechizo actual: " + listaHechizos[index].name);
    }
    /// <summary>
    /// Obtiene la cantidad actual de maná disponible
    /// </summary>
    /// <returns>Maná actual del personaje</returns>
    public int CheckMana()
    {
        return manaActual;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="manaToRemove"></param>
    /// <returns></returns>
    public bool RemoveMana(int manaToRemove)
    {
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
    /// <summary>
    /// Reduce una cantidad de maná y publica un evento <see cref="ManaEvent"/> mediante <see cref="IEventService"/>
    /// </summary>
    /// <param name="spellToAdd">Cantidad de maná que será eliminada</param>
    /// <returns>Devuelve true si la operación se realizó correctamente o false si no hay suficiente maná</returns>
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
            if (listaDeHechizosPrimarios.Count == 0)
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
        SpellChangeOnPageEvent _eventSpellChangeOnPageEvent = new SpellChangeOnPageEvent();
        _eventSpellChangeOnPageEvent.nombre = spellToAdd.spell.objectData.objectName;
        _eventSpellChangeOnPageEvent.mana = spellToAdd.spell.manaCost;
        _eventSpellChangeOnPageEvent.spellSprite = spellToAdd.spell.spellImage;
        _eventSpellChangeOnPageEvent.importance = spellToAdd.spell.spell_importance;
        _eventService.Publish(_eventSpellChangeOnPageEvent);
        return true;
    }
    /// <summary>
    /// Calcula el espacio disponible restante para almacenar hechizos
    /// </summary>
    /// <returns>Espacio restante disponible</returns>
    public int CheckSpellCapacity()
    {
        int costeActualHechizos = 0;
        foreach (SpellBase hechizoEnLista in listaHechizos)
        {
            costeActualHechizos += hechizoEnLista.spell.CosteSlots;
        }
        return slots - costeActualHechizos;
    }
    /// <summary>
    /// Obtiene todos los hechizos principales almacenados en la lista de objetos <see cref="SpellBase"/>
    /// </summary>
    /// <returns>Lista de hechizos principales</returns>
   
    public List<SpellBase> getPrimarySpell()
    {
        List<SpellBase> listaDeHechizosPrimarios = new List<SpellBase>();
        if (listaHechizos.Count == 0) return listaDeHechizosPrimarios;
        foreach (SpellBase hechizoEnLista in listaHechizos)
        {
            if (hechizoEnLista.spell.spell_importance == Spellimportance.primary) listaDeHechizosPrimarios.Add(hechizoEnLista);
        }

        return listaDeHechizosPrimarios;
    }
    /// <summary>
    /// Obtiene todos los hechizos secundarios almacenados en la lista de objetos <see cref="SpellBase"/>
    /// </summary>
    /// <returns>Lista de hechizos secundarios</returns>
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
    /// <summary>
    /// Busca y devuelve un objeto <see cref="SpellBase"/> a partir de su nombre
    /// </summary>
    /// <param name="spellName">Nombre del hechizo</param>
    /// <returns>Hechizo encontrado o null</returns>
    public SpellBase getSpell(string spellName)
    {
        if (spellName == null || spellName.Equals("") || spellName.Equals("Change Spell name")) return null;

        foreach (SpellBase hechizoEnLista in listaHechizos)
        {
            if (hechizoEnLista.spell.nombreHechizo.Equals(spellName)) return hechizoEnLista;
        }

        return null;
    }
    /// <summary>
    /// Obtiene un objeto <see cref="SpellBase"/> según su posición dentro de la lista
    /// </summary>
    /// <param name="spellPosition">Posición del hechizo</param>
    /// <returns>Hechizo encontrado o null</returns>
    public SpellBase getSpell(int spellPosition)
    {
        if (spellPosition < 0) return null;
        if (spellPosition + 1 > listaHechizos.Count) return null;
        if (listaHechizos.Count == 0) return null;
        return listaHechizos[spellPosition];
    }
    /// <summary>
    /// Elimina un objeto <see cref="SpellBase"/> de la lista de hechizos
    /// </summary>
    /// <param name="spellToRemove">Hechizo que será eliminado</param>
    /// <returns>Devuelve true si se eliminó correctamente</returns>
    public bool removeSpell(SpellBase spellToRemove)
    {
        if (spellToRemove == null) return false;
        if (!listaHechizos.Contains(spellToRemove)) return false;
        listaHechizos.Remove(spellToRemove);
        return true;
    }
    /// <summary>
    /// Elimina un hechizo según su posición dentro de la lista
    /// </summary>
    /// <param name="spellToRemove">Posición del hechizo a eliminar</param>
    /// <returns>Devuelve true si la operación fue correcta</returns>
    public bool removeSpell(int spellToRemove)
    {
        if (spellToRemove < 0) return false;
        if (spellToRemove + 1 > listaHechizos.Count) return false;
        listaHechizos.RemoveAt(spellToRemove);
        return true;
    }
    /// <summary>
    /// Elimina un objeto<see cref = "SpellBase" /> utilizando su nombre
    /// </summary>
    /// <param name="spellToRemove">Nombre del hechizo que será eliminado</param>
    /// <returns>Devuelve true si se eliminó correctamente</returns>
    public bool removeSpell(string spellToRemove)
    {
        if (spellToRemove == null || spellToRemove.Equals("") || spellToRemove.Equals("Change Spell name")) return false;

        for (int i = 0; listaHechizos.Count >= i; i++)
        {
            if (listaHechizos[i].spell.nombreHechizo.Equals(spellToRemove))
            {
                listaHechizos.RemoveAt(i);
                return true;
            }
        }

        return false;
    }
    /// <summary>
    /// Restaura salud al personaje y publica un evento <see cref="HPEvent"/> mediante <see cref="IEventService"/>
    /// </summary>
    /// <param name="amountHealed">Cantidad de vida recuperada</param>
    public void Heal(int amountHealed)
    {
        if (Curretlife + amountHealed > life) Curretlife = life;
        else Curretlife += amountHealed;
        HPEvent hpEvent = new HPEvent();
        hpEvent.HPToChange = Curretlife;
        _eventService.Publish(hpEvent);
    }
    /// <summary>
    /// Reduce la vida actual del personaje y publica un evento <see cref="HPEvent"/> mediante <see cref="IEventService"/>. Si la vida llega a cero ejecuta Die()
    /// </summary>
    /// <param name="damageTaken">Cantidad de daño recibido</param>
    public void TakeDamage(int damageTaken)
    {
        this.Curretlife -= damageTaken;
        HPEvent hpEvent = new HPEvent();
        hpEvent.HPToChange = Curretlife;
        _eventService.Publish(hpEvent);
        if (this.Curretlife < 1) Die();
    }
    /// <summary>
    /// Gestiona la muerte del personaje publicando un evento <see cref="DieEvent"/> mediante <see cref="IEventService"/>
    /// </summary>
    public void Die()
    {
        _eventService.Publish(new DieEvent());

    }
    /// <summary>
    /// Restaura todos los valores del personaje a su estado inicial, reinicia los hechizos almacenados y publica eventos <see cref="HPEvent"/> y <see cref="ManaEvent"/> mediante <see cref="IEventService"/>
    /// </summary>
    public void ResetCharacter()
    {
        Curretlife = life;
        manaActual = mana;
        _pacifista = true;
        listaHechizos = new List<SpellBase>();
        index = 0;
        HPEvent hpEvent = new HPEvent();
        hpEvent.HPToChange = Curretlife;
        _eventService.Publish(hpEvent);
        ManaEvent ManaEvent = new ManaEvent();
        ManaEvent.ManaToChange = manaActual;
        _eventService.Publish(ManaEvent);
    }
    /// <summary>
    /// Desactiva el estado pacifista del personaje
    /// </summary>
    public void Genocide()
    {
        _pacifista = false;
    }
    /// <summary>
    /// Obtiene el estado pacifista actual del personaje
    /// </summary>
    /// <returns>Devuelve true si el personaje continúa siendo pacifista</returns>
    public bool getPacifist()
    {
        return _pacifista;
    }
}
