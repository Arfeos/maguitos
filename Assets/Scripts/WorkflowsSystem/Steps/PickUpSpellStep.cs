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
    public LocalizedString Name {get => new LocalizedString { TableReference = "Steps", TableEntryReference = "mouseMove" };}
    public LocalizedString Description
    {
        get => new LocalizedString { TableReference = "Steps", TableEntryReference = "mouseMove" };
        /* {
            var moveAction = PlayerInputManager.Actions.Player.Interact;
            var keyNames = string.Join(", ", moveAction.controls.Select(c => c.displayName));

            return $"Acercate a la mesa y manten pulsado {keyNames} sobre el libro para obtener el hechizo";
        } */
    }

    public bool IsComplete { get => this._isComplete; set => this._isComplete = value; }
    public event Action OnComplete;

    public PickUpSpellStep()
    {
    }


    public void Deactivate()
    {
        PlayerInputManager.Actions.Player.Interact.performed -= HandleAction;
    }

    public void Activate()
    {
        var action = PlayerInputManager.Actions.Player.Interact;
        action.performed += HandleAction;
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
