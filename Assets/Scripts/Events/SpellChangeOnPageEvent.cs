using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
/// <summary>
/// Clase de evento derivada de <see cref="GameEventBase"/> utilizada para notificar cambios relacionados con la visualización de un hechizo en la interfaz. 
/// Permite enviar información del hechizo seleccionado a otros sistemas suscritos mediante <see cref="IEventService"/>, como menús o páginas del libro de hechizos
/// </summary>
public class SpellChangeOnPageEvent : GameEventBase
{
    /// <summary>
    /// Variable pública que almacena el nombre localizado del hechizo que será mostrado
    /// </summary>
    public LocalizedString nombre;
    /// <summary>
    /// Variable pública que almacena el coste de maná necesario para utilizar el hechizo
    /// </summary>
    public int mana;
    /// <summary>
    /// Variable pública que almacena la imagen o icono representativo del hechizo
    /// </summary>
    public Sprite spellSprite;
    /// <summary>
    /// Variable pública que almacena la categoría o prioridad del hechizo utilizando la enumeración <see cref="Spellimportance"/>
    /// </summary>
    public Spellimportance importance;
    
}
