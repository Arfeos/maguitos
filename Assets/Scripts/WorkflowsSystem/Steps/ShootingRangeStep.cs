using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

public class ShootingRangeStep : IStep
{
    private GameObject _messageBox;
    private IScoreService _scoreService;
    private IAlertService _alertService;
    private IEventService _eventService;
    // --- Variables ---
    private bool _isComplete = false;
    public int puntos = 0;

    // --- IStep ---
    public LocalizedString Name {get => new LocalizedString { TableReference = "Steps", TableEntryReference = "shootingRangeName" };}
    public LocalizedString Description
    {
        set { }
        get
        {
            var attackAction = PlayerInputManager.Actions.Player.Attack;
            var keyNames = string.Join(" ", attackAction.controls.Select(c => c.displayName));

            return
                new LocalizedString
                {
                    TableReference = "Steps",
                    TableEntryReference = "shootingRangeDesc",
                    Arguments = new object[] { keyNames, puntos }
                };
        }
    }

    public bool IsComplete { get => this._isComplete; set => this._isComplete = value; }
    public event Action OnComplete;

    public ShootingRangeStep(GameObject MessageBox)
    {
        _messageBox = MessageBox;
    }

    public void Activate()
    {
        Debug.Log($"Activamos {this.Name}");
        Debug.Log($"{this.Description}");

        _scoreService = AppContainer.Get<IScoreService>();
        _alertService = AppContainer.Get<IAlertService>();
        _eventService = AppContainer.Get<IEventService>();
        _eventService.Subscribe<ScoreChangeEvent>(actualizarPuntos);
        puntos = _scoreService.GetPoints("TutorialPlayer");
        PlayerInputManager.Actions.Player.Attack.performed += HandleAction;

    }

    public void Deactivate()
    {
        PlayerInputManager.Actions.Player.Attack.performed -= HandleAction;
    }

    public void actualizarPuntos(GameEventBase parameters)
    {
        puntos = _scoreService.GetPoints("TutorialPlayer");
        ObjectDataScriptable data = new ObjectDataScriptable();
        data.objectName = Name;
        data.objetDescription = Description;
        _alertService.ShowAlertMessage(_messageBox, data);
        if (puntos >= 500)
        {
            this.IsComplete = true;
            this.OnComplete?.Invoke();
        }
    }

    private void HandleAction(InputAction.CallbackContext context)
    {

    }
}