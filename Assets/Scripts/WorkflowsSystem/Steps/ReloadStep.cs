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
    public LocalizedString Name {get => new LocalizedString { TableReference = "Steps", TableEntryReference = "reloadName" };}
    public LocalizedString Description
    {
        get
        {
            var reloadAction = PlayerInputManager.Actions.Player.Reload;
            var keyNames = string.Join(" ", reloadAction.controls.Select(c => c.displayName));

            return
                new LocalizedString
                {
                    TableReference = "Steps",
                    TableEntryReference = "reloadDesc",
                    Arguments = new object[] { keyNames }
                };
        }
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
