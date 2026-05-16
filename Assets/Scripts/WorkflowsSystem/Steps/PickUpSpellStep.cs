using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

public class PickUpSpellStep : IStep
{
    // --- Variables ---
    private bool _isComplete = false;
    private ICharacterService _characterService;

    // --- IStep ---
    public LocalizedString Name { get => new LocalizedString { TableReference = "Steps", TableEntryReference = "pickUpSpellName" }; }
    public LocalizedString Description
    {
        get
        {
            var pickUpAction = PlayerInputManager.Actions.Player.Interact;
            var keyNames = string.Join(" ", pickUpAction.controls.Select(c => c.displayName));

            return
                new LocalizedString
                {
                    TableReference = "Steps",
                    TableEntryReference = "pickUpSpellDesc",
                    Arguments = new object[] { keyNames }
                };
        }
    }

    public bool IsComplete { get => this._isComplete; set => this._isComplete = value; }
    public event Action OnComplete;

    public PickUpSpellStep()
    {
    }


    public void Deactivate()
    {
        PlayerInputManager.Actions.Player.Interact.started -= HandleAction;
    }

    public void Activate()
    {
        var action = PlayerInputManager.Actions.Player.Interact;
        action.started += HandleAction;
        Debug.Log($"Suscrito. Listeners: {action.GetType()}");
    }

    private void HandleAction(InputAction.CallbackContext context)
    {
        _characterService = AppContainer.Get<ICharacterService>();
        if (_characterService.getSpell(_characterService.getIndex()) == null) return;
        this.IsComplete = true;
        this.OnComplete?.Invoke();
    }
}
