using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SlimeController : MonoBehaviour, IHittable
{
    PlayerController _playerController;

    [Header("Jump")]
    [SerializeField] private float jumpDistance = 3f;
    [SerializeField] private float jumpHeight = 2f;

    [Header("Combat")]
    [SerializeField] private float distanceToAttack = 1.5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float damage = 20;
    [SerializeField] private float Life = 100f;
    [SerializeField] private LayerMask AttackLayer;


    [Header("Landing")]
    [SerializeField] private float recoverTime = 2f;

    [Header("Ground Check")]
    
    [SerializeField] private float groundRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Dissolve")]
    [SerializeField] private Texture2D dissolveTexture;
    [SerializeField] private Color dissolveColor = Color.red;
    [SerializeField] private float delayBeforeStart = 3f;
    [SerializeField] private float dissolveTime = 2f;


    [SerializeField] private float dissolveProgress = 0.3f;

    

    Rigidbody _rigidbodySlime;
    Animator _animator;

    bool _nextActionTime = true;

    bool _isGrounded;
    bool _wasGrounded;

    bool _isJumping;

    bool _isAttacking;

    bool _isDeath;

    void Start()
    {
        _playerController = FindFirstObjectByType<PlayerController>();

        _rigidbodySlime = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (_playerController == null)
            return;
        if (_isDeath) return;
        CheckGrounded();

        DetectLanding();

        CheckState();

        _wasGrounded = _isGrounded;
    }

    void CheckGrounded()
    {
        Collider[] hits = Physics.OverlapSphere(
      transform.position,
      groundRadius,
      groundLayer
  );

        _isGrounded = false;

        foreach (Collider c in hits)
        {
            if (c.transform != transform)
            {
                _isGrounded = true;
                break;
            }
        }
    }

    void DetectLanding()
    {
        // Estaba en el aire y acaba de tocar suelo
        if (!_wasGrounded && _isGrounded && _isJumping)
        {
            _isJumping = false;

            StartCoroutine(LandingRoutine());
        }
    }

    void CheckState()
    {
        if (!_nextActionTime)
            return;

        float distance = CalculateDistance();

        if (distance <= distanceToAttack)
        {
            Attack();
        }
        else if (_isGrounded)
        {
            Move(distance);
        }
    }

    float CalculateDistance()
    {
        Vector3 target = _playerController.transform.position;
        target.y = 0f;

        Vector3 start = transform.position;
        start.y = 0f;

        return Vector3.Distance(start, target);
    }

    void Attack()
    {
        _nextActionTime = false;

        _animator.SetTrigger("Attack");
        _isAttacking = true;


        var CollisionAttack = Physics.OverlapSphere(transform.position, distanceToAttack + 0.5f, AttackLayer);
        foreach (Collider hit in CollisionAttack)
        {
            if (hit.CompareTag("Player") && hit.GetComponent<IHittable>() != null)
            {
                hit.GetComponent<IHittable>().Hit(20);
            }
        }

        StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);

        _nextActionTime = true;
        _isAttacking = false;
    }

    void Move(float distance)
    {
        _nextActionTime = false;

        _animator.SetTrigger("Jump");

        float distanceToJump = jumpDistance;

        // Evitar acercarse demasiado al player
        if (distance < jumpDistance)
        {
            distanceToJump = distance - distanceToAttack - 0.5f
            ;
        }
        if (distanceToJump <= 0)
        {
            distanceToJump = 0.1f;
        }

        Vector3 start = transform.position;

        // Dirección horizontal
        Vector3 direction =
            (_playerController.transform.position - start);

        direction.y = 0f;
        direction.Normalize();

        // Mirar hacia el player
        transform.forward = direction;

        // Punto final
        Vector3 endPoint =
            start + direction * distanceToJump;

        // Física del salto
        float gravity = Mathf.Abs(Physics.gravity.y);

        // Velocidad vertical
        float verticalVelocity =
            Mathf.Sqrt(2f * gravity * jumpHeight);

        // Tiempo hasta el pico
        float timeUp = verticalVelocity / gravity;

        // Tiempo total
        float totalTime = timeUp * 2f;

        // Velocidad horizontal
        Vector3 horizontalVelocity =
            (endPoint - start) / totalTime;

        horizontalVelocity.y = 0f;

        // Velocidad final
        Vector3 velocity =
            horizontalVelocity +
            Vector3.up * verticalVelocity;

        // Resetear velocidad
        _rigidbodySlime.linearVelocity = Vector3.zero;

        // Aplicar salto
        _rigidbodySlime.linearVelocity = velocity;

        _isJumping = true;
    }

    IEnumerator LandingRoutine()
    {
        // Detener movimiento residual
        _rigidbodySlime.linearVelocity = Vector3.zero;

        // Animación squash / recover
        _animator.SetTrigger("Landing");

        // Esperar mientras está aplastado
        yield return new WaitForSeconds(recoverTime);

        // Puede volver a actuar
        _nextActionTime = true;
    }

    void OnDrawGizmosSelected()
    {
       

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            transform.position,
            groundRadius
        );


    }

    public void Hit(float damage)
    {
        if (_isDeath) return;
        Life -= damage;

        if (Life <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        StopAllCoroutines();
        Debug.Log("DEath");
        _animator.SetTrigger("Death");
        _isDeath = true;
        StartCoroutine("DissolveAfterDelay");
    }



    private IEnumerator DissolveAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeStart);

        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            Material[] mats = r.materials;

            for (int i = 0; i < mats.Length; i++)
            {
                mats[i].shader = Shader.Find("Custom/Dissolve");
                mats[i].SetTexture("_DissolveTex", dissolveTexture);
                mats[i].SetColor("_DissolveColor", dissolveColor);
            }
        }

        while (dissolveProgress < 1f)
        {
            dissolveProgress += Time.deltaTime / dissolveTime;

            foreach (Renderer r in renderers)
            {
                Material[] mats = r.materials;

                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i].SetFloat("_DissolveThreshold", dissolveProgress);
                }
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}