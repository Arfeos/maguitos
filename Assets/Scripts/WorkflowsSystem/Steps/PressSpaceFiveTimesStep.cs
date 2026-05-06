using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PressSpaceFiveTimesStep : IStep
{

    // --- Variables ---
    private bool _isComplete = false;
    private int _keyPressedTimes = 0;

    // --- IStep ---
    public string Name => "Presiona SPACE";
    public string Description => "Presionando la tecla `SPACE` 5 veces superas el `step` del workflow";
    public bool IsComplete { get => this._isComplete; set => this._isComplete = value; }
    public event Action OnComplete;

    public PressSpaceFiveTimesStep()
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
        this._keyPressedTimes++;

        Debug.Log($"Pulsada {this._keyPressedTimes}/5 veces");

        if (this._keyPressedTimes >= 5)
        {
            this.IsComplete = true;
            this.OnComplete?.Invoke();

        }
    }
}
