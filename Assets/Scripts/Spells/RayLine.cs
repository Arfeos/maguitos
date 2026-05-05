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
        float time = _characterService.getSpell(_characterService.getIndex()).spell.RayAliveTime;
        StartCoroutine(DisableAfterTime(time));
    }

    private IEnumerator DisableAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        _spellService.ReturnRay(gameObject);
    }
}