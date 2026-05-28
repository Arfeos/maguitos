using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Slime a distancia que mantiene un rango óptimo con el jugador y dispara un raycast cuando tiene línea de visión.
/// No usa la lógica de salto de <see cref="BasicSlimeController"/>; hereda directamente de <see cref="SlimeBase"/>
/// e implementa su propio movimiento de deslizamiento en el suelo.
/// </summary>
public class ShooterSlimeController : SlimeBase
{
    // ── Distancias ───────────────────────────────────────────────────────────
    [Header("Shooter - Distancias")]
    /// <summary>Distancia ideal a la que el slime intenta mantenerse del jugador.</summary>
    [Tooltip("Distancia ideal a la que quiere estar del jugador")]
    [SerializeField] private float preferredDistance = 15f;

    /// <summary>Si el jugador se acerca por debajo de este valor, el slime retrocede.</summary>
    [Tooltip("Si el jugador se acerca más de esto, el slime retrocede")]
    [SerializeField] private float tooCloseDistance = 10f;

    /// <summary>Si el jugador se aleja por encima de este valor, el slime avanza.</summary>
    [Tooltip("Si el jugador se aleja más de esto, el slime avanza")]
    [SerializeField] private float tooFarDistance = 20f;

    // ── Disparo ──────────────────────────────────────────────────────────────
    [Header("Shooter - Disparo")]
    /// <summary>Distancia máxima del raycast de disparo.</summary>
    [Tooltip("Distancia máxima del raycast")]
    [SerializeField] private float shootRange = 20f;

    /// <summary>Capas que puede impactar el disparo.</summary>
    [Tooltip("Layer que puede impactar el disparo")]
    [SerializeField] private LayerMask shootLayer;

    /// <summary>Magnitud de la dispersión aleatoria aplicada a la dirección del disparo en los ejes X e Y.</summary>
    [Tooltip("Dispersión del disparo")]
    [SerializeField] private float shootDispersion = 0.3f;

    // ── Movimiento ───────────────────────────────────────────────────────────
    [Header("Shooter - Movimiento")]
    /// <summary>Velocidad de desplazamiento al acercarse o alejarse del jugador.</summary>
    [SerializeField] private float moveSpeed = 3f;

    /// <summary>Transform desde el que se origina el raycast de disparo. Si es null se usa la posición del slime.</summary>
    [Tooltip("Transform desde donde sale el rayo")]
    [SerializeField] private UnityEngine.Transform shootOrigin;

    // ── Ground Check ─────────────────────────────────────────────────────────
    [Header("Ground Check")]
    /// <summary>Radio de la esfera usada para comprobar si el slime está en el suelo.</summary>
    [SerializeField] private float groundRadius = 0.25f;

    /// <summary>Capas que se consideran suelo.</summary>
    [SerializeField] private LayerMask groundLayer;

    // ── Visual del disparo ───────────────────────────────────────────────────
    [Header("Shoot Material")]
    /// <summary>Materiales usados por <see cref="ISpellService"/> para renderizar la línea del disparo. Si es null se usa el material por defecto.</summary>
    public List<Material> RayMaterial;

    // ── Estado interno ───────────────────────────────────────────────────────
    /// <summary>Indica si el slime está tocando el suelo. El movimiento solo se aplica cuando es <c>true</c>.</summary>
    private bool _isGrounded;

    // ── Sonidos ──────────────────────────────────────────────────────────────
    [Header("Sounds")]
    /// <summary>Sonido que se reproduce cuando el disparo impacta en un objetivo.</summary>
    [SerializeField] protected AudioClip ShotImpact;

    // ── Override OnUpdate ────────────────────────────────────────────────────

    /// <summary>
    /// Comprueba el suelo cada frame antes de evaluar el estado de ataque o movimiento.
    /// </summary>
    protected override void OnUpdate()
    {
        CheckGrounded();
        base.OnUpdate(); // → CheckState → Attack o Move
    }

    // ── Implementación obligatoria ───────────────────────────────────────────

