using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingRangeStep : IStep
{
    private GameObject _messageBox;
    private IScoreService _scoreService;
    private IAlertService _alertService;
    // --- Variables ---
    private bool _isComplete = false;
    public int puntos = 0;

    // --- IStep ---
    public string Name => "Sal fuera y consigue 500 puntos";
    public string Description
    {
        set { }
        get
        {
            var moveAction = PlayerInputManager.Actions.Player.Attack;
            var keyNames = string.Join(", ", moveAction.controls.Select(c => c.displayName));
            return $"Dispara al glifo del cartel usando {keyNames} para iniciar el minujuego de campo de tiro." +
                $"Intenta conseguir 500 puntos disparando a las dianas con tus hechizos, puedes cambiar de hechizos en la mesa de atrás" +
                $" {puntos}/500";
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

        puntos = _scoreService.GetPoints("TutorialPlayer");
        PlayerInputManager.Actions.Player.Attack.performed += HandleAction;

    }

    public void Deactivate()
    {
        PlayerInputManager.Actions.Player.Attack.performed -= HandleAction;
    }

    private void HandleAction(InputAction.CallbackContext context)
    {
        //TODO revisar lo del player aqui tambien, esto habra que cambiarlo cuando mas de 1 jugador pueda jugar
        puntos = _scoreService.GetPoints("TutorialPlayer");
        var moveAction = PlayerInputManager.Actions.Player.Attack;
        var keyNames = string.Join(", ", moveAction.controls.Select(c => c.displayName));
        Description = $"Dispara al glifo del cartel usando {keyNames} para iniciar el minujuego de campo de tiro." +
            $"Intenta conseguir 500 puntos disparando a las dianas con tus hechizos, puedes cambiar de hechizos en la mesa de atrás" +
            $" {puntos}/500";
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
}
