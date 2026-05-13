
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

public class MoveStep : IStep
{

    // --- Variables ---
    private bool _isComplete = false;


    // --- IStep ---
    public LocalizedString Name { get => new LocalizedString { TableReference = "Steps", TableEntryReference = "playerMoveName" }; }
    public LocalizedString Description
    {
        get  => new LocalizedString { TableReference = "Steps", TableEntryReference = "playerMoveDesc" };
        /* {
            var moveAction = PlayerInputManager.Actions.Player.Move;
            var keyNames = string.Join(", ", moveAction.controls.Select(c => c.displayName));

            return $"Presionando las teclas {keyNames}";
        } */
    }

    //public string Description => "Presionando la tecla " + PlayerInputManager.Actions.Player.Move.controls.ToString() +  " superas el `step` del workflow";
    public bool IsComplete { get => this._isComplete; set => this._isComplete = value; }
    public event Action OnComplete;

    public MoveStep()
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
