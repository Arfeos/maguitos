using TMPro;
using UnityEngine;
/// <summary>
/// Componente de Unity encargado de controlar y actualizar la información mostrada en una página del libro de hechizos. 
/// Gestiona la visualización del nombre, coste de maná e imagen del hechizo mediante eventos recibidos desde <see cref="IEventService"/> utilizando objetos <see cref="SpellChangeOnPageEvent"/>
/// </summary>
public class SpellBookPageController : MonoBehaviour
{
    /// <summary>
    /// Variable serializada que determina qué categoría de hechizo mostrará la página actual utilizando la enumeración <see cref="Spellimportance"/>
    /// </summary>
    [SerializeField] private Spellimportance importance;
    /// <summary>
    /// Referencia al componente encargado de mostrar el nombre del hechizo
    /// </summary>
    [SerializeField] private TextMeshPro textoNombre;
    /// <summary>
    /// Referencia al componente encargado de mostrar el coste de maná del hechizo
    /// </summary>
    [SerializeField] private TextMeshPro textoMana;
    /// <summary>
    /// Referencia al componente visual encargado de mostrar la imagen asociada al hechizo
    /// </summary>
    [SerializeField] private SpriteRenderer spellImage;
    private ICharacterService _characterService;
    private IEventService _eventService;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /// <summary>
    /// Método ejecutado durante la inicialización del objeto. Obtiene referencias a los servicios <see cref="ICharacterService"/> y <see cref="IEventService"/> mediante <see cref="AppContainer"/> y busca automáticamente el componente visual encargado de mostrar la imagen del hechizo
    /// </summary>
    void Awake()
    {
        _characterService = AppContainer.Get<ICharacterService>();
        _eventService = AppContainer.Get<IEventService>();

        spellImage = this.GetComponentInChildren<SpriteRenderer>(true);
    }
    /// <summary>
    /// Método ejecutado cuando el objeto se activa. Registra el método ChangePageSpell() como suscriptor del evento <see cref="SpellChangeOnPageEvent"/> mediante <see cref="IEventService"/>
    /// </summary>
    private void OnEnable()
    {
        _eventService.Subscribe<SpellChangeOnPageEvent>(ChangePageSpell);
    }
    /// <summary>
    /// Método ejecutado cuando el objeto se desactiva. Elimina la suscripción al evento <see cref="SpellChangeOnPageEvent"/> mediante <see cref="IEventService"/>
    /// </summary>
    private void OnDisable()
    {
        _eventService.Unsubscribe<SpellChangeOnPageEvent>(ChangePageSpell);
    }
    /// <summary>
    /// Método encargado de actualizar la información visual de la página cuando se recibe un evento <see cref="SpellChangeOnPageEvent"/>. 
    /// Si la importancia del hechizo coincide con la página actual, actualiza el nombre localizado, el coste de maná y la imagen asociada
    /// </summary>
    /// <param name="parameters">Evento recibido que contiene información del hechizo. Se convierte internamente a <see cref="SpellChangeOnPageEvent"/></param>
    private void ChangePageSpell(GameEventBase parameters)
    {

        SpellChangeOnPageEvent param = (SpellChangeOnPageEvent)parameters;
        if (param.importance == importance)
        {
            param.nombre.GetLocalizedStringAsync().Completed += handle =>
                    {
                        textoNombre.text = handle.Result;
                    };

            textoMana.text = param.mana.ToString();

            if (param.spellSprite != null)
                spellImage.sprite = param.spellSprite;
        }

    }
}
