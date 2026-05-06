
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PressAStep : IStep
{

    // --- Variables ---
    private bool _isComplete = false;


    // --- IStep ---
    public string Name => "Presiona A";
    public string Description => "Presionando la tecla `A` superas el `step` del workflow";
    public bool IsComplete { get => this._isComplete; set => this._isComplete = value; }
    public event Action OnComplete;

    public PressAStep()
    {
    }

    public void Activate()
    {
        Debug.Log($"Activamos {this.Name}");
        Debug.Log($"{this.Description}");
        PlayerInputManager.Actions.Player.Move.performed += HandleAction;

    }

    public void Deactivate()
    {
        PlayerInputManager.Actions.Player.Move.performed -= HandleAction;
    }

    private void HandleAction(InputAction.CallbackContext context)
    {
        this.IsComplete = true;
        this.OnComplete?.Invoke();
    }
}
