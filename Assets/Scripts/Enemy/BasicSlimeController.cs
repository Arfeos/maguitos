
using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BasicSlimeController : SlimeBase
{
    [Header("Jump")]
    [SerializeField] private float jumpDistance = 3f;
    [SerializeField] private float jumpHeight = 2f;

    [Header("Landing")]
    [SerializeField] private float recoverTime = 2f;

    [Header("Ground Check")]
    [SerializeField] private float groundRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;

    // ── Estado interno ───────────────────────────────────────────────────────
    private bool _isGrounded;
    private bool _wasGrounded;
    private bool _isJumping;

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

    private void DetectLanding()
    {
        if (!_wasGrounded && _isGrounded && _isJumping)
        {
            _isJumping = false;
            StartCoroutine(LandingRoutine());
        }
    }

    private IEnumerator LandingRoutine()
    {
        _rigidbody.linearVelocity = Vector3.zero;
        _animator.SetTrigger("Landing");
        yield return new WaitForSeconds(recoverTime);
    }

}
