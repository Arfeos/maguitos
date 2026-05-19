using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

/// <summary>
/// Slime a distancia: mantiene un rango óptimo con el jugador
/// y dispara un raycast cuando tiene línea de visión.
/// No usa lógica de salto del SlimeController base.
/// </summary>
public class ShooterSlimeController : SlimeBase
{
    [Header("Shooter - Distancias")]
    [Tooltip("Distancia ideal a la que quiere estar del jugador")]
    [SerializeField] private float preferredDistance = 15f;

    [Tooltip("Si el jugador se acerca más de esto, el slime retrocede")]
    [SerializeField] private float tooCloseDistance = 10f;

    [Tooltip("Si el jugador se aleja más de esto, el slime avanza")]
    [SerializeField] private float tooFarDistance = 20f;

    [Header("Shooter - Disparo")]
    [Tooltip("Distancia máxima del raycast")]
    [SerializeField] private float shootRange = 20f;

    [Tooltip("Layer que puede impactar el disparo")]
    [SerializeField] private LayerMask shootLayer;

    [Tooltip("Dispersion del disparo")]
    [SerializeField] private float shootDispersion = 0.3f;

    [Header("Shooter - Movimiento")]
    [SerializeField] private float moveSpeed = 3f;

    [Tooltip("Transform desde donde sale el rayo")]
    [SerializeField] private UnityEngine.Transform shootOrigin;

    // ── Ground Check ─────────────────────────────────────────────────────────
    [Header("Ground Check")]
    [SerializeField] private float groundRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;

    [Header("shoot material")]
    public List<Material> RayMaterial;
    private bool _isGrounded;
    // ── Sonidos ─────────────────────────────────────────────────────────────
    [Header("Sounds")]
    [SerializeField] protected AudioClip ShotImpact;

    // ── Override OnUpdate ────────────────────────────────────────────────────

    protected override void OnUpdate()
    {
        CheckGrounded();
        base.OnUpdate(); // → CheckState → Attack o Move
    }

    // ── Implementación obligatoria ───────────────────────────────────────────

    /// <summary>
    /// Dispara un raycast hacia el jugador si tiene línea de visión.
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
        //Producir la linea
        
        var spellService = AppContainer.Get<ISpellService>();
        if(RayMaterial == null) spellService.ShootRay(origin, endPoint);
        else spellService.ShootRay(origin, endPoint, RayMaterial);
        
    }

    /// <summary>
    /// Gestiona la distancia con el jugador:
    /// - Demasiado cerca → retrocede
    /// - Demasiado lejos → avanza
    /// - En rango óptimo → se queda quieto
    /// </summary>
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
        {
            // Retroceder (dirección contraria al jugador)
            targetVelocity = -dirToPlayer * moveSpeed;
            _animator.SetBool("IsMoving", true);
        }
        else if (distanceToPlayer > tooFarDistance)
        {
            // Avanzar hacia el jugador
            targetVelocity = dirToPlayer * moveSpeed;
            _animator.SetBool("IsMoving", true);
        }
        else
        {
            // Rango óptimo: parar
            _animator.SetBool("IsMoving", false);
        }

        targetVelocity.y = _rigidbody.linearVelocity.y; // Conservar gravedad
        _rigidbody.linearVelocity = targetVelocity;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void FacePlayer()
    {
        if (_playerController == null) return;

        Vector3 dir = _playerController.transform.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            transform.forward = dir.normalized;
    }

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
    private Vector3 CalculateDispersion(Vector3 vector3)
    {
        float xDispersiom = UnityEngine.Random.Range(shootDispersion, -shootDispersion);
        float yDispersiom = UnityEngine.Random.Range(shootDispersion, -shootDispersion);

        Vector3 dispersion = new Vector3(xDispersiom, yDispersiom, 0);
        return vector3 + dispersion;
    }
}