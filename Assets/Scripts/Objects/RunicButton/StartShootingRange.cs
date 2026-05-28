using UnityEngine;

/// <summary>
/// MonoBehaviour que implementa <see cref="IHittable"/> para actuar como disparador
/// del rango de tiro del tutorial. Al recibir un impacto, publica un <see cref="TutorialGameEvent"/>
/// para notificar al sistema que el jugador ha iniciado esta fase.
/// </summary>
public class StartShootingRange : MonoBehaviour, IHittable
{
    /// <summary>Referencia al servicio de eventos para publicar el <see cref="TutorialGameEvent"/> al recibir un impacto.</summary>
    private IEventService _eventService;

    /// <summary>
    /// Llamado al recibir un impacto. Publica un <see cref="TutorialGameEvent"/> para
    /// notificar que el jugador ha activado el rango de tiro del tutorial.
    /// El parámetro <paramref name="damage"/> no se utiliza en esta implementación.
    /// </summary>
    /// <param name="damage">Cantidad de daño recibido (no utilizado).</param>
    public void Hit(float damage)
    {
        TutorialGameEvent startTutorial = new TutorialGameEvent();
        _eventService.Publish(startTutorial);
    }

    /// <summary>
    /// Obtiene la referencia al <see cref="IEventService"/> al inicializarse el componente.
    /// </summary>
    private void Awake()
    {
        _eventService = AppContainer.Get<IEventService>();
    }
}
