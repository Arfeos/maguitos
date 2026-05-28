using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// MonoBehaviour que controla los elementos de UI relacionados con las estadísticas del jugador,
/// actualizando los sliders y textos de vida y maná en respuesta a los eventos del juego.
/// </summary>
public class UIController : MonoBehaviour
{
    /// <summary>GameObject que contiene el <see cref="Slider"/> de vida del jugador.</summary>
    [SerializeField] GameObject HP_Slider;
    /// <summary>GameObject que contiene el <see cref="Slider"/> de maná del jugador.</summary>
    [SerializeField] GameObject Mana_Slider;
    /// <summary>Texto que muestra el valor numérico actual de la vida del jugador.</summary>
    [SerializeField] TMP_Text HP_text;
    /// <summary>Texto que muestra el valor numérico actual del maná del jugador.</summary>
    [SerializeField] TMP_Text Mana_text;
    /// <summary>Referencia al servicio de eventos para suscribirse y desuscribirse a los eventos de HP y maná.</summary>
    private IEventService _EventService;

    /// <summary>
    /// Se suscribe a los eventos <see cref="HPEvent"/> y <see cref="ManaEvent"/>
    /// al activarse el componente.
    /// </summary>
    private void OnEnable()
    {
        _EventService.Subscribe<HPEvent>(ChangeHPUi);
        _EventService.Subscribe<ManaEvent>(ChangeManaUi);
    }

    /// <summary>
    /// Cancela la suscripción a los eventos <see cref="HPEvent"/> y <see cref="ManaEvent"/>
    /// al desactivarse el componente, evitando llamadas a métodos sobre objetos destruidos.
    /// </summary>
    private void OnDisable()
    {
        _EventService.Unsubscribe<HPEvent>(ChangeHPUi);
        _EventService.Unsubscribe<ManaEvent>(ChangeManaUi);
    }

    /// <summary>
    /// Obtiene la referencia al <see cref="IEventService"/> al inicializarse el componente.
    /// </summary>
    private void Awake()
    {
        _EventService = AppContainer.Get<IEventService>();
    }

    /// <summary>
    /// Actualiza el slider y el texto de maná con el valor recibido en el <see cref="ManaEvent"/>.
    /// </summary>
    /// <param name="evento">Evento de tipo <see cref="ManaEvent"/> con el nuevo valor de maná.</param>
    public void ChangeManaUi(GameEventBase evento)
    {
        ManaEvent manaEvent = (ManaEvent)evento;

        Mana_text.text = manaEvent.ManaToChange.ToString();
        Mana_Slider.GetComponent<Slider>().value = manaEvent.ManaToChange;
    }

    /// <summary>
    /// Actualiza el slider y el texto de vida con el valor recibido en el <see cref="HPEvent"/>.
    /// </summary>
    /// <param name="evento">Evento de tipo <see cref="HPEvent"/> con el nuevo valor de vida.</param>
    public void ChangeHPUi(GameEventBase evento)
    {
        HPEvent hpEvent = (HPEvent)evento;

        HP_text.text = hpEvent.HPToChange.ToString();
        HP_Slider.GetComponent<Slider>().value = hpEvent.HPToChange;
    }

  
}
