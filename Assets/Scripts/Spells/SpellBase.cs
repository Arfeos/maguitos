using System.Collections;
using UnityEngine;

public partial class SpellBase : MonoBehaviour
{
    public SpellBaseScriptable spell;

    public bool canCast { get; private set; } = true; 
    public bool isCasting { get; private set; } = false;

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
    public virtual void LanzarHechizo(Transform spellSpawn, SpellBase spell, LayerMask layersToHit) 
    {
        if(_characterService == null) _characterService = AppContainer.Get<ICharacterService>();

        if (canCast && !isCasting && _characterService.CheckMana() > spell.spell.manaCost)
        {
            
            canCast = false;
            isCasting = true;

            switch (spell.spell.spell_Type)
            {
                case SpellType.ray:
                    CastRaySpell(spellSpawn, spell, layersToHit);
                    break;
                case SpellType.ball:
                    Debug.Log("Suck this ball");
                //case SpellType.ball:
                //    ActualSpell.LanzarHechizo(spellSpawn, ActualSpell, layersToHit);
                //    LanzarBolaServerRpc(
                //        spellSpawn.position,
                //        spellSpawn.forward,
                //        ActualSpell.spell.velocity,
                //        _characterService.getIndex()
                //    );
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
        if (_characterService.CheckMana() == 0) Debug.Log("No tenes munbicion pive");
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
                hit.collider.gameObject.GetComponent<IHittable>().Hit();
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
}
