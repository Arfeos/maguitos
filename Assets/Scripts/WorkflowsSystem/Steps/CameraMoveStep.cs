using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
/// <summary>
/// Clase encargada de implementar un paso dentro de un sistema de tutorial o guía interactiva mediante la interfaz <see cref="IStep"/>. 
/// Detecta el movimiento de la cámara a través de <see cref="PlayerInputManager"/> y marca el paso como completado cuando el jugador realiza la acción correspondiente
/// </summary>
public class CameraMoveStep : IStep
{

    // --- Variables ---
    private bool _isComplete = false;


    // --- IStep ---
    /// <summary>
    /// Propiedad que devuelve el nombre localizado del paso utilizando el sistema de localización de Unity
    /// </summary>
    ///<returns>Nombre localizado asociado al paso actual.</returns>
    public LocalizedString Name { get => new LocalizedString { TableReference = "Steps", TableEntryReference = "cameraMoveName" }; }
    /// <summary>
    /// Propiedad que devuelve una descripción localizada del paso utilizando el sistema de localización de Unity
    /// </summary>
    /// <returns>Descripción localizada asociada al paso.</returns>
    public LocalizedString Description
    {
        get => new LocalizedString { TableReference = "Steps", TableEntryReference = "cameraMoveDesc" };
        /* {
            var moveAction = PlayerInputManager.Actions.Player.Look;
            var keyNames = string.Join(", ", moveAction.controls.Select(c => c.displayName));
            if (keyNames.Contains("Delta")) keyNames = "el ratón";
            
            return
                new LocalizedString
                {
                    TableReference = "Steps",
                    TableEntryReference = "cameraMoveDesc",
                    Arguments = new object[] { keyNames }
                };
        } */
    }
    /// <summary>
    /// Propiedad que almacena el estado actual del paso indicando si este ha sido completado o no
    /// </summary>
    /// <returns>Estado de finalización del paso</returns>
    public bool IsComplete { get => this._isComplete; set => this._isComplete = value; }
    /// <summary>
    /// Evento lanzado cuando el paso se completa correctamente
    /// </summary>
    public event Action OnComplete;
    /// <summary>
    /// Constructor de la clase encargado de crear una nueva instancia del paso de movimiento de cámara
    /// </summary>
    public CameraMoveStep()
    {
    }
    /// <summary>
    /// Activa el paso actual y registra el método HandleAction() al evento de movimiento de cámara de <see cref="PlayerInputManager"/> para detectar la interacción del jugador
    /// </summary>
    public void Activate()
    {
        Debug.Log($"Activamos {this.Name}");
        Debug.Log($"{this.Description}");
        PlayerInputManager.Actions.Player.Look.performed += HandleAction;
    }
    /// <summary>
    /// Desactiva el paso eliminando la suscripción al evento de movimiento de cámara de <see cref="PlayerInputManager"/>
    /// </summary>
    public void Deactivate()
    {
        PlayerInputManager.Actions.Player.Look.performed -= HandleAction;
    }
    /// <summary>
    /// Método ejecutado cuando el jugador realiza una acción de movimiento de cámara. Marca el paso como completado y lanza el evento OnComplete
    /// </summary>
    /// <param name="context">Información asociada a la acción realizada por el jugador</param>
    private void HandleAction(InputAction.CallbackContext context)
    {
        this.IsComplete = true;
        this.OnComplete?.Invoke();
    }
}
