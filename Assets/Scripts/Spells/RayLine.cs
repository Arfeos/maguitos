using System;
using System.Collections;
using UnityEngine;

public class RayLine : MonoBehaviour
{
    private ISpellService _spellService;
    private ICharacterService _characterService;
    void Awake()
    {
        _spellService = AppContainer.Get<ISpellService>();
        _characterService = AppContainer.Get<ICharacterService>();
    }

    void OnEnable()
    {
        try
        {
            float time = _characterService.getSpell(_characterService.getIndex()).spell.RayAliveTime;
            StartCoroutine(DisableAfterTime(time));
        }
        catch (Exception e)
        {
            StartCoroutine(DisableAfterTime(0.2f));
        }
        
    }

    private IEnumerator DisableAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        _spellService.ReturnRay(gameObject);
    }
}