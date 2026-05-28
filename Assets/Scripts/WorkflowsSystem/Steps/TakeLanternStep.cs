using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

/// <summary>
/// Paso del tutorial que enseña al jugador a recoger y encender la linterna.
/// Se completa en dos fases: primero interactuar para recogerla y después
/// activarla con su tecla correspondiente.
/// </summary>
public class TakeLanternStep : IStep
{
    // --- Variables ---
    /// <summary>Indica si el jugador ya ha completado la primera fase del paso (recoger la linterna).</summary>
    private bool _actionComplete = false;
    /// <summary>Indica si el paso completo ha sido superado.</summary>
    private bool _isComplete = false;

    // --- IStep ---
    /// <summary>
    /// Nombre localizado del paso, obtenido de la tabla <c>Steps</c> con la clave <c>takeLanternName</c>.
    /// </summary>
    public LocalizedString Name { get => new LocalizedString { TableReference = "Steps", TableEntryReference = "takeLanternName" }; }
    /// <summary>
    /// Descripción localizada del paso, construida dinámicamente con los nombres de las teclas
    /// de interacción y de la linterna detectadas en los controles activos del jugador.
    /// </summary>
    public LocalizedString Description
    {
        get
        {
            var takeAction = PlayerInputManager.Actions.Player.Interact;
            var interactKeyName = string.Join(" ", takeAction.controls.Select(c => c.displayName));
            var onOffAction = PlayerInputManager.Actions.Player.Lantern;
            var onOffKeyName = string.Join(" ", onOffAction.controls.Select(c => c.displayName));

            return
                new LocalizedString
                {
                    TableReference = "Steps",
                    TableEntryReference = "takeLanternDesc",
                    Arguments = new object[] { interactKeyName, onOffKeyName }
                };
        }
    }
    /// <summary>
    /// Indica si el paso ha sido completado. Al establecerse a <c>true</c> se considera superado.
    /// </summary>
    public bool IsComplete { get => this._isComplete; set => this._isComplete = value; }
    /// <summary>
    /// Evento invocado cuando el jugador completa ambas fases del paso.
    /// </summary>
    public event Action OnComplete;

    // <summary>
    /// Inicializa una nueva instancia del paso sin parámetros adicionales.
    /// </summary>
    public TakeLanternStep()
    {
    }

    // <summary>
    /// Desactiva el paso cancelando la suscripción a las acciones de interacción y linterna.
    /// </summary>
    public void Deactivate()
    {
        PlayerInputManager.Actions.Player.Interact.started -= HandleAction1;
        PlayerInputManager.Actions.Player.Lantern.started -= HandleAction2;
    }
    /// <summary>
    /// Activa el paso suscribiéndose a las acciones de interacción y linterna del jugador.
    /// </summary>
    public void Activate()
    {
        var action1 = PlayerInputManager.Actions.Player.Interact;
        var action2 = PlayerInputManager.Actions.Player.Lantern;

        action1.started += HandleAction1;
        action2.started += HandleAction2;

        Debug.Log($"Suscrito. Listeners: {action1.GetType()} y {action2.GetType()}");
    }
    /// <summary>
    /// Callback de la primera fase: marca que el jugador ha interactuado para recoger la linterna.
    /// </summary>
    /// <param name="context">Contexto del input recibido.</param>
    private void HandleAction1(InputAction.CallbackContext context)
    {
        _actionComplete = true;
    }
    /// <summary>
    /// Callback de la segunda fase: si la primera fase ya fue completada,
    /// marca el paso como superado e invoca <see cref="OnComplete"/>.
    /// </summary>
    /// <param name="context">Contexto del input recibido.</param>
    private void HandleAction2(InputAction.CallbackContext context)
    {
        if (_actionComplete)
        {
            this.IsComplete = true;
            this.OnComplete?.Invoke();
        }
    }
}
