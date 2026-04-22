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
    //public SpellType spellType { get => spell.spell_Type; set => spell.spell_Type = value; }
    //public CastType castType { get => spell.cast_Type; set => spell.cast_Type = value; }
    //public int AmmoSpace { get => spell.ammoSpace; set => spell.ammoSpace = value; }
    //public float Velocity { get => spell.velocity; set => spell.velocity = value; }
    //public float LifeTime { get => spell.lifeTime; set => spell.lifeTime = value; }
    //public float ShootDelay { get => spell.shootDelay; set => spell.shootDelay = value; }
    //public bool ProducesLine { get => spell.producesLine; set => spell.producesLine = value; }
    //public GameObject? SpawnPrefab { get => spell.spawnPrefab; set => spell.spawnPrefab = value; }
    //public GameObject? ProducedParticle { get => spell.producedParticle; set => spell.producedParticle = value; }
    //public GameObject? HitParticle { get => spell.hitParticle; set => spell.hitParticle = value; }
    //public AudioSource? SpawnSound { get => spell.spawnSound; set => spell.spawnSound = value; }
    //public AudioSource? AirSound { get => spell.airSound; set => spell.airSound = value; }
    //public AudioSource? HitSound { get => spell.hitSound; set => spell.hitSound = value; }
    //public float Damage { get => spell.damage; set => spell.damage = value; }
    //public float SpreadIntensity { get => spell.spreadIntensity; set => spell.spreadIntensity = value; }
    //public int CurrentAmmo { get => spell.currentAmmo; set => spell.currentAmmo = value; }
    //public float ReloadTime { get => spell.reloadTime; set => spell.reloadTime = value; }
    //public int MaxCharge1 { get => spell.MaxCharge; set => spell.MaxCharge = value; }
    //public int CurrentCharge { get => spell.currentCharge; set => spell.currentCharge = value; }
    //public float ChargeTimePerUnit1 { get => spell.ChargeTimePerUnit; set => spell.ChargeTimePerUnit = value; }
    //public int SlotCost { get => spell.CosteSlots;set => spell.CosteSlots = value; }
    //public string Name { get => spell.nombreHechizo; set => spell.nombreHechizo = value; }

    private ICharacterService _characterService;
    private void Awake()
    {
        _characterService = AppContainer.Get<ICharacterService>();
    }

    public virtual void LanzarHechizo(Transform spellSpawn, SpellBase spell, LayerMask layersToHit) 
    {
        if(canCast && !isCasting && spell.spell.currentAmmo > 0)
        {
            
            canCast = false;
            isCasting = true;

            switch (spell.spell.spell_Type)
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

            Invoke("ResetCast", spell.spell.shootDelay);
        }
        if (spell.spell.currentAmmo == 0) Debug.Log("No tenes munbicion pive");
    }

    public virtual IEnumerator CargarHechizo()
    {
        do
        {
            yield return new WaitForSeconds(spell.ChargeTimePerUnit);
            spell.currentCharge++;
        } while (spell.MaxCharge > spell.currentCharge++);
        Debug.Log("Carga maxima");
    }
    public virtual void CastRaySpell(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    {   
        if(spell.spell.cast_Type == CastType.charged)
        {
            if (spell.spell.currentCharge == spell.spell.MaxCharge)
            {
                spell.spell.currentAmmo--;
                ShootRaySpell(spellSpawn, spell, layersToHit);
                spell.spell.currentCharge = 0;
            }
            else spell.spell.currentCharge = 0;
        }
        else
        {
            spell.spell.currentAmmo--;
            ShootRaySpell(spellSpawn, spell, layersToHit);
        }
            
    }

    private void ShootRaySpell(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    {
        RaycastHit hit;

    Vector3 direction = CalculateDispersion(spellSpawn.forward);
    Vector3 endPoint;

    if (Physics.Raycast(spellSpawn.position, direction, out hit, spell.spell.lifeTime, layersToHit))
    {
        endPoint = hit.point;
        Debug.Log("ObjetoGolpeado");
    }
    else
    {
        endPoint = spellSpawn.position + direction * spell.spell.lifeTime;
    }

    if (spell.spell.producesLine)
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
        float xDispersiom = UnityEngine.Random.Range(spell.spreadIntensity, -spell.spreadIntensity);
        float yDispersiom = UnityEngine.Random.Range(spell.spreadIntensity, -spell.spreadIntensity);

        Vector3 dispersion = new Vector3(xDispersiom, yDispersiom, 0);
        return vector3 + dispersion;
    }

    public virtual void Reload()
    {
        spell.currentAmmo = spell.ammoSpace;
    }

    public virtual void ResetCast()
    {
        isCasting = false;
        canCast = true;
    }
}
