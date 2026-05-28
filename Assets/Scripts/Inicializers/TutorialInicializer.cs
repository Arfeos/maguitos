
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour encargado de inicializar y gestionar el flujo del tutorial inicial,
/// definiendo la secuencia de pasos que el jugador debe completar antes de acceder al juego.
/// </summary>
public class TutorialInicializer : MonoBehaviour
{
    /// <summary>
    /// Flujo de trabajo que encadena todos los pasos del tutorial en orden.
    /// </summary>
    Workflow mainSceneWorkflow;

    /// <summary>
    /// Referencia al servicio de animaciones, usado para reproducir la animación
    /// de salida del panel de mensajes al finalizar el tutorial.
    /// </summary>
    IAnimationService _animation;

    /// <summary>Panel de mensajes usado para mostrar las instrucciones de cada paso del tutorial.</summary>
    [SerializeField] GameObject MessageBox;
    /// <summary>Puerta que el jugador debe aprender a abrir durante el tutorial.</summary>
    [SerializeField] GameObject door;
    /// <summary>Portal que el jugador debe atravesar para abandonar el tutorial.</summary>
    [SerializeField] Portal_Controller Portal;
    /// <summary>Orbe de vida que el jugador debe recoger durante el tutorial.</summary>
    [SerializeField] GameObject HPOrb;
    /// <summary>Orbe de maná que el jugador debe recoger durante el tutorial.</summary>
    [SerializeField] GameObject ManaOrb;

    /// <summary>
    /// Inicializa el flujo del tutorial y obtiene las referencias a los servicios necesarios.
    /// </summary>
    private void Awake()
    {
        InitWorkflow();
        _animation = AppContainer.Get<IAnimationService>();
    }
    /// <summary>
    /// Construye el <see cref="Workflow"/> con la secuencia completa de pasos del tutorial,
    /// se suscribe al evento de finalización y lo inicia.
    /// </summary>
    private void InitWorkflow()
    {
        mainSceneWorkflow = new Workflow(new List<IStep>
        {
            new CameraMoveStep(),
            new MoveStep(),
            new TakeLanternStep(),
            new PickUpSpellStep(),
            new ShootStep(),
            new OpenDoorStep(door),
            new CrouchStep(),
            new PressSpaceStep(),
            new ReloadStep(),
            new TakeOrbsStep(HPOrb,ManaOrb),
            new ShootingRangeStep(MessageBox),
            new LeaveTutorialStep(Portal),

        }, MessageBox);

        mainSceneWorkflow.OnComplete += WorkflowFinished;

        mainSceneWorkflow.Begin();

        
    }
    /// <summary>
    /// Callback invocado al completarse todos los pasos del tutorial.
    /// Reproduce la animación de fade out sobre el panel de mensajes.
    /// </summary>
    private void WorkflowFinished()
    {
        Debug.Log($"Hemos finalizado el workflow");
        _animation.FadeOutUIAnimation(MessageBox, 2 );
    }
}
