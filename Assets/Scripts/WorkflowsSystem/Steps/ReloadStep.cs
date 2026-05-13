using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

public class ReloadStep : IStep
{
    // --- Variables ---
    private bool _isComplete = false;


    // --- IStep ---
    public LocalizedString Name {get => new LocalizedString { TableReference = "Steps", TableEntryReference = "mouseMove" };}
    public LocalizedString Description
    {
        get => new LocalizedString { TableReference = "Steps", TableEntryReference = "mouseMove" };
        /* {
            var moveAction = PlayerInputManager.Actions.Player.Reload;
            var keyNames = string.Join(", ", moveAction.controls.Select(c => c.displayName));
            return $"Pulsa la tecla {keyNames} para recargar tu mana";
        } */
    }

    public bool IsComplete { get => this._isComplete; set => this._isComplete = value; }
    public event Action OnComplete;

    public ReloadStep()
    {
    }

    public void Activate()
    {
        Debug.Log($"Activamos {this.Name}");
        Debug.Log($"{this.Description}");
        PlayerInputManager.Actions.Player.Reload.performed += HandleAction;

    }

    public void Deactivate()
    {
        PlayerInputManager.Actions.Player.Reload.performed -= HandleAction;
    }

    private void HandleAction(InputAction.CallbackContext context)
    {
        this.IsComplete = true;
        this.OnComplete?.Invoke();
    }
}
