using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpSpellStep : IStep
{
    // --- Variables ---
    private bool _isComplete = false;
    private ICharacterService _characterService;

    // --- IStep ---
    public string Name => "Recoge el hechizo";
    public string Description
    {
        get
        {
            var moveAction = PlayerInputManager.Actions.Player.Interact;
            var keyNames = string.Join(", ", moveAction.controls.Select(c => c.displayName));

            return $"Acercate a la mesa y pulsa {keyNames} sobre el libro para obtener el hechizo";
        }
    }

    //public string Description => "Presionando la tecla " + PlayerInputManager.Actions.Player.Move.controls.ToString() +  " superas el `step` del workflow";
    public bool IsComplete { get => this._isComplete; set => this._isComplete = value; }
    public event Action OnComplete;

    public PickUpSpellStep()
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
        _characterService = AppContainer.Get<CharacterService>();
        if (_characterService.getSpell(_characterService.getIndex()) == null) return;
        this.IsComplete = true;
        this.OnComplete?.Invoke();
    }
}
