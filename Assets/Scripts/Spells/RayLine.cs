using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// MonoBehaviour que gestiona el ciclo de vida de un rayo lanzado como hechizo,
/// desactivándolo automáticamente tras el tiempo de vida definido en el hechizo activo.
/// </summary>
public class RayLine : MonoBehaviour
{
    /// <summary>Referencia al servicio de hechizos para devolver el rayo al pool al desactivarse.</summary>
    private ISpellService _spellService;

    /// <summary>Referencia al servicio de personaje para obtener el hechizo activo y su tiempo de vida.</summary>
    private ICharacterService _characterService;

    /// <summary>
    /// Obtiene las referencias a los servicios necesarios al inicializarse el componente.
    /// </summary>
    void Awake()
    {
        _spellService = AppContainer.Get<ISpellService>();
        _characterService = AppContainer.Get<ICharacterService>();
    }

    /// <summary>
    /// Al activarse el componente, inicia la corrutina que desactivará el rayo
    /// tras el tiempo de vida definido en el hechizo activo.
    /// Si no se puede obtener el tiempo de vida, usa un valor de seguridad de 0.2 segundos.
    /// </summary>
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

    /// <summary>
    /// Corrutina que espera el tiempo indicado y devuelve el rayo al pool
    /// mediante el <see cref="ISpellService"/>.
    /// </summary>
    /// <param name="time">Tiempo en segundos antes de devolver el objeto al pool.</param>
    private IEnumerator DisableAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        _spellService.ReturnRay(gameObject);
    }
}