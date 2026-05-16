using TMPro;
using UnityEngine;

public class SpellBookPageController : MonoBehaviour
{
    [SerializeField] private Spellimportance importance;
    [SerializeField] private TextMeshPro textoNombre;
    [SerializeField] private TextMeshPro textoMana;
    private ICharacterService _characterService;
    private IEventService _eventService;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _characterService = AppContainer.Get<ICharacterService>();
        _eventService = AppContainer.Get<IEventService>();
    }
    private void OnEnable()
    {
        _eventService.Subscribe<SpellChangeOnPageEvent>(ChangePageSpell);
    }
    private void OnDisable()
    {
        _eventService.Unsubscribe<SpellChangeOnPageEvent>(ChangePageSpell);
    }
    private void ChangePageSpell(GameEventBase parameters)
    {

        SpellChangeOnPageEvent param = (SpellChangeOnPageEvent)parameters;
        if(param.importance == importance)
        {
            textoNombre.text = param.nombre;
            textoMana.text = param.mana.ToString();
        }
        
    }
}
