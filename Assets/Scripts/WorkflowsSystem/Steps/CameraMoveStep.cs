using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Tables;

public class CameraMoveStep : IStep
{

    // --- Variables ---
    private bool _isComplete = false;


    // --- IStep ---
    public string Name => "Mueve la camara";
    public string Description
    {
        get
        {
            var moveAction = PlayerInputManager.Actions.Player.Look;
            var keyNames = string.Join(", ", moveAction.controls.Select(c => c.displayName));
            if (keyNames.Contains("Delta")) keyNames = "el rat�n";
            return $"Mueve la camara usando {keyNames}";
        }
    }

    public bool IsComplete { get => this._isComplete; set => this._isComplete = value; }
    public event Action OnComplete;

    public CameraMoveStep()
    {
    }

    public void Activate()
    {
        Debug.Log($"Activamos {this.Name}");
        Debug.Log($"{this.Description}");
        PlayerInputManager.Actions.Player.Look.performed += HandleAction;
    }

    public void Deactivate()
    {
        PlayerInputManager.Actions.Player.Look.performed -= HandleAction;
    }

    private void HandleAction(InputAction.CallbackContext context)
    {
        this.IsComplete = true;
        this.OnComplete?.Invoke();
    }
}
