using System.Collections;
using UnityEngine;

/// <summary>
/// Slime que explota al entrar en rango de ataque, dañando todo lo que esté cerca y destruyéndose.
/// Sobreescribe <see cref="Attack"/> para activar la explosión directamente en lugar de golpear,
/// y <see cref="Die"/> para omitir la animación de muerte y la disolución estándar.
/// </summary>
public class ExplosiveSlime : BasicSlimeController
{
    // ── Efectos ──────────────────────────────────────────────────────────────
    /// <summary>Sistema de partículas que se reproduce durante la explosión.</summary>
    [SerializeField] private ParticleSystem _particleSystemExplosion;

    // ── Override de comportamiento ───────────────────────────────────────────
    /// <summary>
    /// Al entrar en rango, el slime explosivo se suicida en lugar de atacar normalmente.
    /// No llama a <c>base.Attack()</c> porque no reproduce sonido de ataque ni animación.
    /// </summary>
    protected override void Attack()
    {
        Die();
    }

    /// <summary>
    /// Sobreescribe la muerte para omitir la animación de Death y la disolución.
    /// Detiene todas las corrutinas, marca el slime como muerto, oculta la barra de vida,
    /// ejecuta <see cref="SlimeBase.OnDeath"/> (puntuación y orbes) e inicia la explosión.
    /// </summary>
    protected override void Die()
    {
        StopAllCoroutines();

        _isDeath = true;

        _lifeBar.gameObject.SetActive(false);

        OnDeath();

        Explode();
    }


    
     // ── Explosión ────────────────────────────────────────────────────────────
 
    /// <summary>
    /// Reproduce los efectos de partículas y sonido, aplica daño a todos los <see cref="IHittable"/>
    /// dentro del rango de ataque (excluyendo al propio slime) e inicia la corrutina
    /// que destruye el objeto al terminar las partículas.
    /// </summary>
    private void Explode()
    {
        _particleSystemExplosion.Play();
        if (_audioService == null) _audioService = AppContainer.Get<IAudioService>();
        if (AttackSound != null)
            _audioService.PlaySound(AttackSound);
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            distanceToAttack + 0.5f,
            attackLayer
        );

        foreach (Collider hit in hits)
        {
            if (hit.transform == transform) continue;

            IHittable hittable = hit.GetComponent<IHittable>();

            if (hittable != null)
                hittable.Hit(damage);
        }

        StartCoroutine(FinishExplosion());
    }

    /// <summary>
    /// Espera a que el sistema de partículas termine de reproducirse y luego destruye el objeto.
    /// </summary>
    private IEnumerator FinishExplosion()
    {
        while (_particleSystemExplosion.isPlaying)
            yield return null;

        Destroy(gameObject);
    }
}
