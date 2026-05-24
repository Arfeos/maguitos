using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
/// <summary>
/// Paso que se completa cuando se recoge un orbve de mana y otro de vida
/// </summary>
public class TakeOrbsStep : IStep
{
    // --- Variables ---
    private bool _isComplete = false;
    private GameObject _hpOrb;
    private GameObject _manaOrb;
    private bool _isManaRecovered = false;
    private bool _isHpRecovered = false;

    private IEventService _eventService;
    // --- IStep ---
    public LocalizedString Name {get => new LocalizedString { TableReference = "Steps", TableEntryReference = "TakeOrbName" };}
    public LocalizedString Description { get => new LocalizedString { TableReference = "Steps", TableEntryReference = "TakeOrbDesc" }; }

    public bool IsComplete { get => this._isComplete; set => this._isComplete = value; }
    public event Action OnComplete;

    public TakeOrbsStep(GameObject hpOrb, GameObject manaOrb)
    {
        _hpOrb = hpOrb;
        _manaOrb = manaOrb;
    }

    public void Activate()
    {
        Debug.Log($"Activamos {this.Name}");
        Debug.Log($"{this.Description}");

        _eventService = AppContainer.Get<IEventService>();
        PlayerInputManager.Actions.Player.Move.started += HandleAction;
    }

    public void Deactivate()
    {
        PlayerInputManager.Actions.Player.Move.started -= HandleAction;
    }

    private void checkMana()
    {
        if (_manaOrb.gameObject == null) {
            _isManaRecovered = true;
        }

    }
    private void checkHP()
    {
        if (_hpOrb.gameObject == null)
        {
            _isHpRecovered = true;
        }
    }
    private void HandleAction(InputAction.CallbackContext context)
    {
        checkMana();
        checkHP();
        if (_isHpRecovered && _isManaRecovered)
        {
            this.IsComplete = true;
            this.OnComplete?.Invoke();
        }
        
    }
}
