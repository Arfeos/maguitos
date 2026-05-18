
using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BasicSlimeController : SlimeBase
{
    [Header("Jump")]
    [SerializeField] protected float jumpDistance = 3f;
    [SerializeField] protected float jumpHeight = 2f;

    [Header("Landing")]
    [SerializeField] protected float recoverTime = 2f;

    [Header("Ground Check")]
    [SerializeField] protected float groundRadius = 0.25f;
    [SerializeField] protected LayerMask groundLayer;

    // ── Estado interno ───────────────────────────────────────────────────────
    protected bool _isGrounded;
    protected bool _wasGrounded;
    protected bool _isJumping;

    // ── Override lifecycle ───────────────────────────────────────────────────

    protected override void Start()
    {
        base.Start(); // Inicializa referencias comunes, vida, barra, etc.
    }

    /// <summary>Añade detección de suelo y aterrizaje al ciclo base.</summary>
    protected override void OnUpdate()
    {
        CheckGrounded();
        DetectLanding();
        base.OnUpdate(); // Llama a CheckState()
        _wasGrounded = _isGrounded;
    }

    // ── Implementación obligatoria ───────────────────────────────────────────

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
                hittable.Hit(damage);
        }
    }

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

    protected void DetectLanding()
    {
        if (!_wasGrounded && _isGrounded && _isJumping)
        {
            _isJumping = false;
            StartCoroutine(LandingRoutine());
        }
    }

    protected IEnumerator LandingRoutine()
    {
        _rigidbody.linearVelocity = Vector3.zero;
        _animator.SetTrigger("Landing");
        yield return new WaitForSeconds(recoverTime);
    }

}
