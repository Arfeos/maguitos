using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
/// <summary>
/// Clase encargada de implementar un paso dentro del sistema de tutorial mediante la interfaz <see cref="IStep"/>. 
/// Detecta cuándo el jugador realiza una acción de recarga utilizando <see cref="PlayerInputManager"/> y marca el paso como completado cuando dicha acción es ejecutada
/// </summary>
public class ReloadStep : IStep
{
    // --- Variables ---
    private bool _isComplete = false;


    // --- IStep ---
    /// <summary>
    /// Propiedad que devuelve el nombre localizado correspondiente al paso actual mediante el sistema de localización de Unity
    /// </summary>
    public LocalizedString Name {get => new LocalizedString { TableReference = "Steps", TableEntryReference = "reloadName" };}
    /// <summary>
    /// Propiedad que genera una descripción localizada del paso incluyendo dinámicamente la tecla o entrada asignada a la acción de recarga obtenida desde <see cref="PlayerInputManager"/>
    /// </summary>
    public LocalizedString Description
    {
        get
        {
            var reloadAction = PlayerInputManager.Actions.Player.Reload;
            var keyNames = string.Join(" ", reloadAction.controls.Select(c => c.displayName));

            return
                new LocalizedString
                {
                    TableReference = "Steps",
                    TableEntryReference = "reloadDesc",
                    Arguments = new object[] { keyNames }
                };
        }
    }
    /// <summary>
    /// Propiedad que almacena el estado actual del paso indicando si ha sido completado
    /// </summary>
    /// <returns>Estado de finalización del paso</returns>
    public bool IsComplete { get => this._isComplete; set => this._isComplete = value; }
    /// <summary>
    /// Evento ejecutado cuando el paso se completa correctamente
    /// </summary>
    public event Action OnComplete;
    /// <summary>
    /// Constructor encargado de crear una nueva instancia del paso de recarga
    /// </summary>
    public ReloadStep()
    {
    }
    /// <summary>
    /// Activa el paso actual registrando el método HandleAction() al evento de recarga de <see cref="PlayerInputManager"/> para detectar la interacción del jugador
    /// </summary>
    public void Activate()
    {
        Debug.Log($"Activamos {this.Name}");
        Debug.Log($"{this.Description}");
        PlayerInputManager.Actions.Player.Reload.performed += HandleAction;

    }
    /// <summary>
    /// Desactiva el paso eliminando la suscripción al evento de recarga de <see cref="PlayerInputManager"/>
    /// </summary>
    public void Deactivate()
    {
        PlayerInputManager.Actions.Player.Reload.performed -= HandleAction;
    }
    /// <summary>
    /// Método ejecutado cuando el jugador realiza una acción de recarga. Marca el paso como completado y ejecuta el evento OnComplete
    /// </summary>
    /// <param name="context">Información asociada a la acción realizada por el jugador</param>
    private void HandleAction(InputAction.CallbackContext context)
    {
        this.IsComplete = true;
        this.OnComplete?.Invoke();
    }
}
