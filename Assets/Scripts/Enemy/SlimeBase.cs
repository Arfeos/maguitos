using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Clase base para todos los slimes del juego.
/// Gestiona vida, barra de vida, disolución al morir y el ciclo de acciones.
/// Las subclases implementan Attack() y Move() con su comportamiento específico.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public abstract class SlimeBase : MonoBehaviour, IHittable
{
    // ── Referencias ─────────────────────────────────────────────────────────
    protected PlayerController _playerController;
    protected Rigidbody _rigidbody;
    protected Animator _animator;

    // ── Combat ───────────────────────────────────────────────────────────────
    [Header("Combat")]
    [SerializeField] protected float distanceToAttack = 2.5f;
    [SerializeField] protected float attackCooldown = 2f;
    [SerializeField] protected float damage = 20f;
    [SerializeField] protected float maxLife = 100f;
    [SerializeField] protected LayerMask attackLayer;

    [Header("Puntuacion")]
    [SerializeField] protected int puntuación = 20;

    // ── Life bar ─────────────────────────────────────────────────────────────
    [Header("UI")]
    [SerializeField] protected Slider _lifeBar;

    // ── Dissolve ─────────────────────────────────────────────────────────────
    [Header("Dissolve")]
    [SerializeField] protected Texture2D dissolveTexture;
    [SerializeField] protected Color dissolveColor = Color.red;
    [SerializeField] protected float delayBeforeStart = 3f;
    [SerializeField] protected float dissolveTime = 2f;
    // ── Sonidos ─────────────────────────────────────────────────────────────
    [Header("Sounds")]
    [SerializeField] protected AudioClip TakeDamageSound;
    [SerializeField] protected AudioClip JumpSound;
    [SerializeField] protected AudioClip AttackSound;
    // ── Estado interno ───────────────────────────────────────────────────────
    protected float Life;
    protected bool _isDeath;
    protected bool _nextActionTime = true;

    protected float dissolveProgress;
    protected Coroutine _actionCoroutine;
    protected IAudioService _audioService;

    protected IScoreService _scoreService;
    // ── Unity Lifecycle ──────────────────────────────────────────────────────
    private void Awake()
    {
        _audioService = AppContainer.Get<IAudioService>();
        _scoreService = AppContainer.Get<IScoreService>();
    }
    protected virtual void Start()
    {
        _playerController = FindFirstObjectByType<PlayerController>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        _lifeBar = GetComponentInChildren<Slider>();
        _lifeBar.maxValue = maxLife;
        _lifeBar.value = maxLife;


        _lifeBar.gameObject.SetActive(false);
        Life = maxLife;
        dissolveProgress = 0f;
    }

    protected virtual void Update()
    {
        if (_playerController == null)
        {
            _playerController = FindFirstObjectByType<PlayerController>();
            return;
        }

        if (_isDeath) return;

        OnUpdate();

        if (_actionCoroutine == null)
            _actionCoroutine = StartCoroutine(ActionCooldownRoutine());
    }

    // ── Métodos abstractos que cada enemigo debe implementar ─────────────────

    /// <summary>Lógica de ataque específica del enemigo.</summary>
    protected virtual void Attack() {
        if (_audioService == null) _audioService = AppContainer.Get<IAudioService>();
        if (AttackSound != null)

            _audioService.PlaySound(AttackSound);
    }


    /// <summary>Lógica de movimiento específica del enemigo.</summary>
    protected virtual void Move(float distanceToPlayer) {
        if (_audioService == null) _audioService = AppContainer.Get<IAudioService>();
        if (JumpSound != null) _audioService.PlaySound(JumpSound);
    }

    // ── Métodos virtuales sobreescribibles ───────────────────────────────────

    /// <summary>
    /// Se llama cada Update (ya filtrado por null y muerte).
    /// Sobreescribe para añadir lógica extra (ej: CheckGrounded, DetectLanding).
    /// La base ya llama a CheckState().
    /// </summary>
    protected virtual void OnUpdate()
    {
        CheckState();
    }

    /// <summary>Se llama justo antes de destruir el objeto. Sobreescribe para lógica extra al morir.</summary>
    protected virtual void OnDeath() 
    {
        //_scoreService.addPoints("Horde", puntuación);
    }

    // ── Lógica común ─────────────────────────────────────────────────────────

    protected void CheckState()
    {
        if (!_nextActionTime) return;

        float distance = CalculateDistance();

        if (distance <= distanceToAttack)
            Attack();
        else
            Move(distance);
    }

    protected float CalculateDistance()
    {
        Vector3 target = _playerController.transform.position;
        target.y = 0f;

        Vector3 origin = transform.position;
        origin.y = 0f;

        return Vector3.Distance(origin, target);
    }

    // ── IHittable ────────────────────────────────────────────────────────────

    public virtual void Hit(float damage)
    {
        if (_audioService == null) _audioService = AppContainer.Get<IAudioService>();
        if (TakeDamageSound != null)

            _audioService.PlaySound(TakeDamageSound);
        if (_isDeath) return;

        _lifeBar.gameObject.SetActive(true);
        Life -= damage;
        _lifeBar.value = Life;

        if (Life <= 0)
            Die();
    }

    // ── Muerte y disolución ──────────────────────────────────────────────────

    protected virtual void Die()
    {
        

        StopAllCoroutines();
        _isDeath = true;

        _animator.SetTrigger("Death");
        _lifeBar.gameObject.SetActive(false);

        OnDeath();

        StartCoroutine(DissolveAfterDelay());
    }

    private IEnumerator DissolveAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeStart);

        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        // Cambiar shader en todos los materiales
        foreach (Renderer r in renderers)
        {
            foreach (Material mat in r.materials)
            {
                mat.shader = Shader.Find("Custom/Dissolve");
                mat.SetTexture("_DissolveTex", dissolveTexture);
                mat.SetColor("_DissolveColor", dissolveColor);
            }
        }

        // Animar la disolución
        while (dissolveProgress < 1f)
        {
            dissolveProgress += Time.deltaTime / dissolveTime;

            foreach (Renderer r in renderers)
                foreach (Material mat in r.materials)
                    mat.SetFloat("_DissolveThreshold", dissolveProgress);

            yield return null;
        }

        Destroy(gameObject);
    }

    // ── Cooldown de acciones ─────────────────────────────────────────────────

    private IEnumerator ActionCooldownRoutine()
    {
        _nextActionTime = false;
        yield return new WaitForSecondsRealtime(attackCooldown);
        _nextActionTime = true;
        _actionCoroutine = null;
    }
}
