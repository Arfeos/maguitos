using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PressSpaceStep : IStep
{

    // --- Variables ---
    private bool _isComplete = false;
    private int _keyPressedTimes = 0;

    // --- IStep ---
    public string Name => "Salta por encima del pilar caido";
    public string Description
    {
        get
        {
            var moveAction = PlayerInputManager.Actions.Player.Jump;
            var keyNames = string.Join(", ", moveAction.controls.Select(c => c.displayName));

            return $"Acercate al segundo pilar caido y mientras te mueves hacia delante presiona la tecla {keyNames}";
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
