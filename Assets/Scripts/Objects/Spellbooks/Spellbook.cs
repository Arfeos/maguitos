
using UnityEngine;
/// <summary>
/// Componente de Unity encargado de representar un libro de hechizos coleccionable dentro del juego. 
/// Implementa la interfaz <see cref="ICollectable"/> y permite añadir un objeto <see cref="SpellBase"/> al personaje mediante <see cref="ICharacterService"/>
/// </summary>
public class Spellbook : MonoBehaviour, ICollectable
{
    /// <summary>
    /// Variable serializada que almacena el hechizo asociado al libro que será añadido al personaje al recogerlo
    /// </summary>
    [SerializeField] private SpellBase Spell;
    private ICharacterService _characterService;
    /// <summary>
    /// Método ejecutado cuando el objeto es recogido. Obtiene una referencia a <see cref="ICharacterService"/> mediante <see cref="AppContainer"/> y añade el hechizo asociado utilizando un objeto <see cref="SpellBase"/>
    /// </summary>
    public void Collect()
    {
        if(_characterService == null) _characterService = AppContainer.Get<ICharacterService>();
        _characterService.addSpell(Spell);
    }
}