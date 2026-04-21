using System;
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
        structure
    }
    public enum CastType
    {
        auto,
        semi,
        charged
    }

    
    [Header("Spell options")]
    [SerializeField] private SpellType spell_Type;
    [SerializeField] private CastType cast_Type;
    [SerializeField] private int ammoSpace = 1;
    [SerializeField] private int currentAmmo = 1;
    [SerializeField] private int MaxCharge = 5;
    [SerializeField] private int currentCharge = 0;
    [SerializeField] private float ChargeTimePerUnit = 0.5f;
    [SerializeField] private float reloadTime = 1;
    [SerializeField] private float velocity = 1f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private float shootDelay = 1f;
    [SerializeField] private float spreadIntensity = 0.1f;
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
    public bool isCasting = false;


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
    public float Damage { get => damage; set => damage = value; }
    public float SpreadIntensity { get => spreadIntensity; set => spreadIntensity = value; }
    public int CurrentAmmo { get => currentAmmo; set => currentAmmo = value; }
    public float ReloadTime { get => reloadTime; set => reloadTime = value; }
    public int MaxCharge1 { get => MaxCharge; set => MaxCharge = value; }
    public int CurrentCharge { get => currentCharge; set => currentCharge = value; }
    public float ChargeTimePerUnit1 { get => ChargeTimePerUnit; set => ChargeTimePerUnit = value; }

    //public void createLine(Vector3 posicionInicio, Ray ray, RaycastHit hit)
    //{
    //    if (producedParticle == null) return;
        
    //    if (producedParticle.TryGetComponent<LineRenderer>(out var lineRendered) == false) return;
            
      
    //    GameObject particula = Instantiate(producedParticle);
    //    LineRenderer objectLineRenderer = producedParticle.GetComponent<LineRenderer>();

    //    objectLineRenderer.SetPosition(0, posicionInicio);
    //    if (hit.point != new Vector3(0, 0, 0))
    //    {
    //        // Si choca, el fin es el punto de impacto
    //        objectLineRenderer.SetPosition(1, hit.point);
    //    }
    //    else
    //    {
    //        // Si no choca, el fin es la distancia máxima
    //        objectLineRenderer.SetPosition(1, ray.direction * LifeTime);
    //    }
    //    Destroy(particula, 0.3f);
    //}
    public virtual void LanzarHechizo(Transform spellSpawn, SpellBase spell, LayerMask layersToHit) 
    {
        if(canCast && !isCasting && CurrentAmmo > 0)
        {
            
            canCast = false;
            isCasting = true;

            switch (spell.spellType)
            {
                case SpellType.ray:
                    CastRaySpell(spellSpawn, spell, layersToHit);
                    break;
                case SpellType.ball:
                    //TODO implement type of spell
                    break;
                case SpellType.buff:
                    //TODO implement type of spell
                    break;
                case SpellType.structure:
                    //TODO implement type of spell
                    break;
            }

            Invoke("ResetCast", shootDelay);
        }
        
    }

    public virtual IEnumerator CargarHechizo()
    {
        do
        {
            yield return new WaitForSeconds(ChargeTimePerUnit);
            CurrentCharge++;
        } while (MaxCharge > CurrentCharge);
        Debug.Log("Carga maxima");
    }
    public virtual void CastRaySpell(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    {   
        if(spell.castType == CastType.charged)
        {
            if (CurrentCharge == MaxCharge)
            {
                CurrentAmmo--;
                ShootRaySpell(spellSpawn, spell, layersToHit);
                CurrentCharge = 0;
            }
            else CurrentCharge = 0;
        }
        else
        {
            CurrentAmmo--;
            ShootRaySpell(spellSpawn, spell, layersToHit);
        }
            
    }

    private void ShootRaySpell(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    {
        RaycastHit hit;

    Vector3 direction = CalculateDispersion(spellSpawn.forward);
    Vector3 endPoint;

    if (Physics.Raycast(spellSpawn.position, direction, out hit, LifeTime, layersToHit))
    {
        endPoint = hit.point;
        Debug.Log("ObjetoGolpeado");
    }
    else
    {
        endPoint = spellSpawn.position + direction * LifeTime;
    }

    if (ProducesLine)
    {
        var spellService = AppContainer.Get<ISpellService>();
        spellService.ShootRay(spellSpawn.position, endPoint);
    }
    }
    //private void ShootRaySpell(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    //{
    //    Ray ray = new Ray(spellSpawn.position, spellSpawn.transform.TransformDirection(Vector3.forward));
    //    SpellBase ActualSpell = spell.GetComponent<SpellBase>();

    //    RaycastHit hit;
    //    Vector3 ShootDirection = CalculateDispersion(spellSpawn.transform.TransformDirection(Vector3.forward));

    //    if (Physics.Raycast(spellSpawn.position, ShootDirection, out hit, ActualSpell.LifeTime, layersToHit))
    //    {
    //        Debug.DrawRay(spellSpawn.position, spellSpawn.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);

    //        //TODO: Añadir hit a gameObjects
    //        Debug.Log("ObjetoGolpeado");
    //    }
    //    if (ActualSpell.ProducesLine) ActualSpell.createLine(spellSpawn.position, ray, hit);
    //}

    private Vector3 CalculateDispersion(Vector3 vector3)
    {
        float xDispersiom = UnityEngine.Random.Range(SpreadIntensity, -SpreadIntensity);
        float yDispersiom = UnityEngine.Random.Range(SpreadIntensity, -SpreadIntensity);

        Vector3 dispersion = new Vector3(xDispersiom, yDispersiom, 0);
        return vector3 + dispersion;
    }

    public virtual void Reload()
    {
        CurrentAmmo = AmmoSpace;
    }

    public virtual void ResetCast()
    {
        isCasting = false;
        canCast = true;
    }
}
