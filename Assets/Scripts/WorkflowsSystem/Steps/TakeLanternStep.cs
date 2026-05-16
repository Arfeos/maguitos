using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

public class TakeLanternStep : IStep
{
    // --- Variables ---
    private bool _actionComplete = false;
    private bool _isComplete = false;

    // --- IStep ---
    public LocalizedString Name { get => new LocalizedString { TableReference = "Steps", TableEntryReference = "takeLanternName" }; }
    public LocalizedString Description
    {
        get
        {
            var takeAction = PlayerInputManager.Actions.Player.Interact;
            var interactKeyName = string.Join(", ", takeAction.controls.Select(c => c.displayName));
            var onOffAction = PlayerInputManager.Actions.Player.Lantern;
            var onOffKeyName = string.Join(", ", onOffAction.controls.Select(c => c.displayName));

            return
                new LocalizedString
                {
                    TableReference = "Steps",
                    TableEntryReference = "takeLanternDesc",
                    Arguments = new object[] { interactKeyName, onOffKeyName }
                };
        }
    }

    public bool IsComplete { get => this._isComplete; set => this._isComplete = value; }
    public event Action OnComplete;

    public TakeLanternStep()
    {
    }


    public void Deactivate()
    {
        PlayerInputManager.Actions.Player.Interact.performed -= HandleAction1;
        PlayerInputManager.Actions.Player.Lantern.performed -= HandleAction2;
    }

    public void Activate()
    {
        var action1 = PlayerInputManager.Actions.Player.Interact;
        var action2 = PlayerInputManager.Actions.Player.Lantern;

        action1.performed += HandleAction1;
        action2.performed += HandleAction2;

        Debug.Log($"Suscrito. Listeners: {action1.GetType()} y {action2.GetType()}");
    }

    private void HandleAction1(InputAction.CallbackContext context)
    {
        _actionComplete = true;
    }

    private void HandleAction2(InputAction.CallbackContext context)
    {
        if (_actionComplete)
        {
            this.IsComplete = true;
            this.OnComplete?.Invoke();
        }
    }
}
