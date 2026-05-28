using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Define el contrato para gestionar el estado del personaje jugador:
/// hechizos equipados, maná, vida y estadísticas de partida.
/// </summary>
public interface ICharacterService
{
    /// <summary>
    /// Devuelve el índice del hechizo actualmente seleccionado.
    /// </summary>
    public int getIndex();

    /// <summary>
    /// Cambia el hechizo activo desplazando la selección la cantidad indicada.
    /// </summary>
    /// <param name="cambioDePosicion">Desplazamiento positivo o negativo sobre la lista de hechizos.</param>
    public void setActualSpell(int cambioDePosicion);

    /// <summary>
    /// Añade un hechizo al inventario del personaje.
    /// </summary>
    /// <param name="spellToAdd">Hechizo a añadir.</param>
    /// <returns><c>true</c> si se añadió correctamente; <c>false</c> si no hay hueco o ya estaba equipado.</returns>
    public bool addSpell(SpellBase spellToAdd);

    /// <summary>
    /// Elimina un hechizo del inventario por referencia.
    /// </summary>
    /// <param name="spellToRemove">Hechizo a eliminar.</param>
    /// <returns><c>true</c> si se eliminó correctamente; <c>false</c> si no se encontró.</returns>
    public bool removeSpell(SpellBase spellToRemove);

    /// <summary>
    /// Elimina el hechizo situado en la posición indicada del inventario.
    /// </summary>
    /// <param name="spellToRemove">Índice del hechizo a eliminar.</param>
    /// <returns><c>true</c> si se eliminó correctamente; <c>false</c> si el índice no es válido.</returns>
    public bool removeSpell(int spellToRemove);

    /// <summary>
    /// Elimina el hechizo con el nombre indicado del inventario.
    /// </summary>
    /// <param name="spellToRemove">Nombre del hechizo a eliminar.</param>
    /// <returns><c>true</c> si se encontró y eliminó; <c>false</c> si no existe ningún hechizo con ese nombre.</returns>
    public bool removeSpell(string spellToRemove);

    /// <summary>
    /// Restaura el personaje a su estado inicial (vida, maná, hechizos, etc.).
    /// </summary>
    public void ResetCharacter();

    /// <summary>
    /// Devuelve el hechizo equipado con el nombre indicado.
    /// </summary>
    /// <param name="spellName">Nombre del hechizo a buscar.</param>
    /// <returns>El hechizo encontrado, o <c>null</c> si no existe.</returns>
    public SpellBase getSpell(string spellName);

    /// <summary>
    /// Devuelve el hechizo situado en la posición indicada del inventario.
    /// </summary>
    /// <param name="spellPosition">Índice del hechizo a obtener.</param>
    /// <returns>El hechizo en esa posición, o <c>null</c> si el índice no es válido.</returns>
    public SpellBase getSpell(int spellPosition);

    /// <summary>
    /// Devuelve la lista de hechizos primarios equipados.
    /// </summary>
    public List<SpellBase> getPrimarySpell();

    /// <summary>
    /// Devuelve la lista de hechizos secundarios equipados.
    /// </summary>
    public List<SpellBase> getSecundarySpell();

    /// <summary>
    /// Añade maná al personaje sin superar el máximo.
    /// </summary>
    /// <param name="mana">Cantidad de maná a añadir.</param>
    public void AddMana(int mana);

    /// <summary>
    /// Consume maná del personaje si hay suficiente.
    /// </summary>
    /// <param name="mana">Cantidad de maná a consumir.</param>
    /// <returns><c>true</c> si había suficiente maná y se consumió; <c>false</c> en caso contrario.</returns>
    public bool RemoveMana(int mana);

    /// <summary>
    /// Devuelve el maná actual del personaje.
    /// </summary>
    public int CheckMana();

    /// <summary>
    /// Devuelve el maná máximo del personaje.
    /// </summary>
    public int getMaxMana();

    /// <summary>
    /// Devuelve el número de huecos de hechizo disponibles.
    /// </summary>
    public int CheckSpellCapacity();

    /// <summary>
    /// Aplica daño al personaje, reduciendo su vida.
    /// </summary>
    /// <param name="damageTaken">Cantidad de daño a aplicar.</param>
    public void TakeDamage(int damageTaken);

    /// <summary>
    /// Cura al personaje, aumentando su vida sin superar el máximo.
    /// </summary>
    /// <param name="amountHealed">Cantidad de vida a restaurar.</param>
    public void Heal(int amountHealed);
    public void SyncHealth(int newHealth);

    /// <summary>
    /// Ejecuta la secuencia de muerte del personaje.
    /// </summary>
    public void Die();

    /// <summary>
    /// Registra que el jugador ha usado un hechizo, invalidando el logro pacifista si procede.
    /// </summary>
    public void Genocide();

    /// <summary>
    /// Indica si el jugador ha completado la partida sin usar ningún hechizo.
    /// </summary>
    /// <returns><c>true</c> si el jugador es pacifista; <c>false</c> si ha usado un hechizo.</returns>
    public bool getPacifist();
}