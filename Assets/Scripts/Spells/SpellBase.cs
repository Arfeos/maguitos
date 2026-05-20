using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SpellBase : MonoBehaviour
{
    public SpellBaseScriptable spell;

    private bool canCast = true;
    private bool isCasting = false;
    IAudioService _audioService;
    Coroutine CastingSpellCoroutine;

    private ICharacterService _characterService;

    private void Awake()
    {
        _characterService = AppContainer.Get<ICharacterService>();
        _audioService = AppContainer.Get<IAudioService>();
    }

    public void ResetSpellShot()
    {
        canCast = true;
        isCasting = false;
    }

    public bool CanLaunch()
    {
        if (_characterService == null) _characterService = AppContainer.Get<ICharacterService>();
        return canCast && !isCasting && _characterService.CheckMana() >= spell.manaCost;
    }

    public void ConsumeAndCooldown()
    {
        if (_characterService == null) _characterService = AppContainer.Get<ICharacterService>();
        canCast = false;
        isCasting = true;
        _characterService.RemoveMana(spell.manaCost);

        if (_audioService == null) _audioService = AppContainer.Get<IAudioService>();
        if (spell.spawnSound != null)
            _audioService.PlaySound(spell.spawnSound);

        Invoke("ResetCast", spell.shootDelay);
    }

    public virtual IEnumerator Reload()
    {
        if (_characterService == null) _characterService = AppContainer.Get<ICharacterService>();
        do
        {
            yield return new WaitForSeconds(0.1f);
            _characterService.AddMana(1);
        } while (_characterService.CheckMana() <= _characterService.getMaxMana());
    }

    public virtual IEnumerator CargarHechizo()
    {
        spell.currentCharge = 0;
        if (spell.chargeSound != null)
        {
            _audioService = AppContainer.Get<IAudioService>();
            if (_audioService != null)
                _audioService.PlayLoopSound(spell.chargeSound);
            Debug.Log("CHEQUANDO");

        }
        while (spell.MaxCharge > spell.currentCharge)
        {
            yield return new WaitForSeconds(spell.ChargeTimePerUnit);
            spell.currentCharge++;
        }
        stopCharginSound();
    }

    public virtual void CastRaySpell(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    {
        if (spell.spell.cast_Type == CastType.charged)
        {
            if (spell.spell.currentCharge == spell.spell.MaxCharge)
            {
                ExecuteRaySpellLogic(spellSpawn.position, spellSpawn.forward, layersToHit);
                spell.spell.currentCharge = 0;
            }
            else
            {
                spell.spell.currentCharge = 0;
            }
        }
        else
        {
            _characterService.RemoveMana(spell.spell.manaCost);
            ExecuteRaySpellLogic(spellSpawn.position, spellSpawn.forward, layersToHit);
        }
    }

    public virtual void CastBallSpell(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    {
        if (spell.spell.cast_Type == CastType.charged)
        {
            if (spell.spell.currentCharge == spell.spell.MaxCharge)
            {
                ExecuteBallSpellLogic(spellSpawn.position, spellSpawn.forward);
                spell.spell.currentCharge = 0;
            }
            else
            {
                spell.spell.currentCharge = 0;
            }
        }
        else
        {
            _characterService.RemoveMana(spell.spell.manaCost);
            ExecuteBallSpellLogic(spellSpawn.position, spellSpawn.forward);
        }
    }

    public void ExecuteRaySpellLogic(Vector3 spawnPos, Vector3 spawnForward, LayerMask layersToHit)
    {
        int nivelDePenetracion = spell.penetrationlevel;
        Vector3 direction = CalculateDispersion(spawnForward);
        Vector3 endPoint;

        if (spell.penetrates)
        {
            RaycastHit[] hits = Physics.RaycastAll(spawnPos, direction, spell.lifeTime, layersToHit);
            endPoint = hits.Count() >= nivelDePenetracion
                ? hits[nivelDePenetracion - 1].point
                : spawnPos + direction * spell.lifeTime;

            foreach (RaycastHit _hit in hits)
            {
                if (_hit.collider.gameObject.GetComponent<IHittable>() != null)
                {
                    _hit.collider.gameObject.GetComponent<IHittable>().Hit(spell.damage);
                    Debug.Log("ObjetoGolpeado");
                }
            }
        }
        else
        {
            if (Physics.Raycast(spawnPos, direction, out RaycastHit hit, spell.lifeTime, layersToHit))
            {
                endPoint = hit.point;
                if (hit.collider.gameObject.GetComponent<IHittable>() != null)
                {
                    hit.collider.gameObject.GetComponent<IHittable>().Hit(spell.damage);
                    Debug.Log("ObjetoGolpeado");
                }
            }
            else
            {
                endPoint = spawnPos + direction * spell.lifeTime;
            }
        }

        //COMPROBAR OFFLINE
        VisualRayEffect(spawnPos, endPoint);
    }

    public void VisualRayEffect(Vector3 from, Vector3 to)
    {
        if (spell.producesLine)
        {
            var spellService = AppContainer.Get<ISpellService>();
            if (spell.RayMaterial == null)
                spellService.ShootRay(from, to);
            else
                spellService.ShootRay(from, to, spell.RayMaterial);
        }
    }

    public void ExecuteBallSpellLogic(Vector3 spawnPos, Vector3 spawnForward)
    {
        var spellService = AppContainer.Get<ISpellService>();
        spellService.ShootBall(spawnPos, spawnForward, _characterService.getSpell(_characterService.getIndex()).spell.velocity, spell.RayMaterial);
    }

    public void VisualBallEffect(Vector3 spawnPos, Vector3 spawnForward)
    {
        var spellService = AppContainer.Get<ISpellService>();
        spellService.ShootBall(spawnPos, spawnForward, spell.velocity, spell.RayMaterial);
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