using UnityEngine;
using static SpellBase;

/// <summary>
/// MonoBehaviour que gestiona el lanzamiento y carga de hechizos del jugador,
/// procesando la entrada según el tipo de disparo del hechizo activo
/// (automático, semi-automático o cargado) y delegando la lógica en el <see cref="ISpellService"/>.
/// </summary>
public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Configuracion de hechizo")]
    //[SerializeField] private SpellBase[] spellList;
    //[SerializeField] private SpellBase Actualspell;
    /// <summary>Punto de origen desde el que se instancian los proyectiles de los hechizos.</summary>
    [SerializeField] private Transform spellSpawn;

    [Header("Configuracion de Objetos")]
    /// <summary>Máscara de capas con las que pueden colisionar los proyectiles lanzados.</summary>
    [SerializeField] private LayerMask layersToHit;
    
    [Header("prueba sonido")]
    //private IAudioService _audioService;
    /// <summary>Referencia al servicio de eventos para suscribirse a <see cref="ReloadEvent"/> y <see cref="SpellChangeEvent"/>.</summary>
    private IEventService _eventService;
    /// <summary>Referencia al servicio de hechizos para gestionar el lanzamiento y pool de proyectiles.</summary>
    private ISpellService _spellService;
    /// <summary>Referencia al servicio de personaje para obtener el hechizo activo, el maná y el estado pacifista.</summary>
    private ICharacterService _characterService;
    /// <summary>Corrutina activa de carga del hechizo, usada para evitar cargas simultáneas y poder cancelarla.</summary>
    private Coroutine _coroutineCharge;
    /// <summary>Corrutina activa de recarga del hechizo, usada para evitar recargas simultáneas y poder cancelarla.</summary>
    private Coroutine _coroutineReload;


    /// <summary>
    /// Obtiene las referencias a los servicios necesarios al inicializarse el componente.
    /// </summary>
    void Awake()
    {
        //PlayerInputManager.Actions.Player.Reload.started += OnReloadStarted;
        //_audioService = AppContainer.Get<IAudioService>();
        _eventService = AppContainer.Get<IEventService>();
        _spellService = AppContainer.Get<ISpellService>();
        _characterService = AppContainer.Get<ICharacterService>();

        
    }

    /// <summary>
    /// Se suscribe a los eventos <see cref="ReloadEvent"/> y <see cref="SpellChangeEvent"/>
    /// al activarse el componente.
    /// </summary>
    private void OnEnable()
    {
        _eventService.Subscribe<ReloadEvent>(OnReloadStarted);
        _eventService.Subscribe<SpellChangeEvent>(OnSpellChanged);
    }

    /// <summary>
    /// Cancela la suscripción a los eventos <see cref="ReloadEvent"/> y <see cref="SpellChangeEvent"/>
    /// al desactivarse el componente.
    /// </summary>
    private void OnDisable()
    {
        _eventService.Unsubscribe<ReloadEvent>(OnReloadStarted);
        _eventService.Unsubscribe<SpellChangeEvent>(OnSpellChanged);
    }

    /// <summary>
    /// Callback invocado al recibir un <see cref="ReloadEvent"/>.
    /// Inicia la corrutina de recarga del hechizo activo si no hay ya una en curso
    /// y el hechizo activo no es nulo.
    /// </summary>
    /// <param name="parameters">Evento base recibido, correspondiente a un <see cref="ReloadEvent"/>.</param>
    private void OnReloadStarted(GameEventBase parameters)
    {
        //SpellBase ActualSpell = Actualspell.GetComponent<SpellBase>();
        //ActualSpell.Invoke( "Reload", ActualSpell.spell.reloadTime);
        if (_coroutineReload != null) return;
        if (_characterService.getSpell(_characterService.getIndex()) == null) return;
        _coroutineReload = StartCoroutine(_characterService.getSpell(_characterService.getIndex())?.Reload());
    }

    /// <summary>
    /// Callback invocado al recibir un <see cref="SpellChangeEvent"/>.
    /// Actualiza el hechizo activo del personaje con el índice de cambio recibido en el evento.
    /// </summary>
    /// <param name="parameters">Evento base recibido, correspondiente a un <see cref="SpellChangeEvent"/>.</param>
    private void OnSpellChanged(GameEventBase parameters)
    {
        SpellChangeEvent parametrosSpellChange = (SpellChangeEvent)parameters;
        _characterService.setActualSpell(parametrosSpellChange.cambio);
    }

    /// <summary>
    /// Procesa la entrada del jugador cada frame según el <see cref="CastType"/> del hechizo activo:
    /// disparo continuo para automático, disparo por pulsación para semi-automático,
    /// y carga al pulsar con lanzamiento al soltar para cargado.
    /// No realiza ninguna acción si no hay hechizo activo.
    /// </summary>
    void Update()
    {
  

        SpellBase ActualSpell = _characterService.getSpell(_characterService.getIndex())?.GetComponent<SpellBase>();
        if(ActualSpell == null) return;
        switch (ActualSpell.spell.cast_Type)
        {
            case CastType.auto:
                if (PlayerInputManager.Actions.Player.Attack.IsPressed()) LanzarHechizo(ActualSpell);
                break;
            case CastType.semi:
                if (PlayerInputManager.Actions.Player.Attack.WasPressedThisFrame()) LanzarHechizo(ActualSpell);
                break;
            case CastType.charged:
                if (_characterService.CheckMana() < ActualSpell.spell.manaCost) break;
                if (PlayerInputManager.Actions.Player.Attack.WasPressedThisFrame()) CargarHechizo(ActualSpell);
                if (PlayerInputManager.Actions.Player.Attack.WasReleasedThisFrame()) LanzarHechizo(ActualSpell);
                break;
        }
    }


    /// <summary>
    /// Lanza el hechizo activo desde el punto de spawn, cancelando previamente
    /// cualquier corrutina de carga o recarga en curso.
    /// Si el jugador estaba en modo pacifista, activa el modo genocidio antes de lanzar.
    /// </summary>
    /// <param name="ActualSpell">Instancia del hechizo activo a lanzar.</param>

    private void LanzarHechizo(SpellBase ActualSpell)
    {
      if (_characterService.getPacifist())
        {
            _characterService.Genocide();
        }
        if (_coroutineCharge != null) { 
        StopCoroutine(_coroutineCharge);
        _coroutineCharge = null;
          ActualSpell.stopCharginSound();
        }
        if (_coroutineReload != null) StopCoroutine(_coroutineReload);
        _coroutineReload = null;

        ActualSpell.LanzarHechizo(spellSpawn, ActualSpell, layersToHit);
    }

    /// <summary>
    /// Inicia la corrutina de carga del hechizo activo, cancelando primero
    /// cualquier recarga en curso. Si ya hay una carga activa, no hace nada.
    /// </summary>
    /// <param name="ActualSpell">Instancia del hechizo activo a cargar.</param>
    private void CargarHechizo(SpellBase ActualSpell)
    {
      
        if (_coroutineReload != null) StopCoroutine(_coroutineReload);
        _coroutineReload = null;
        if (_coroutineCharge != null) return;
        _coroutineCharge = StartCoroutine(ActualSpell.CargarHechizo());

    }
}
