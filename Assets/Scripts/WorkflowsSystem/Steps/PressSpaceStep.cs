using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
/// <summary>
/// Clase encargada de implementar un paso dentro del sistema de tutorial mediante la interfaz <see cref="IStep"/>. 
/// Detecta cuándo el jugador realiza la acción de salto utilizando <see cref="PlayerInputManager"/> y marca el paso como completado cuando la acción es ejecutada
/// </summary>
public class PressSpaceStep : IStep
{

    // --- Variables ---
    private bool _isComplete = false;
    private int _keyPressedTimes = 0;

    // --- IStep ---
    /// <summary>
    /// Propiedad que devuelve el nombre localizado correspondiente al paso actual mediante el sistema de localización de Unity
    /// </summary>
    public LocalizedString Name {get => new LocalizedString { TableReference = "Steps", TableEntryReference = "jumpName" };}
    /// <summary>
    /// Propiedad que genera una descripción localizada del paso incluyendo dinámicamente la tecla o entrada asignada a la acción de salto obtenida desde <see cref="PlayerInputManager"/>
    /// </summary>
    public LocalizedString Description
    {
        get
        {
            var jumpAction = PlayerInputManager.Actions.Player.Jump;
            var keyNames = string.Join(" ", jumpAction.controls.Select(c => c.displayName));

            return
                new LocalizedString
                {
                    TableReference = "Steps",
                    TableEntryReference = "jumpDesc",
                    Arguments = new object[] { keyNames }
                };
        }
    }
    /// <summary>
    /// Propiedad que almacena el estado actual del paso indicando si ya ha sido completado
    /// </summary>
    /// <returns>Estado de finalización del paso</returns>
    public bool IsComplete { get => this._isComplete; set => this._isComplete = value; }
    /// <summary>
    /// Evento ejecutado cuando el paso se completa correctamente
    /// </summary>
    public event Action OnComplete;
    /// <summary>
    /// Constructor encargado de crear una nueva instancia del paso de salto
    /// </summary>
    public PressSpaceStep()
    {
    }
    /// <summary>
    /// Activa el paso actual registrando el método HandleAction() al evento de salto de <see cref="PlayerInputManager"/> para detectar la acción realizada por el jugador
    /// </summary>
    public void Activate()
    {
        Debug.Log($"Activamos {this.Name}");
        Debug.Log($"{this.Description}");
        PlayerInputManager.Actions.Player.Jump.performed += HandleAction;

    }
    /// <summary>
    /// Desactiva el paso eliminando la suscripción al evento de salto de <see cref="PlayerInputManager"/>
    /// </summary>
    public void Deactivate()
    {
        PlayerInputManager.Actions.Player.Jump.performed -= HandleAction;
    }
    /// <summary>
    /// Método ejecutado cuando el jugador realiza una acción de salto. Marca el paso como completado y ejecuta el evento OnComplete
    /// </summary>
    /// <param name="context">Información asociada a la acción ejecutada por el jugador</param>
    private void HandleAction(InputAction.CallbackContext context)
    {
        this.IsComplete = true;
        this.OnComplete?.Invoke();
    }
}
