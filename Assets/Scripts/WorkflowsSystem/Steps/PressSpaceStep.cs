using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

public class PressSpaceStep : IStep
{

    // --- Variables ---
    private bool _isComplete = false;
    private int _keyPressedTimes = 0;

    // --- IStep ---
    public LocalizedString Name {get => new LocalizedString { TableReference = "Steps", TableEntryReference = "jumpName" };}
    public LocalizedString Description
    {
        get
        {
            var jumpAction = PlayerInputManager.Actions.Player.Jump;
            var keyNames = string.Join(" ", jumpAction.controls.Select(c => c.displayName));

            return
                new LocalizedString
                {
                    TableReference = "Steps",
                    TableEntryReference = "jumpDesc",
                    Arguments = new object[] { keyNames }
                };
        }
    }
    public bool IsComplete { get => this._isComplete; set => this._isComplete = value; }
    public event Action OnComplete;

    public PressSpaceStep()
    {
    }

    public void Activate()
    {
        Debug.Log($"Activamos {this.Name}");
        Debug.Log($"{this.Description}");
        PlayerInputManager.Actions.Player.Jump.performed += HandleAction;

    }

    public void Deactivate()
    {
        PlayerInputManager.Actions.Player.Jump.performed -= HandleAction;
    }

    private void HandleAction(InputAction.CallbackContext context)
    {
        this.IsComplete = true;
        this.OnComplete?.Invoke();
    }
}
