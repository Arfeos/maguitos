using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public partial class SpellBase : MonoBehaviour
{

    
    public SpellBaseScriptable spell;

    private bool canCast = true;
    private bool isCasting = false;

#nullable enable
    Coroutine CastingSpellCoroutine;
    public SpellType spellType { get => spell.spell_Type; set => spell.spell_Type = value; }
    public CastType castType { get => spell.cast_Type; set => spell.cast_Type = value; }
    public int AmmoSpace { get => spell.ammoSpace; set => spell.ammoSpace = value; }
    public float Velocity { get => spell.velocity; set => spell.velocity = value; }
    public float LifeTime { get => spell.lifeTime; set => spell.lifeTime = value; }
    public float ShootDelay { get => spell.shootDelay; set => spell.shootDelay = value; }
    public bool ProducesLine { get => spell.producesLine; set => spell.producesLine = value; }
    public GameObject? SpawnPrefab { get => spell.spawnPrefab; set => spell.spawnPrefab = value; }
    public GameObject? ProducedParticle { get => spell.producedParticle; set => spell.producedParticle = value; }
    public GameObject? HitParticle { get => spell.hitParticle; set => spell.hitParticle = value; }
    public AudioSource? SpawnSound { get => spell.spawnSound; set => spell.spawnSound = value; }
    public AudioSource? AirSound { get => spell.airSound; set => spell.airSound = value; }
    public AudioSource? HitSound { get => spell.hitSound; set => spell.hitSound = value; }
    public float Damage { get => spell.damage; set => spell.damage = value; }
    public float SpreadIntensity { get => spell.spreadIntensity; set => spell.spreadIntensity = value; }
    public int CurrentAmmo { get => spell.currentAmmo; set => spell.currentAmmo = value; }
    public float ReloadTime { get => spell.reloadTime; set => spell.reloadTime = value; }
    public int MaxCharge1 { get => spell.MaxCharge; set => spell.MaxCharge = value; }
    public int CurrentCharge { get => spell.currentCharge; set => spell.currentCharge = value; }
    public float ChargeTimePerUnit1 { get => spell.ChargeTimePerUnit; set => spell.ChargeTimePerUnit = value; }

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

            Invoke("ResetCast", spell.ShootDelay);
        }
        
    }

    public virtual IEnumerator CargarHechizo()
    {
        do
        {
            yield return new WaitForSeconds(spell.ChargeTimePerUnit);
            CurrentCharge++;
        } while (spell.MaxCharge > CurrentCharge);
        Debug.Log("Carga maxima");
    }
    public virtual void CastRaySpell(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    {   
        if(spell.castType == CastType.charged)
        {
            if (CurrentCharge == spell.MaxCharge1)
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