    /// <summary>
    /// Orienta el slime hacia el jugador, activa la animación de ataque y lanza un raycast con dispersión.
    /// Si el rayo impacta un <see cref="IHittable"/>, aplica daño y reproduce el sonido de impacto.
    /// En cualquier caso, dibuja la línea del disparo usando <see cref="ISpellService"/>.
    /// </summary>
    protected override void Attack()
    {
        if (_playerController == null) return;
        base.Attack();
        RaycastHit hit;
        Vector3 endPoint;
        // Mirar al jugador antes de disparar
        FacePlayer();

        _animator.SetTrigger("Attack");

        Vector3 origin = shootOrigin != null ? shootOrigin.position : transform.position;
        Vector3 directionfinal = CalculateDispersion((_playerController.transform.position - origin).normalized);
        if (Physics.Raycast(origin, directionfinal, out hit, shootRange, shootLayer))
        {
            endPoint = hit.point;
            if (hit.collider.gameObject.GetComponent<IHittable>() != null)
            {
                hit.collider.gameObject.GetComponent<IHittable>().Hit(damage);
                if (_audioService == null) _audioService = AppContainer.Get<IAudioService>();
                if (ShotImpact != null) _audioService.PlaySound(ShotImpact);
            }

        }
        else
        {
            endPoint = origin + directionfinal * shootRange;
        }
        //Renderizar la línea del disparo

        var spellService = AppContainer.Get<ISpellService>();
        if(RayMaterial == null) spellService.ShootRay(origin, endPoint);
        else spellService.ShootRay(origin, endPoint, RayMaterial);
        
    }

    /// <summary>
    /// Gestiona la distancia con el jugador moviéndose en el plano XZ:
    /// retrocede si está demasiado cerca (<see cref="tooCloseDistance"/>),
    /// avanza si está demasiado lejos (<see cref="tooFarDistance"/>),
    /// o se queda quieto si está en el rango óptimo.
    /// No actúa si el slime está en el aire.
    /// </summary>
    /// <param name="distanceToPlayer">Distancia actual al jugador, recibida desde <see cref="SlimeBase.CheckState"/>.</param>
      protected override void Move(float distanceToPlayer)
    {
        if (!_isGrounded) return;
        if (_playerController == null) return;
 
        base.Move(distanceToPlayer);
        FacePlayer();
 
        Vector3 dirToPlayer = (_playerController.transform.position - transform.position);
        dirToPlayer.y = 0f;
        dirToPlayer.Normalize();
 
        Vector3 targetVelocity = Vector3.zero;
 
        if (distanceToPlayer < tooCloseDistance)
            targetVelocity = -dirToPlayer * moveSpeed;   // Retroceder
        else if (distanceToPlayer > tooFarDistance)
            targetVelocity = dirToPlayer * moveSpeed;    // Avanzar
 
        targetVelocity.y = _rigidbody.linearVelocity.y; // Conservar gravedad
        _rigidbody.linearVelocity = targetVelocity;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Rota el slime para que mire hacia el jugador en el plano XZ.
    /// No hace nada si el jugador no está asignado o la dirección es casi cero.
    /// </summary>
    private void FacePlayer()
    {
        if (_playerController == null) return;

        Vector3 dir = _playerController.transform.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            transform.forward = dir.normalized;
    }

    /// <summary>
    /// Comprueba mediante un OverlapSphere si algún collider de <see cref="groundLayer"/>
    /// está en contacto con el slime, actualizando <see cref="_isGrounded"/>.
    /// </summary>
    private void CheckGrounded()
    {
        _isGrounded = false;
        foreach (Collider c in Physics.OverlapSphere(transform.position, groundRadius, groundLayer))
        {
            if (c.transform == transform) continue;
            _isGrounded = true;
            break;
        }
    }

    /// <summary>
    /// Añade un desplazamiento aleatorio en X e Y a la dirección de disparo para simular imprecisión.
    /// El rango de dispersión está acotado por <see cref="shootDispersion"/>.
    /// </summary>
    /// <param name="direction">Dirección normalizada original hacia el jugador.</param>
    /// <returns>Dirección modificada con dispersión aplicada.</returns>
    private Vector3 CalculateDispersion(Vector3 vector3)
    {
        float xDispersiom = UnityEngine.Random.Range(shootDispersion, -shootDispersion);
        float yDispersiom = UnityEngine.Random.Range(shootDispersion, -shootDispersion);

        Vector3 dispersion = new Vector3(xDispersiom, yDispersiom, 0);
        return vector3 + dispersion;
    }
}