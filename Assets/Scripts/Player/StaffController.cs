using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using static SpellBase;

public class StaffController : NetworkBehaviour
{
    [Header("Configuracion de hechizo")]
    //[SerializeField] private SpellBase[] spellList;
    //[SerializeField] private SpellBase Actualspell;
    [SerializeField] private Transform spellSpawn;
    [SerializeField] private GameObject ballPrefab;


    [Header("Configuracion de Objetos")]
    [SerializeField] private LayerMask layersToHit;
    [Header("prueba sonido")]
    //private IAudioService _audioService;
    private IEventService _eventService;
    private ISpellService _spellService;
    private ICharacterService _characterService;
    private IAudioService _audioService;
    private Coroutine _coroutineCharge;
    private Coroutine _coroutineReload;

    private SpellBase _currentCastingSpell;

    private bool HasAuthority => !IsSpawned || IsOwner;

    void Awake()
    {
        _eventService = AppContainer.Get<IEventService>();
        _spellService = AppContainer.Get<ISpellService>();
        _characterService = AppContainer.Get<ICharacterService>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (HasAuthority)
            SusEvents();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (HasAuthority)
            DesEvents();
    }

    private void OnEnable()
    {
        if (!IsSpawned)
            SusEvents();
    }

    private void OnDisable()
    {
        if (!IsSpawned)
            DesEvents();
    }

    private void SusEvents()
    {
        _eventService.Subscribe<ReloadEvent>(OnReloadStarted);
        _eventService.Subscribe<SpellChangeEvent>(OnSpellChanged);
    }

    private void DesEvents()
    {
        _eventService.Unsubscribe<ReloadEvent>(OnReloadStarted);
        _eventService.Unsubscribe<SpellChangeEvent>(OnSpellChanged);
    }

    private void OnReloadStarted(GameEventBase parameters)
    {
        if (!HasAuthority) return;

        if (_coroutineReload != null) return;
        if (_characterService.getSpell(_characterService.getIndex()) == null) return;
        _coroutineReload = StartCoroutine(_characterService.getSpell(_characterService.getIndex())?.Reload());
    }

    private void OnSpellChanged(GameEventBase parameters)
    {
        if (!HasAuthority) return;

        SpellChangeEvent parametrosSpellChange = (SpellChangeEvent)parameters;
        _characterService.setActualSpell(parametrosSpellChange.cambio);
    }

    void Update()
    {
        if (!HasAuthority) return;

        SpellBase ActualSpell = _characterService.getSpell(_characterService.getIndex())?.GetComponent<SpellBase>();
        if (ActualSpell == null) return;

        switch (ActualSpell.spell.cast_Type)
        {
            case CastType.auto:
                if (PlayerInputManager.Actions.Player.Attack.IsPressed())
                    LanzarHechizo(ActualSpell);
                break;

            case CastType.semi:
                if (PlayerInputManager.Actions.Player.Attack.WasPressedThisFrame())
                    LanzarHechizo(ActualSpell);
                break;

            case CastType.charged:
                if (PlayerInputManager.Actions.Player.Attack.WasPressedThisFrame())
                    CargarHechizo(ActualSpell);
                if (PlayerInputManager.Actions.Player.Attack.WasReleasedThisFrame())
                    LanzarHechizo(ActualSpell);
                break;
        }
    }

    private void LanzarHechizo(SpellBase ActualSpell)
    {
        if (_coroutineCharge != null)
        {
            StopCoroutine(_coroutineCharge);
            _coroutineCharge = null;
            ActualSpell.stopCharginSound();
        }

        if (_coroutineReload != null) StopCoroutine(_coroutineReload);
        _coroutineReload = null;

        ActualSpell.LanzarHechizo(spellSpawn, ActualSpell, layersToHit);
    }

    private void CargarHechizo(SpellBase ActualSpell)
    {
        if (_coroutineReload != null) StopCoroutine(_coroutineReload);
        _coroutineReload = null;

        if (_coroutineCharge != null) return;
        _coroutineCharge = StartCoroutine(ActualSpell.CargarHechizo());
    }
}