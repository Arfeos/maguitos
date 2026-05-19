using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SpellBase : MonoBehaviour
{
    private IAudioService _audioService;
    public SpellBaseScriptable spell;

    public bool canCast { get; set; } = true; 
    public bool isCasting { get; set; } = false;

    [SerializeField] private List<Material> materials;

    Coroutine CastingSpellCoroutine;

    private ICharacterService _characterService;
    private void Awake()
    {
        _characterService = AppContainer.Get<ICharacterService>();
    }
    public void ResetSpellShot()
    {
        canCast = true;
        isCasting = false;
    }

    public virtual Vector3 ExecuteSpell(Transform spellSpawn,SpellBase spell,LayerMask layersToHit)
    {
        Vector3 direction = spellSpawn.forward;
        Vector3 endPoint;

        RaycastHit hit;

        if (Physics.Raycast(
            spellSpawn.position,
            direction,
            out hit,
            spell.spell.lifeTime,
            layersToHit))
        {
            endPoint = hit.point;

            IHittable hittable =
                hit.collider.GetComponent<IHittable>();

            if (hittable != null)
            {
                hittable.Hit(spell.spell.damage);
            }
        }
        else
        {
            endPoint =
                spellSpawn.position +
                direction * spell.spell.lifeTime;
        }

        return endPoint;
    }

    public virtual void LanzarHechizoBase(Transform spellSpawn,SpellBase spell,LayerMask layersToHit)
    {
        ExecuteSpell(spellSpawn, spell, layersToHit);
    }

    private void ResetLocalCast()
    {
        SpellBase ActualSpell = _characterService.getSpell(_characterService.getIndex())?.GetComponent<SpellBase>();
        if (ActualSpell != null) ActualSpell.ResetCast();
    }

    public virtual IEnumerator Reload()
    {
        if (_characterService == null) _characterService = AppContainer.Get<ICharacterService>();
        do
        {
            yield return new WaitForSeconds(0.1f);
            _characterService.AddMana(1);
            //Debug.Log(_characterService.CheckMana());
        } while (_characterService.CheckMana() <= _characterService.getMaxMana());
    }

    public virtual IEnumerator CargarHechizo()
    {
        do
        {
            yield return new WaitForSeconds(spell.ChargeTimePerUnit);
            spell.currentCharge++;
        } while (spell.MaxCharge > spell.currentCharge);
        //TODO añadir sonido de carga maxima

        Debug.Log("Carga maxima");
    }
    public virtual void CastRaySpell(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    {   
        if(spell.spell.cast_Type == CastType.charged)
        {
            if (spell.spell.currentCharge == spell.spell.MaxCharge)
            {
                if (!_characterService.RemoveMana(spell.spell.manaCost))
                {
                    //Si no se tiene suficiente mana para lanzar el hechizo
                    spell.spell.currentCharge = 0;
                    return;
                }
                //Lanzamos el hechizo
                ShootRaySpell(spellSpawn, spell, layersToHit);
                spell.spell.currentCharge = 0;
            }
            else
            {
                //Si el hechizo se lanza sin llegar a carga maxima
                spell.spell.currentCharge = 0;
            }
        }
        else
        {
            _characterService.RemoveMana(spell.spell.manaCost);
            ShootRaySpell(spellSpawn, spell, layersToHit);
        }
            
    }

    public virtual void CastBallSpell(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    {
        if (spell.spell.cast_Type == CastType.charged)
        {
            if (spell.spell.currentCharge == spell.spell.MaxCharge)
            {
                if (!_characterService.RemoveMana(spell.spell.manaCost))
                {
                    //Si no se tiene suficiente mana para lanzar el hechizo
                    spell.spell.currentCharge = 0;
                    return;
                }
                //Lanzamos el hechizo
                ShootBallSpell(spellSpawn, spell, layersToHit);
                spell.spell.currentCharge = 0;
            }
            else
            {
                //Si el hechizo se lanza sin llegar a carga maxima
                spell.spell.currentCharge = 0;
            }
        }
        else
        {
            _characterService.RemoveMana(spell.spell.manaCost);
            ShootBallSpell(spellSpawn, spell, layersToHit);
        }

    }
    private void ShootBallSpell(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    {
        var spellService = AppContainer.Get<ISpellService>();
        spellService.ShootBall(spellSpawn.position, spellSpawn.transform.forward , _characterService.getSpell(_characterService.getIndex()).spell.velocity, spell.spell.RayMaterial);
    }

    private void ShootRaySpell(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    {
        RaycastHit hit;

        Vector3 direction = CalculateDispersion(spellSpawn.forward);
        Vector3 endPoint;

        if (Physics.Raycast(spellSpawn.position, direction, out hit, spell.spell.lifeTime, layersToHit))
        {
            endPoint = hit.point;
            if(hit.collider.gameObject.GetComponent<IHittable>() != null)
            {
                hit.collider.gameObject.GetComponent<IHittable>().Hit(spell.spell.damage);
                Debug.Log("ObjetoGolpeado");
            }
            
        }
        else
        {
            endPoint = spellSpawn.position + direction * spell.spell.lifeTime;
        }

        if (spell.spell.producesLine)
        {
            var spellService = AppContainer.Get<ISpellService>();
            if(spell.spell.RayMaterial == null) spellService.ShootRay(spellSpawn.position, endPoint);
                else spellService.ShootRay(spellSpawn.position, endPoint, spell.spell.RayMaterial);
        }
    }

    private Vector3 CalculateDispersion(Vector3 vector3)
    {
        float xDispersiom = UnityEngine.Random.Range(spell.spreadIntensity, -spell.spreadIntensity);
        float yDispersiom = UnityEngine.Random.Range(spell.spreadIntensity, -spell.spreadIntensity);

        Vector3 dispersion = new Vector3(xDispersiom, yDispersiom, 0);
        return vector3 + dispersion;
    }


    public virtual void ResetCast()
    {
        isCasting = false;
        canCast = true;
    }
    public void stopCharginSound()
    {
        if (_audioService == null) _audioService = AppContainer.Get<IAudioService>();
        if (spell.chargeSound != null)
            _audioService.StopSound(spell.chargeSound);
    }
}