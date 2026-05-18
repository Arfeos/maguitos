using System.Collections;
using UnityEngine;

public class ExplosiveSlime : BasicSlimeController
{
    [SerializeField]private ParticleSystem _particleSystemExplosion;
    

    protected override void Attack()
    {
        

        Die();
    }

    protected override void Die()
    {
        StopAllCoroutines();

        _isDeath = true;

        
        _lifeBar.gameObject.SetActive(false);

        OnDeath();

        Explode();
    }
   


    private void Explode()
    {
        _particleSystemExplosion.Play();

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

    IEnumerator FinishExplosion()
    {
        // Espera mientras las partículas siguen activas
        while (_particleSystemExplosion.isPlaying)
        {
            yield return null;
        }

        // Destruye el objeto al terminar
        Destroy(gameObject);
    }
}
