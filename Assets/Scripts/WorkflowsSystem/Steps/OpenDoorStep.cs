using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

public class OpenDoorStep : IStep
{

    // --- Variables ---
    private bool _isComplete = false;
    private GameObject _door;

    // --- IStep ---
    public LocalizedString Name { get => new LocalizedString { TableReference = "Steps", TableEntryReference = "openDoorName" }; }
    public LocalizedString Description
    {
        get
        {
            var openAction = PlayerInputManager.Actions.Player.Attack;
            var keyNames = string.Join(" ", openAction.controls.Select(c => c.displayName));

            return
                new LocalizedString
                {
                    TableReference = "Steps",
                    TableEntryReference = "openDoorDesc",
                    Arguments = new object[] { keyNames }
                };
        }
    }

    //public string Description => "Presionando la tecla " + PlayerInputManager.Actions.Player.Move.controls.ToString() +  " superas el `step` del workflow";
    public bool IsComplete { get => this._isComplete; set => this._isComplete = value; }
    public event Action OnComplete;

    public OpenDoorStep(GameObject door)
    {
        _door = door;
    }

    public void Activate()
    {
        Debug.Log($"Activamos {this.Name}");
        Debug.Log($"{this.Description}");
        PlayerInputManager.Actions.Player.Attack.performed += HandleAction;

    }

    public void Deactivate()
    {
        PlayerInputManager.Actions.Player.Attack.performed -= HandleAction;
    }

    private void HandleAction(InputAction.CallbackContext context)
    {
        CoroutineRunner.Instance.StartCoroutine(CheckforDoorOpen());

    }

    private IEnumerator CheckforDoorOpen()
    {
        yield return new WaitForSeconds(1);

        if (_door.transform.rotation.z != -180) yield return null;
        this.IsComplete = true;
        this.OnComplete?.Invoke();
    }
}
