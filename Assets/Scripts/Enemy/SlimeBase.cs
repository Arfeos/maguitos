using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Clase base para todos los slimes del juego.
/// Gestiona vida, barra de vida, disolución al morir y el ciclo de acciones.
/// Las subclases implementan <see cref="Attack"/> y <see cref="Move"/> con su comportamiento específico.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public abstract class SlimeBase : MonoBehaviour, IHittable
{
    // ── Referencias ─────────────────────────────────────────────────────────
    /// <summary>Referencia al controlador del jugador, localizado en escena.</summary>
    protected PlayerController _playerController;
    /// <summary>Rigidbody del slime, usado para aplicar fuerzas de movimiento.</summary>
    protected Rigidbody _rigidbody;
    /// <summary>Animator del slime, controla las transiciones de animación.</summary>
    protected Animator _animator;

    // ── Combat ───────────────────────────────────────────────────────────────
    [Header("Combat")]
    /// <summary>Distancia máxima al jugador para activar el ataque en lugar del movimiento.</summary>
    [SerializeField] protected float distanceToAttack = 2.5f;
    /// <summary>Tiempo en segundos que el slime espera entre acciones (ataque o movimiento).</summary>
    [SerializeField] protected float attackCooldown = 2f;
    /// <summary>Daño que inflige el slime al jugador por ataque.</summary>
    [SerializeField] protected float damage = 20f;
    /// <summary>Vida máxima del slime. También es el valor inicial de <see cref="Life"/>.</summary>
    [SerializeField] protected float maxLife = 100f;
    /// <summary>Máscara de capas que el slime puede impactar con su ataque.</summary>
    [SerializeField] protected LayerMask attackLayer;

    [Header("Puntuacion")]
    /// <summary>Puntos que se suman al perfil del jugador al matar este slime.</summary>
    [SerializeField] protected int puntuacion = 20;

    // ── Life bar ─────────────────────────────────────────────────────────────
    [Header("UI")]
    /// <summary>Slider de la UI que representa la barra de vida del slime.</summary>
    [SerializeField] protected Slider _lifeBar;

    // ── Dissolve ─────────────────────────────────────────────────────────────
    [Header("Dissolve")]
    /// <summary>Textura de ruido usada por el shader Custom/Dissolve para controlar el patrón de disolución.</summary>
    [SerializeField] protected Texture2D dissolveTexture;
    /// <summary>Color del borde que aparece durante la disolución.</summary>
    [SerializeField] protected Color dissolveColor = Color.red;
    /// <summary>Segundos que se espera tras la muerte antes de iniciar la disolución.</summary>
    [SerializeField] protected float delayBeforeStart = 3f;
    /// <summary>Duración en segundos de la animación de disolución completa.</summary>
    [SerializeField] protected float dissolveTime = 2f;

    // ── Orbes ───────────────────────────────────────────────────────────────
    [Header("Orbes")]
    /// <summary>Prefab del orbe de vida que puede spawnear al morir el slime.</summary>
    [SerializeField] protected GameObject orbVida;
    /// <summary>Prefab del orbe de maná que puede spawnear al morir el slime.</summary>
    [SerializeField] protected GameObject orbMana;
    /// <summary>Probabilidad (0-100) de que aparezca un orbe de vida al morir.</summary>
    [SerializeField, Range(0f, 100f)] protected float percentSpawnLife;
    /// <summary>Probabilidad (0-100) de que aparezca un orbe de maná al morir.</summary>
    [SerializeField, Range(0f, 100f)] protected float percentSpawnMana;

    // ── Sonidos ─────────────────────────────────────────────────────────────
    [Header("Sounds")]
    /// <summary>Sonido que se reproduce cuando el slime recibe daño.</summary>
    [SerializeField] protected AudioClip TakeDamageSound;
    /// <summary>Sonido que se reproduce cuando el slime salta o se mueve.</summary>
    [SerializeField] protected AudioClip JumpSound;
    /// <summary>Sonido que se reproduce cuando el slime realiza un ataque.</summary>
    [SerializeField] protected AudioClip AttackSound;

    // ── Estado interno ───────────────────────────────────────────────────────
    /// <summary>Vida actual del slime. Se reduce con cada golpe recibido.</summary>
    protected float Life;
    /// <summary>Indica si el slime ya ha muerto. Bloquea el Update y el Hit cuando es <c>true</c>.</summary>
    protected bool _isDeath;
    /// <summary>
    /// Bandera que habilita la siguiente acción del slime.
    /// Se pone a <c>false</c> al ejecutar una acción y vuelve a <c>true</c> tras el cooldown.
    /// </summary>
    protected bool _nextActionTime = true;
    /// <summary>Progreso actual de la disolución, de 0 (sin disolver) a 1 (completamente disuelto).</summary>
    protected float dissolveProgress;
    /// <summary>Referencia a la corrutina del cooldown de acciones, para evitar iniciarla varias veces.</summary>
    protected Coroutine _actionCoroutine;
    /// <summary>Servicio de audio inyectado desde el contenedor de dependencias.</summary>
    protected IAudioService _audioService;
    /// <summary>Servicio de perfiles para obtener el perfil activo del jugador.</summary>
    protected IProfileService _profileService;
    /// <summary>Servicio de puntuación para sumar puntos al matar al slime.</summary>
    protected IScoreService _scoreService;

    // ── Unity Lifecycle ──────────────────────────────────────────────────────
    /// <summary>
    /// Resuelve las dependencias de servicios desde el contenedor de la aplicación.
    /// </summary>
    protected virtual void Awake()
    {
        _audioService = AppContainer.Get<IAudioService>();
        _scoreService = AppContainer.Get<IScoreService>();
        _profileService = AppContainer.Get<IProfileService>();
    }

    /// <summary>
    /// Inicializa referencias de componentes, configura la barra de vida y los valores de estado.
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

    /// <summary>
    /// Cada frame busca al jugador si falta la referencia, comprueba si el slime está muerto
    /// y lanza la corrutina de cooldown de acciones si no hay ninguna en curso.
    /// </summary>
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

    /// <summary>
    /// Lógica de ataque específica del enemigo.
    /// La base reproduce el <see cref="AttackSound"/>; las subclases deben llamar a <c>base.Attack()</c>.
    /// </summary>
    protected virtual void Attack()
    {
        if (_audioService == null) _audioService = AppContainer.Get<IAudioService>();
        if (AttackSound != null)
            _audioService.PlaySound(AttackSound);
    }


    /// <summary>
    /// Lógica de movimiento específica del enemigo.
    /// La base reproduce el <see cref="JumpSound"/>; las subclases deben llamar a <c>base.Move()</c>.
    /// </summary>
    /// <param name="distanceToPlayer">Distancia actual al jugador, precalculada por <see cref="CheckState"/>.</param>
    protected virtual void Move(float distanceToPlayer)
    {
        if (_audioService == null) _audioService = AppContainer.Get<IAudioService>();
        if (JumpSound != null) _audioService.PlaySound(JumpSound);
    }


    // ── Métodos virtuales sobreescribibles ───────────────────────────────────

    /// <summary>
    /// Se llama cada frame desde <see cref="Update"/> (ya filtrado por null y muerte).
    /// Sobreescribe para añadir lógica extra (ej: CheckGrounded, DetectLanding).
    /// La base ya llama a <see cref="CheckState"/>.
    /// </summary>
    protected virtual void OnUpdate()
    {
        CheckState();
    }

    /// <summary>
    /// Se llama justo antes de iniciar la disolución del objeto.
    /// La base suma puntuación e intenta spawnear orbes según sus porcentajes configurados.
    /// Sobreescribe para añadir lógica extra al morir (drops, efectos, eventos, etc.).
    /// </summary>
    protected virtual void OnDeath()
    {
        if (_scoreService == null) _scoreService = AppContainer.Get<IScoreService>();
        _scoreService.addPoints(_profileService.getSelectedProfile().guid, puntuacion);

        CreateOrbsByPercent(percentSpawnMana, orbMana);
        CreateOrbsByPercent(percentSpawnLife, orbVida);
    }


    // ── Lógica común ─────────────────────────────────────────────────────────

    /// <summary>
    /// Evalúa si el slime debe atacar o moverse en función de la distancia al jugador.
    /// Solo actúa si <see cref="_nextActionTime"/> es <c>true</c>.
    /// </summary>
    protected void CheckState()
    {
        if (!_nextActionTime) return;

        float distance = CalculateDistance();

        if (distance <= distanceToAttack)
            Attack();
        else
            Move(distance);
    }

    /// <summary>
    /// Calcula la distancia plana (ignorando el eje Y) entre el slime y el jugador.
    /// </summary>
    /// <returns>Distancia en unidades de mundo al jugador, proyectada en el plano XZ.</returns>
    protected float CalculateDistance()
    {
        Vector3 target = _playerController.transform.position;
        target.y = 0f;

        Vector3 origin = transform.position;
        origin.y = 0f;

        return Vector3.Distance(origin, target);
    }


   // ── IHittable ────────────────────────────────────────────────────────────
 
    /// <summary>
    /// Recibe un golpe, reduce la vida y activa la barra de vida.
    /// Si la vida llega a 0 o menos, llama a <see cref="Die"/>.
    /// No tiene efecto si el slime ya está muerto.
    /// </summary>
    /// <param name="damage">Cantidad de daño a aplicar.</param>
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
 
    /// <summary>
    /// Ejecuta la secuencia de muerte: detiene corrutinas, activa la animación de muerte,
    /// llama a <see cref="OnDeath"/> e inicia la disolución con retardo.
    /// </summary>
    protected virtual void Die()
    {
        StopAllCoroutines();
        _isDeath = true;
 
        _animator.SetTrigger("Death");
        _lifeBar.gameObject.SetActive(false);
 
        OnDeath();
 
        StartCoroutine(DissolveAfterDelay());
    }

    /// <summary>
    /// Corrutina que espera <see cref="delayBeforeStart"/> segundos, luego aplica el shader
    /// de disolución a todos los renderers hijos y anima el progreso hasta destruir el objeto.
    /// </summary>
    protected virtual IEnumerator DissolveAfterDelay()
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

    /// <summary>
    /// Comprueba la probabilidad y, si se cumple, instancia el orbe indicado cerca del slime.
    /// </summary>
    /// <param name="Percent">Probabilidad de 0 a 100 de que aparezca el orbe.</param>
    /// <param name="Orb">Prefab del orbe a instanciar.</param>
    protected void CreateOrbsByPercent(float Percent, GameObject Orb)
    {
        if (CheckPercentSpawn(Percent))
        {
            InstantiateOrb(Orb);
        }
    }

    /// <summary>
    /// Genera un número aleatorio entre 0 y 100 y lo compara con <paramref name="percentToCheck"/>.
    /// </summary>
    /// <param name="percentToCheck">Umbral de probabilidad (0-100).</param>
    /// <returns><c>true</c> si el número aleatorio es menor o igual al umbral; <c>false</c> en caso contrario.</returns>
    protected bool CheckPercentSpawn(float percentToCheck)
    {
        float percentValue = UnityEngine.Random.Range(0, 100);
        return percentValue <= percentToCheck;
    }

     /// <summary>
    /// Instancia el prefab <paramref name="Orb"/> en una posición aleatoria dentro de 1 unidad del slime.
    /// </summary>
    /// <param name="Orb">Prefab del orbe a instanciar.</param>
    protected void InstantiateOrb(GameObject Orb)
    {
        Vector3 randomPos = transform.position + UnityEngine.Random.insideUnitSphere * 1f;
        Instantiate(Orb, randomPos, quaternion.identity);
    }

    // ── Cooldown de acciones ─────────────────────────────────────────────────

    /// <summary>
    /// Corrutina que bloquea nuevas acciones durante <see cref="attackCooldown"/> segundos
    /// y después las habilita de nuevo, limpiando la referencia <see cref="_actionCoroutine"/>.
    /// </summary>
    protected IEnumerator ActionCooldownRoutine()
    {
        _nextActionTime = false;
        yield return new WaitForSeconds(attackCooldown);
        _nextActionTime = true;
        _actionCoroutine = null;
    }
}
