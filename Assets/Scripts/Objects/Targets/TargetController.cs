using UnityEngine;

/// <summary>
/// MonoBehaviour que controla el comportamiento de un blanco del rango de tiro del tutorial.
/// Implementa <see cref="IHittable"/> para reaccionar a los impactos, se mueve horizontalmente
/// rebotando en los límites y suma puntos al jugador cuando el juego está activo.
/// </summary>
public class TargetController : MonoBehaviour, IHittable
{
    /// <summary>Referencia al servicio de animaciones para reproducir la animación y sonido al recibir un impacto.</summary>
    IAnimationService _animationService;
    /// <summary>Referencia al servicio de eventos para suscribirse al <see cref="TutorialGameEvent"/> y publicar <see cref="ScoreChangeEvent"/>.</summary>
    IEventService _eventService;
    /// <summary>Referencia al servicio de puntuación para sumar puntos al acertar el blanco.</summary>
    IScoreService _scoreService;
    /// <summary>Clip de audio que se reproduce al recibir un impacto.</summary>
    [SerializeField]AudioClip audioWhenHit;
    /// <summary>Velocidad mínima de movimiento horizontal del blanco.</summary>
    [SerializeField] int minVelocity;
    /// <summary>Velocidad máxima de movimiento horizontal del blanco.</summary>
    [SerializeField] int maxVelocity;
    /// <summary>Puntos que se otorgan al jugador al acertar este blanco.</summary>
    [SerializeField] int puntos;
    /// <summary>Velocidad actual del blanco, inicializada aleatoriamente entre <see cref="minVelocity"/> y <see cref="maxVelocity"/>.</summary>
    private int velocity;
    /// <summary>Dirección actual del movimiento: 1 para derecha, -1 para izquierda.</summary>
    private int direction = 1;
    /// <summary>Posición inicial del blanco, usada como referencia para el movimiento.</summary>
    private Vector3 InitialPosition;
    /// <summary>Indica si el minijuego del rango de tiro está activo.</summary>
    private bool _isGameStarted = false;

    /// <summary>
    /// Llamado al recibir un impacto. Reproduce la animación y sonido de golpe y,
    /// si el minijuego está activo, suma los puntos definidos al jugador y publica
    /// un <see cref="ScoreChangeEvent"/> con la cantidad obtenida.
    /// </summary>
    /// <param name="damage">Cantidad de daño recibido (no utilizado en esta implementación).</param>
    public void Hit(float damage)
    {
        _animationService.WobbleAnimationWithSound(this.gameObject, audioWhenHit);
        //TODO: esto no es taki taki rumba pero pensando en un multiplayter futuro se queda asi, darle una revision cuando se pueda
        if (_isGameStarted)
        {
            _scoreService.addPoints("TutorialPlayer", puntos);
            ScoreChangeEvent score = new ScoreChangeEvent();
            score.points = puntos;
            _eventService.Publish(score);
        }
    }
    /// <summary>
    /// Obtiene las referencias a los servicios, inicializa la velocidad aleatoria
    /// y guarda la posición inicial del blanco.
    /// </summary>
    private void Awake()
    {
        _animationService = AppContainer.Get<IAnimationService>();
        _eventService = AppContainer.Get<IEventService>();
        _scoreService = AppContainer.Get<IScoreService>();
        velocity = Random.Range(minVelocity, maxVelocity);
        InitialPosition = this.transform.position;
    }

    /// <summary>
    /// Mueve el blanco horizontalmente cada frame si el minijuego está activo.
    /// </summary>
    private void Update()
    {
        if(!_isGameStarted) return;
        Move();
    }

    /// <summary>
    /// Callback invocado al recibir un <see cref="TutorialGameEvent"/>.
    /// Alterna el estado activo del minijuego y, al desactivarse, resetea la puntuación.
    /// </summary>
    /// <param name="parameters">Evento base recibido, correspondiente a un <see cref="TutorialGameEvent"/>.</param>
    public void activateGame(GameEventBase parameters)
    {
        _isGameStarted = !_isGameStarted;
        if (!_isGameStarted)
        {
            _scoreService.resetScore();
        }
    }

    /// <summary>
    /// Se suscribe al <see cref="TutorialGameEvent"/> al activarse el componente.
    /// </summary>
    private void OnEnable()
    {
        _eventService.Subscribe<TutorialGameEvent>(activateGame);
    }

    /// <summary>
    /// Cancela la suscripción al <see cref="TutorialGameEvent"/> al desactivarse el componente.
    /// </summary>
    private void OnDisable()
    {
        _eventService.Unsubscribe<TutorialGameEvent>(activateGame);
    }

    /// <summary>
    /// Mueve el blanco horizontalmente a la velocidad actual y cambia de dirección
    /// al detectar mediante un <see cref="Physics.Raycast"/> una colisión con un objeto
    /// etiquetado como <c>BounceZone</c>.
    /// </summary>
    private void Move()
    {
        float step = velocity * direction * Time.deltaTime;

        // Lanza un ray en la dirección de movimiento
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.right * direction, out hit, Mathf.Abs(step) + 0.5f))
        {
            if (hit.collider.CompareTag("BounceZone"))
            {
                direction *= -1;
            }
        }

        transform.position += step * Vector3.right;
    }
}
