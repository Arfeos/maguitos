using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrouchStep : IStep
{
    // --- Variables ---
    private bool _isComplete = false;
    private int _keyPressedTimes = 0;

    // --- IStep ---
    public string Name => "Paso por debajo del pilar caido";
    public string Description
    {
        get
        {
            var moveAction = PlayerInputManager.Actions.Player.Crouch;
            var keyNames = string.Join(", ", moveAction.controls.Select(c => c.displayName));

            return $"Acercate al primer pilar caido y mientras te mueves hacia delante presiona la tecla {keyNames}";
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
