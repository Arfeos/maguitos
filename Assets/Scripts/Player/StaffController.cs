using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using static SpellBase;
using System.Net;

public class StaffController : NetworkBehaviour
{
    [Header("Configuracion de hechizo")]
    //[SerializeField] private SpellBase[] spellList;
    //[SerializeField] private SpellBase Actualspell;
    [SerializeField] private Transform spellSpawn;

    [Header("Configuracion de Objetos")]
    [SerializeField] private LayerMask layersToHit;
    [Header("prueba sonido")]
    //private IAudioService _audioService;
    private IEventService _eventService;
    private ISpellService _spellService;
    private ICharacterService _characterService;
    private Coroutine _coroutineCharge;
    private Coroutine _coroutineReload;
    
    void Awake()
    {
        //PlayerInputManager.Actions.Player.Reload.started += OnReloadStarted;
        //_audioService = AppContainer.Get<IAudioService>();
        _eventService = AppContainer.Get<IEventService>();
        _spellService = AppContainer.Get<ISpellService>();
        _characterService = AppContainer.Get<ICharacterService>();

        
    }

    private void OnEnable()
    {
        _eventService.Subscribe<ReloadEvent>(OnReloadStarted);
        _eventService.Subscribe<SpellChangeEvent>(OnSpellChanged);
    }
    private void OnDisable()
    {
        _eventService.Unsubscribe<ReloadEvent>(OnReloadStarted);
        _eventService.Unsubscribe<SpellChangeEvent>(OnSpellChanged);
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) enabled = false;
    }
    private void OnReloadStarted(GameEventBase parameters)
    {
        //SpellBase ActualSpell = Actualspell.GetComponent<SpellBase>();
        //ActualSpell.Invoke( "Reload", ActualSpell.spell.reloadTime);
        if (_coroutineReload != null) return;
        if (_characterService.getSpell(_characterService.getIndex()) == null) return;
        _coroutineReload = StartCoroutine(_characterService.getSpell(_characterService.getIndex())?.Reload());
    }
    private void OnSpellChanged(GameEventBase parameters)
    {
        SpellChangeEvent parametrosSpellChange = (SpellChangeEvent)parameters;
        _characterService.setActualSpell(parametrosSpellChange.cambio);
    }

    void Update()
    {
        SpellBase ActualSpell = _characterService.getSpell(_characterService.getIndex())?.GetComponent<SpellBase>();


        if (ActualSpell == null) return;
        switch (ActualSpell.spell.cast_Type)
        {
            case CastType.auto:
                if (PlayerInputManager.Actions.Player.Attack.IsPressed()) LanzarHechizo(ActualSpell);
                break;
            case CastType.semi:
                if (PlayerInputManager.Actions.Player.Attack.WasPressedThisFrame()) LanzarHechizo(ActualSpell);
                break;
            case CastType.charged:
                if (PlayerInputManager.Actions.Player.Attack.WasPressedThisFrame()) CargarHechizo(ActualSpell);
                if (PlayerInputManager.Actions.Player.Attack.WasReleasedThisFrame()) LanzarHechizo(ActualSpell);
                break;
        }
            }

 


    private void LanzarHechizo(SpellBase ActualSpell)
    {
        if (_coroutineCharge != null) { 
        StopCoroutine(_coroutineCharge);
        _coroutineCharge = null;
            ActualSpell.stopCharginSound();
        }
        if (_coroutineReload != null) StopCoroutine(_coroutineReload);
        _coroutineReload = null;


        ActualSpell.LanzarHechizo(spellSpawn, ActualSpell, layersToHit);

        DibujarEfectoServerRpc(
        spellSpawn.position,
        spellSpawn.rotation,
        _characterService.getIndex()
        );


        //ActualSpell.LanzarHechizo(spellSpawn, ActualSpell, layersToHit);
    }
    
    [ServerRpc]
    private void DibujarEfectoServerRpc(Vector3 position, Quaternion rotation, int spellIndex)
    {
        Debug.Log($"[ServerRpc] Dibujando efecto");

        SpellBase ActualSpell = _characterService.getSpell(spellIndex)?.GetComponent<SpellBase>();
        if (ActualSpell == null) return;

        Vector3 direction = rotation * Vector3.forward;
        Vector3 endPoint;

        if (Physics.Raycast(position, direction, out RaycastHit hit,
            ActualSpell.spell.lifeTime, layersToHit))
        {
            endPoint = hit.point;
            if (hit.collider.gameObject.GetComponent<IHittable>() != null)
                hit.collider.gameObject.GetComponent<IHittable>().Hit(ActualSpell.spell.damage);
        }
        else
        {
            endPoint = position + direction * ActualSpell.spell.lifeTime;
        }

        DrawRayClientRpc(position, endPoint);
    }

    [ClientRpc]
    private void DrawRayClientRpc(Vector3 start, Vector3 end)
    {
        Debug.Log($"[ClientRpc] Dibujando rayo");
        var spellService = AppContainer.Get<ISpellService>();
        spellService.ShootRay(start, end);
    }


    private void CargarHechizo(SpellBase ActualSpell)
    {

        if (_coroutineReload != null) StopCoroutine(_coroutineReload);
        _coroutineReload = null;
        if (_coroutineCharge != null) return;
        _coroutineCharge = StartCoroutine(ActualSpell.CargarHechizo());
    }
}
