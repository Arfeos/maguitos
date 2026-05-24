using System.Collections;
using System.Linq;
using UnityEngine;

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
    /// <summary>
    /// Resetea el estado de lanzamiento del hechizo,
    /// permitiendo volver a castear.
    /// </summary>
    public void ResetSpellShot()
    {
        canCast = true;
        isCasting = false;
    }
    /// <summary>
    /// Intenta lanzar un hechizo si hay mana suficiente y no se está casteando.
    /// Delega en el método correspondiente según el tipo de hechizo.
    /// </summary>
    /// <param name="spellSpawn">Transform desde donde se origina el hechizo.</param>
    /// <param name="spell">Referencia al SpellBase que se va a lanzar.</param>
    /// <param name="layersToHit">Capas con las que interactúa el hechizo.</param>
    public virtual void LanzarHechizo(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    {
        if (_characterService == null) _characterService = AppContainer.Get<ICharacterService>();

        if (canCast && !isCasting && _characterService.CheckMana() >= spell.spell.manaCost)
        {
            if (_audioService == null) _audioService = AppContainer.Get<IAudioService>();
            canCast = false;
            isCasting = true;

            switch (spell.spell.spell_Type)
            {
                case SpellType.ray:
                    CastRaySpell(spellSpawn, spell, layersToHit);
                    break;
                case SpellType.ball:
                    Debug.Log("Suck this ball");
                    CastBallSpell(spellSpawn, spell, layersToHit);
                    break;
                case SpellType.buff:
                    //TODO implement type of spell
                    break;
                case SpellType.structure:
                    //TODO implement type of spell
                    break;
            }

            if (spell.spell.spawnSound != null)
                _audioService.PlaySound(spell.spell.spawnSound);
            Invoke("ResetCast", spell.spell.shootDelay);
        }
        if (_characterService.CheckMana() == 0) Debug.Log("No tenes munbicion pive");
    }
    /// <summary>
    /// Corrutina que recarga el mana del personaje gradualmente
    /// hasta alcanzar el máximo.
    /// </summary>
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
    /// <summary>
    /// Corrutina que gestiona la carga progresiva del hechizo.
    /// Reproduce sonido de carga y avanza hasta alcanzar la carga máxima.
    /// </summary>
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
        //TODO añadir sonido de carga maxima
        stopCharginSound();

        Debug.Log("Carga maxima");
    }
    /// <summary>
    /// Gestiona el lanzamiento de un hechizo de tipo rayo.
    /// Si es de tipo cargado, verifica que se haya alcanzado la carga máxima antes de disparar.
    /// </summary>
    /// <param name="spellSpawn">Transform desde donde se origina el rayo.</param>
    /// <param name="spell">Referencia al SpellBase con los datos del hechizo.</param>
    /// <param name="layersToHit">Capas con las que interactúa el rayo.</param>
    public virtual void CastRaySpell(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    {
        if (spell.spell.cast_Type == CastType.charged)
        {
            if (spell.spell.currentCharge >= spell.spell.MaxCharge)
            {


                //Lanzamos el hechizo
                ShootRaySpell(spellSpawn, spell, layersToHit);
                spell.spell.currentCharge = 0;
                _characterService.RemoveMana(spell.spell.manaCost);
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
    /// <summary>
    /// Gestiona el lanzamiento de un hechizo de tipo bola.
    /// Si es de tipo cargado, verifica carga máxima y mana suficiente antes de disparar.
    /// </summary>
    /// <param name="spellSpawn">Transform desde donde se origina la bola.</param>
    /// <param name="spell">Referencia al SpellBase con los datos del hechizo.</param>
    /// <param name="layersToHit">Capas con las que interactúa la bola.</param>
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
    /// <summary>
    /// Ejecuta el disparo físico de la bola delegando en el servicio de hechizos.
    /// </summary>
    /// <param name="spellSpawn">Transform origen del disparo.</param>
    /// <param name="spell">Referencia al SpellBase con los datos del hechizo.</param>
    /// <param name="layersToHit">Capas con las que interactúa la bola.</param>
    private void ShootBallSpell(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    {
        var spellService = AppContainer.Get<ISpellService>();
        spellService.ShootBall(spellSpawn.position, spellSpawn.transform.forward, _characterService.getSpell(_characterService.getIndex()).spell.velocity, spell.spell.RayMaterial);
    }
    /// <summary>
    /// Ejecuta el disparo de un rayo mediante Raycast, aplicando daño a los objetos golpeados.
    /// Soporta penetración de múltiples objetivos y dibuja la línea visual del rayo si está configurado.
    /// </summary>
    /// <param name="spellSpawn">Transform origen del rayo.</param>
    /// <param name="spell">Referencia al SpellBase con los datos del hechizo.</param>
    /// <param name="layersToHit">Capas con las que interactúa el rayo.</param>
    private void ShootRaySpell(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    {
        RaycastHit hit;
        int nivelDePenetracion = spell.spell.penetrationlevel;
        Vector3 direction = CalculateDispersion(spellSpawn.forward);
        Vector3 endPoint;
        if (spell.spell.penetrates)
        {
            //Raycast con penetración
            RaycastHit[] hits = Physics.RaycastAll(spellSpawn.position, direction, spell.spell.lifeTime, layersToHit);
            if (hits.Count() >= nivelDePenetracion)
            {
                endPoint = hits[nivelDePenetracion - 1].point;
            }
            else
            {
                endPoint = spellSpawn.position + direction * spell.spell.lifeTime;
            }
            foreach (RaycastHit _hit in hits)
            {
                //Habria que arreglar esto si queremos que pare al chocar con una parez, ahora mismo me reconoce todo como una pared, de momento estas hechizos atraviesan todo
                //if (_hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                //{
                //    endPoint = _hit.point;
                //    break;
                //}
                if (_hit.collider.gameObject.GetComponent<IHittable>() != null)
                {
                    _hit.collider.gameObject.GetComponent<IHittable>().Hit(spell.spell.damage);
                    Debug.Log("ObjetoGolpeado");
                }
            }
        }
        else
        {
            //Raycast sin penetracion
            if (Physics.Raycast(spellSpawn.position, direction, out hit, spell.spell.lifeTime, layersToHit))
            {
                endPoint = hit.point;
                if (hit.collider.gameObject.GetComponent<IHittable>() != null)
                {
                    hit.collider.gameObject.GetComponent<IHittable>().Hit(spell.spell.damage);
                    Debug.Log("ObjetoGolpeado");
                }

            }
            else
            {
                endPoint = spellSpawn.position + direction * spell.spell.lifeTime;
            }
        }




        //Producir la linea
        if (spell.spell.producesLine)
        {
            var spellService = AppContainer.Get<ISpellService>();
            if (spell.spell.RayMaterial == null) spellService.ShootRay(spellSpawn.position, endPoint);
            else spellService.ShootRay(spellSpawn.position, endPoint, spell.spell.RayMaterial);
        }
    }

    /// <summary>
    /// Calcula la dirección de disparo aplicando dispersión aleatoria
    /// según la intensidad de spread configurada en el hechizo.
    /// </summary>
    /// <param name="vector3">Dirección base del disparo.</param>
    /// <returns>Dirección modificada con dispersión aplicada.</returns>
    private Vector3 CalculateDispersion(Vector3 vector3)
    {
        float xDispersiom = UnityEngine.Random.Range(spell.spreadIntensity, -spell.spreadIntensity);
        float yDispersiom = UnityEngine.Random.Range(spell.spreadIntensity, -spell.spreadIntensity);

        Vector3 dispersion = new Vector3(xDispersiom, yDispersiom, 0);
        return vector3 + dispersion;
    }

    /// <summary>
    /// Restablece los flags de casteo, permitiendo lanzar
    /// el siguiente hechizo tras el delay configurado.
    /// </summary>
    public virtual void ResetCast()
    {
        isCasting = false;
        canCast = true;
    }
    /// <summary>
    /// Detiene el sonido de carga del hechizo si estaba reproduciéndose.
    /// </summary>
    public void stopCharginSound()
    {
        if (_audioService == null) _audioService = AppContainer.Get<IAudioService>();
        if (spell.chargeSound != null)
            _audioService.StopSound(spell.chargeSound);
    }
}
