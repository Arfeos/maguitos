
using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

/// <summary>
/// Slime básico que se mueve saltando hacia el jugador y ataca por proximidad con un OverlapSphere.
/// Extiende <see cref="SlimeBase"/> añadiendo detección de suelo, lógica de aterrizaje y física de salto parabólico.
/// </summary>
public class BasicSlimeController : SlimeBase
{
    // ── Salto ────────────────────────────────────────────────────────────────
    [Header("Jump")]
    /// <summary>Distancia horizontal máxima que el slime recorre en cada salto.</summary>
    [SerializeField] protected float jumpDistance = 3f;
    /// <summary>Altura máxima que alcanza el arco del salto.</summary>
    [SerializeField] protected float jumpHeight = 2f;


    // ── Aterrizaje ───────────────────────────────────────────────────────────
    [Header("Landing")]
    /// <summary>Segundos que el slime permanece inmóvil tras aterrizar antes de poder actuar de nuevo.</summary>
    [SerializeField] protected float recoverTime = 2f;

    // ── Detección de suelo ───────────────────────────────────────────────────
    [Header("Ground Check")]
    /// <summary>Radio de la esfera usada para detectar si el slime está en el suelo.</summary>
    [SerializeField] protected float groundRadius = 0.25f;
    /// <summary>Capas que se consideran suelo para la detección de aterrizaje.</summary>
    [SerializeField] protected LayerMask groundLayer;

    // ── Estado interno ───────────────────────────────────────────────────────
    /// <summary>Indica si el slime está tocando el suelo en el frame actual.</summary>
    protected bool _isGrounded;
    /// <summary>Estado de <see cref="_isGrounded"/> en el frame anterior, usado para detectar el aterrizaje.</summary>
    protected bool _wasGrounded;
    /// <summary>Indica si el slime está en el aire tras haber saltado.</summary>
    protected bool _isJumping;

    // ── Override lifecycle ───────────────────────────────────────────────────

    /// <summary>
    /// Extiende el update base añadiendo detección de suelo y aterrizaje antes de evaluar el estado.
    /// </summary>
    protected override void OnUpdate()
    {
        CheckGrounded();
        DetectLanding();
        base.OnUpdate(); // Llama a CheckState()
        _wasGrounded = _isGrounded;
    }

    // ── Implementación obligatoria ───────────────────────────────────────────
    /// <summary>
    /// Activa la animación de ataque y aplica daño a todos los <see cref="IHittable"/> dentro del rango,
    /// excluyendo al propio slime y a otros slimes.
    /// </summary>
    protected override void Attack()
    {
        base.Attack();
        _animator.SetTrigger("Attack");

        Collider[] hits = Physics.OverlapSphere(transform.position, distanceToAttack + 0.5f, attackLayer);
        foreach (Collider hit in hits)
        {
            if (hit.transform == transform) continue;
            IHittable hittable = hit.GetComponent<IHittable>();
            if (hittable != null)
                if (!hit.GetComponent<SlimeBase>())
                    hittable.Hit(damage);
        }
    }

    /// <summary>
    /// Lanza el slime hacia el jugador con una trayectoria parabólica calculada a partir de
    /// <see cref="jumpHeight"/> y la gravedad del proyecto. Si el jugador está más cerca que
    /// <see cref="jumpDistance"/>, recorta la distancia para no sobrepasarlo.
    /// </summary>
    /// <param name="distanceToPlayer">Distancia actual al jugador, recibida desde <see cref="SlimeBase.CheckState"/>.</param>
    protected override void Move(float distanceToPlayer)
    {
        base.Move(distanceToPlayer);
        _animator.SetTrigger("Jump");

        float distanceToJump = jumpDistance;

        if (distanceToPlayer < jumpDistance)
            distanceToJump = distanceToPlayer - distanceToAttack - 0.5f;

        if (distanceToJump <= 0f)
            distanceToJump = 0.1f;

        Vector3 start = transform.position;
        Vector3 direction = (_playerController.transform.position - start);
        direction.y = 0f;
        direction.Normalize();

        transform.forward = direction;

        Vector3 endPoint = start + direction * distanceToJump;

        float gravity = Mathf.Abs(Physics.gravity.y);
        float verticalVelocity = Mathf.Sqrt(2f * gravity * jumpHeight);
        float totalTime = (verticalVelocity / gravity) * 2f;

        Vector3 horizontalVelocity = (endPoint - start) / totalTime;
        horizontalVelocity.y = 0f;

        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.linearVelocity = horizontalVelocity + Vector3.up * verticalVelocity;

        _isJumping = true;
    }

    // ── Ground check ─────────────────────────────────────────────────────────

    /// <summary>
    /// Comprueba si algún collider de <see cref="groundLayer"/> toca la esfera de radio
    /// <see cref="groundRadius"/> centrada en el slime, actualizando <see cref="_isGrounded"/>.
    /// </summary>
    protected void CheckGrounded()
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
    /// Detecta el momento exacto de aterrizaje comparando <see cref="_wasGrounded"/> con
    /// <see cref="_isGrounded"/>. Solo actúa si el slime venía de un salto activo.
    /// </summary>
    protected void DetectLanding()
    {
        if (!_wasGrounded && _isGrounded && _isJumping)
        {
            _isJumping = false;
            StartCoroutine(LandingRoutine());
        }
    }

    /// <summary>
    /// Corrutina de aterrizaje: detiene la velocidad del rigidbody, activa la animación de
    /// aterrizaje y espera <see cref="recoverTime"/> segundos antes de liberar el control.
    /// </summary>
    protected IEnumerator LandingRoutine()
    {
        _rigidbody.linearVelocity = Vector3.zero;
        _animator.SetTrigger("Landing");
        yield return new WaitForSeconds(recoverTime);
    }
}
