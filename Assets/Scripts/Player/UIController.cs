using TMPro;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject HP_Slider;
    [SerializeField] GameObject Mana_Slider;
    [SerializeField] TMP_Text HP_text;
    [SerializeField] TMP_Text Mana_text;
    private IEventService _EventService;
    private void OnEnable()
    {
        _EventService.Subscribe<HPEvent>(ChangeHPUi);
        _EventService.Subscribe<ManaEvent>(ChangeManaUi);
    }
    private void OnDisable()
    {
        _EventService.Unsubscribe<HPEvent>(ChangeHPUi);
        _EventService.Unsubscribe<ManaEvent>(ChangeManaUi);
    }
    private void Awake()
    {
        _EventService = AppContainer.Get<IEventService>();
    }

    public void ChangeManaUi(GameEventBase evento)
    {
        ManaEvent manaEvent = (ManaEvent)evento;

        Mana_text.text = manaEvent.ManaToChange.ToString();
        Mana_Slider.GetComponent<Slider>().value = manaEvent.ManaToChange;
    }
    public void ChangeHPUi(GameEventBase evento)
    {
        HPEvent hpEvent = (HPEvent)evento;

        HP_text.text = hpEvent.HPToChange.ToString();
        HP_Slider.GetComponent<Slider>().value = hpEvent.HPToChange;
    }

  
}
