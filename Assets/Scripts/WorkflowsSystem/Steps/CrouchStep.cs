using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
/// <summary>
/// Paso que se activa cuando el jugador pulsa el boton de agacharse
/// </summary>
public class CrouchStep : IStep
{
    // --- Variables ---
    private bool _isComplete = false;
    private int _keyPressedTimes = 0;

    // --- IStep ---
    public LocalizedString Name {get => new LocalizedString { TableReference = "Steps", TableEntryReference = "crouchName" };}
    public LocalizedString Description
    {
        get
        {
            var crouchAction = PlayerInputManager.Actions.Player.Crouch;
            var keyNames = string.Join(" ", crouchAction.controls.Select(c => c.displayName));

            return
                new LocalizedString
                {
                    TableReference = "Steps",
                    TableEntryReference = "crouchDesc",
                    Arguments = new object[] { keyNames }
                };
        }
    }
    public bool IsComplete { get => this._isComplete; set => this._isComplete = value; }
    public event Action OnComplete;

    public CrouchStep()
    {
    }

    public void Activate()
    {
        Debug.Log($"Activamos {this.Name}");
        Debug.Log($"{this.Description}");
        PlayerInputManager.Actions.Player.Crouch.performed += HandleAction;

    }

    public void Deactivate()
    {
        PlayerInputManager.Actions.Player.Crouch.performed -= HandleAction;
    }

    private void HandleAction(InputAction.CallbackContext context)
    {
        this.IsComplete = true;
        this.OnComplete?.Invoke();
    }
}
