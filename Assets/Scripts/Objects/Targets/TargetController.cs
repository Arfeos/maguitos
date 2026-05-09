using UnityEngine;
using UnityEngine.WSA;

public class TargetController : MonoBehaviour, IHittable
{
    IAnimationService _animationService;
    IEventService _eventService;
    IScoreService _scoreService;
    [SerializeField]AudioClip audioWhenHit;
    [SerializeField] int minVelocity;
    [SerializeField] int maxVelocity;
    [SerializeField] int puntos;
    private int velocity;
    private int direction = 1;
    private Vector3 InitialPosition;
    private bool _isGameStarted = false;
    public void Hit()
    {
        _animationService.WobbleAnimationWithSound(this.gameObject, audioWhenHit);
        //TODO: esto no es taki taki rumba pero pensando en un multiplayter futuro se queda asi, darle una revision cuando se pueda
        if (_isGameStarted)
        {
            _scoreService.addPoints("TutorialPlayer", puntos);
            Debug.Log("Has ganado " + puntos + " puntos");
        }
    }
    private void Awake()
    {
        _animationService = AppContainer.Get<IAnimationService>();
        _eventService = AppContainer.Get<IEventService>();
        _scoreService = AppContainer.Get<IScoreService>();
        velocity = Random.Range(minVelocity, maxVelocity);
        InitialPosition = this.transform.position;
    }

    private void Update()
    {
        if(!_isGameStarted) return;
        Move();
    }

    public void activateGame(GameEventBase parameters)
    {
        _isGameStarted = !_isGameStarted;
        if (!_isGameStarted)
        {
            _scoreService.resetScore();
        }
    }

    private void OnEnable()
    {
        _eventService.Subscribe<TutorialGameEvent>(activateGame);
    }
    private void OnDisable()
    {
        _eventService.Unsubscribe<TutorialGameEvent>(activateGame);
    }

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
