using System.Collections;
using UnityEngine;

public class RayLine : MonoBehaviour
{
    private ISpellService _spellService;

    void Awake()
    {
        _spellService = AppContainer.Get<ISpellService>();
    }

    void OnEnable()
    {
        StartCoroutine(DisableAfterTime());
    }

    private IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(0.3f);
        _spellService.ReturnRay(gameObject);
    }
}