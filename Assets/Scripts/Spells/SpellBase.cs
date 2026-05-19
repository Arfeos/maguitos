using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SpellBase : NetworkBehaviour
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

    private bool IsNetworked => IsSpawned;

    private bool HasAuthority => !IsNetworked || IsOwner;

    public void ResetSpellShot()
    {
        canCast = true;
        isCasting = false;
    }

    public virtual void LanzarHechizo(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    {
        if (!HasAuthority) return;

        if (_characterService == null) _characterService = AppContainer.Get<ICharacterService>();

        if (canCast && !isCasting && _characterService.CheckMana() >= spell.spell.manaCost)
        {
            canCast = false;
            isCasting = true;

            switch (spell.spell.spell_Type)
            {
                case SpellType.ray:
                    if (IsNetworked)
                        CastRaySpellRpc(spellSpawn.position, spellSpawn.forward, layersToHit);
                    else
                        CastRaySpell(spellSpawn, spell, layersToHit);
                    break;

                case SpellType.ball:
                    Debug.Log("Suck this ball");
                    if (IsNetworked)
                        CastBallSpellRpc(spellSpawn.position, spellSpawn.forward);
                    else
                        CastBallSpell(spellSpawn, spell, layersToHit);
                    break;

                case SpellType.buff:
                    //TODO implement type of spell
                    break;
                case SpellType.structure:
                    //TODO implement type of spell
                    break;
            }

            if (_audioService == null) _audioService = AppContainer.Get<IAudioService>();
            if (spell.spell.spawnSound != null)
                _audioService.PlaySound(spell.spell.spawnSound);

            Invoke("ResetCast", spell.spell.shootDelay);
        }
        if (_characterService.CheckMana() == 0) Debug.Log("No tenes munbicion pive");
    }

    [Rpc(SendTo.Server)]
    private void CastRaySpellRpc(Vector3 spawnPos, Vector3 spawnForward, int layersToHitValue)
    {
        RaySpellLogic(spawnPos, spawnForward, layersToHitValue);

        SpawnRayEffectsRpc(spawnPos, spawnForward);
    }

    [Rpc(SendTo.Server)]
    private void CastBallSpellRpc(Vector3 spawnPos, Vector3 spawnForward)
    {
        BallSpellLogic(spawnPos, spawnForward);

        SpawnBallEffectsRpc(spawnPos, spawnForward); 
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnBallEffectsRpc(Vector3 spawnPos, Vector3 spawnForward)
    {
        var spellService = AppContainer.Get<ISpellService>();

        spellService.ShootBall(spawnPos, spawnForward, spell.velocity, spell.RayMaterial);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnRayEffectsRpc(Vector3 from, Vector3 direction)
    {
        if (spell.producesLine)
        {
            var spellService = AppContainer.Get<ISpellService>();
            Vector3 endPoint = from + direction * spell.lifeTime;
            if (spell.RayMaterial == null)
                spellService.ShootRay(from, endPoint);
            else
                spellService.ShootRay(from, endPoint, spell.RayMaterial);
        }
    }

    private void RaySpellLogic(Vector3 spawnPos, Vector3 spawnForward, int layersToHitValue)
    {
        LayerMask layersToHit = layersToHitValue;
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

        if (spell.producesLine)
        {
            var spellService = AppContainer.Get<ISpellService>();
            if (spell.RayMaterial == null)
                spellService.ShootRay(spawnPos, endPoint);
            else
                spellService.ShootRay(spawnPos, endPoint, spell.RayMaterial);
        }
    }

    private void BallSpellLogic(Vector3 spawnPos, Vector3 spawnForward)
    {
        var spellService = AppContainer.Get<ISpellService>();
        spellService.ShootBall(
            spawnPos,
            spawnForward,
            _characterService.getSpell(_characterService.getIndex()).spell.velocity,
            spell.RayMaterial
        );
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
        do
        {
            yield return new WaitForSeconds(spell.ChargeTimePerUnit);
            if (_audioService == null) _audioService = AppContainer.Get<IAudioService>();
            if (spell.chargeSound != null)
                _audioService.PlaySound(spell.chargeSound);
            spell.currentCharge++;
        } while (spell.MaxCharge > spell.currentCharge);

        Debug.Log("Carga maxima");
    }

    public virtual void CastRaySpell(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    {
        if (spell.spell.cast_Type == CastType.charged)
        {
            if (spell.spell.currentCharge == spell.spell.MaxCharge)
            {
                if (!_characterService.RemoveMana(spell.spell.manaCost))
                {
                    spell.spell.currentCharge = 0;
                    return;
                }
                ShootRaySpell(spellSpawn, spell, layersToHit);
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
                    spell.spell.currentCharge = 0;
                    return;
                }
                ShootBallSpell(spellSpawn, spell, layersToHit);
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
            ShootBallSpell(spellSpawn, spell, layersToHit);
        }
    }

    private void ShootBallSpell(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    {
        BallSpellLogic(spellSpawn.position, spellSpawn.forward);
    }

    private void ShootRaySpell(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    {
        RaySpellLogic(spellSpawn.position, spellSpawn.forward, layersToHit);
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