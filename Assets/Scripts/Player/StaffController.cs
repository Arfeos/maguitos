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

    private SpellBase ActualSpell => _characterService?.getSpell(_characterService.getIndex())?.GetComponent<SpellBase>();

    private bool HasLocalAuthority => !IsSpawned || IsOwner;

    void Awake()
    {
        _eventService = AppContainer.Get<IEventService>();
        _spellService = AppContainer.Get<ISpellService>();
        _characterService = AppContainer.Get<ICharacterService>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (HasLocalAuthority)
            SuscribirEventos();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (HasLocalAuthority)
            DesuscribirEventos();
    }

    private void OnEnable()
    {
        if (!IsSpawned)
            SuscribirEventos();
    }

    private void OnDisable()
    {
        if (!IsSpawned)
            DesuscribirEventos();
    }

    private void SuscribirEventos()
    {
        _eventService.Subscribe<ReloadEvent>(OnReloadStarted);
        _eventService.Subscribe<SpellChangeEvent>(OnSpellChanged);
    }

    private void DesuscribirEventos()
    {
        if (_eventService == null) return;
        _eventService.Unsubscribe<ReloadEvent>(OnReloadStarted);
        _eventService.Unsubscribe<SpellChangeEvent>(OnSpellChanged);
    }

    private void OnReloadStarted(GameEventBase parameters)
    {
        if (!HasLocalAuthority) return;
        if (_coroutineReload != null) return;
        if (ActualSpell == null) return;
        _coroutineReload = StartCoroutine(ActualSpell.Reload());
    }

    private void OnSpellChanged(GameEventBase parameters)
    {
        if (!HasLocalAuthority) return;
        SpellChangeEvent parametrosSpellChange = (SpellChangeEvent)parameters;
        _characterService.setActualSpell(parametrosSpellChange.cambio);
    }

    void Update()
    {
        if (!HasLocalAuthority) return;
        if (ActualSpell == null) return;

        switch (ActualSpell.spell.cast_Type)
        {
            case CastType.auto:
                if (PlayerInputManager.Actions.Player.Attack.IsPressed())
                    LanzarHechizoStaff(ActualSpell);
                break;
            case CastType.semi:
                if (PlayerInputManager.Actions.Player.Attack.WasPressedThisFrame())
                    LanzarHechizoStaff(ActualSpell);
                break;
            case CastType.charged:
                if (PlayerInputManager.Actions.Player.Attack.WasPressedThisFrame())
                    CargarHechizo(ActualSpell);
                if (PlayerInputManager.Actions.Player.Attack.WasReleasedThisFrame())
                    LanzarHechizoStaff(ActualSpell);
                break;
        }
    }

    private void LanzarHechizoStaff(SpellBase spell)
    {
        if (_coroutineCharge != null)
        {
            StopCoroutine(_coroutineCharge);
            _coroutineCharge = null;
            spell.stopCharginSound();
        }
        if (_coroutineReload != null)
        {
            StopCoroutine(_coroutineReload);
            _coroutineReload = null;
        }

        if (!spell.CanLaunch()) return;

        if (IsSpawned)
        {
            switch (spell.spell.spell_Type)
            {
                case SpellType.ray:
                    CastRaySpellRpc(spellSpawn.position, spellSpawn.forward, layersToHit);
                    break;
                case SpellType.ball:
                    Debug.Log($"[CLIENT {NetworkManager.Singleton.LocalClientId}] Enviando CastBallSpellRpc — IsSpawned: {IsSpawned}");
                    CastBallSpellRpc(spellSpawn.position, spellSpawn.forward, spell.spell.velocity);
                    break;
            }
            spell.ConsumeAndCooldown();
        }
        else
        {
            switch (spell.spell.spell_Type)
            {
                case SpellType.ray:
                    spell.CastRaySpell(spellSpawn, spell, layersToHit);
                    break;
                case SpellType.ball:
                    spell.CastBallSpell(spellSpawn, spell, layersToHit);
                    break;
            }
        }
    }

    private void CargarHechizo(SpellBase spell)
    {
        if (_coroutineReload != null)
        {
            StopCoroutine(_coroutineReload);
            _coroutineReload = null;
        }
        if (_coroutineCharge != null) return;
        _coroutineCharge = StartCoroutine(spell.CargarHechizo());
    }

    [Rpc(SendTo.Server)]
    private void CastRaySpellRpc(Vector3 spawnPos, Vector3 spawnForward, int layersToHitValue)
    {
        ActualSpell?.ExecuteRaySpellLogic(spawnPos, spawnForward, layersToHitValue);

        Vector3 endPoint = spawnPos + spawnForward * (ActualSpell?.spell.lifeTime ?? 100f);
        VisualRayEffectRpc(spawnPos, endPoint);
    }

    [Rpc(SendTo.Everyone)]
    private void VisualRayEffectRpc(Vector3 from, Vector3 to)
    {
        ActualSpell?.VisualRayEffect(from, to);
    }

    [Rpc(SendTo.Server)]
    private void CastBallSpellRpc(Vector3 spawnPos, Vector3 spawnForward, float velocity)
    {
        Debug.Log($"[SERVIDOR] CastBallSpellRpc recibido");

        var spellService = AppContainer.Get<ISpellService>();
        spellService.ShootBall(spawnPos, spawnForward, velocity, null);
        VisualBallEffectRpc(spawnPos, spawnForward, velocity);
    }

    [Rpc(SendTo.NotServer)]
    private void VisualBallEffectRpc(Vector3 spawnPos, Vector3 spawnForward, float velocity)
    {
        Debug.Log($"[CLIENT {NetworkManager.Singleton.LocalClientId}] VisualBallEffectRpc recibido — IsServer: {IsServer}");

        var spellService = AppContainer.Get<ISpellService>();
        spellService.ShootBall(spawnPos, spawnForward, velocity, new List<Material>());
    }
}