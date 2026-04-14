using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SpellBase : MonoBehaviour
{
#nullable enable
    public enum SpellType { 
    ray,
    ball,
    buff,
    structure}
    public enum CastType
    {
        auto,
        semi
    }

    
    [Header("Spell options")]
    [SerializeField] private SpellType spell_Type;
    [SerializeField] private CastType cast_Type;
    [SerializeField] private int ammoSpace = 1;
    [SerializeField] private float velocity = 1f;
    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private float shootDelay = 0.5f;
    [SerializeField] private bool producesLine = false;

    [Header("Spell particles")]
    [SerializeField] private GameObject? spawnPrefab;
    [SerializeField] private GameObject? producedParticle;
    [SerializeField] private GameObject? hitParticle;

    [Header("Spell sound")]
    [SerializeField] private AudioSource? spawnSound;
    [SerializeField] private AudioSource? airSound;
    [SerializeField] private AudioSource? hitSound;

    public bool canCast = true;
    Coroutine CastingSpellCoroutine;
    public SpellType spellType { get => spell_Type; set => spell_Type = value; }
    public CastType castType { get => cast_Type; set => cast_Type = value; }
    public int AmmoSpace { get => ammoSpace; set => ammoSpace = value; }
    public float Velocity { get => velocity; set => velocity = value; }
    public float LifeTime { get => lifeTime; set => lifeTime = value; }
    public float ShootDelay { get => shootDelay; set => shootDelay = value; }
    public bool ProducesLine { get => producesLine; set => producesLine = value; }
    public GameObject? SpawnPrefab { get => spawnPrefab; set => spawnPrefab = value; }
    public GameObject? ProducedParticle { get => producedParticle; set => producedParticle = value; }
    public GameObject? HitParticle { get => hitParticle; set => hitParticle = value; }
    public AudioSource? SpawnSound { get => spawnSound; set => spawnSound = value; }
    public AudioSource? AirSound { get => airSound; set => airSound = value; }
    public AudioSource? HitSound { get => hitSound; set => hitSound = value; }

    public void createLine(Vector3 posicionInicio, Ray ray, RaycastHit hit)
    {
        if (producedParticle == null) return;
        
        if (producedParticle.TryGetComponent<LineRenderer>(out var lineRendered) == false) return;
            
      
        GameObject particula = Instantiate(producedParticle);
        LineRenderer objectLineRenderer = producedParticle.GetComponent<LineRenderer>();

        objectLineRenderer.SetPosition(0, posicionInicio);
        if (hit.point != new Vector3(0, 0, 0))
        {
            // Si choca, el fin es el punto de impacto
            objectLineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            // Si no choca, el fin es la distancia máxima
            objectLineRenderer.SetPosition(1, ray.direction * LifeTime);
        }
        Destroy(particula, 0.3f);
    }
    public virtual void LanzarHechizo(Transform spellSpawn,GameObject spell, LayerMask layersToHit) {

        if (canCast == false) return;

        
        Ray ray = new Ray(spellSpawn.position, spellSpawn.transform.TransformDirection(Vector3.forward));
        SpellBase ActualSpell = spell.GetComponent<SpellBase>();
        CastingSpellCoroutine = StartCoroutine(ActualSpell.CastingSpell());
        RaycastHit hit;

        if (Physics.Raycast(spellSpawn.position, spellSpawn.transform.TransformDirection(Vector3.forward), out hit, ActualSpell.LifeTime, layersToHit))
        {
            Debug.DrawRay(spellSpawn.position, spellSpawn.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);

            Debug.Log("ObjetoGolpeado");
        }
        if (ActualSpell.ProducesLine) ActualSpell.createLine(spellSpawn.position, ray, hit);
    }
    public virtual IEnumerator CastingSpell()
    {
        canCast = false;
        yield return new WaitForSeconds(1);
        canCast = true;
    }
}
