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


    //private void LanzarHechizo(SpellBase ActualSpell)
    //{
    //    if (_coroutineCharge != null) { 
    //    StopCoroutine(_coroutineCharge);
    //    _coroutineCharge = null;
    //        ActualSpell.stopCharginSound();
    //    }
    //    if (_coroutineReload != null) StopCoroutine(_coroutineReload);
    //    _coroutineReload = null;


    //    ActualSpell.LanzarHechizoBase(spellSpawn, ActualSpell, layersToHit);

    //    DibujarEfectoServerRpc(
    //    spellSpawn.position,
    //    spellSpawn.rotation,
    //    _characterService.getIndex()
    //    );


    //    //ActualSpell.LanzarHechizo(spellSpawn, ActualSpell, layersToHit);
    //}


    //ACORDARSE DE MODIFICARLO PARA HACERLO EN LA NETWORK Y NO EN EL CLIENTE
    private void LanzarHechizo(SpellBase ActualSpell)
    {
        if (!ActualSpell.canCast || ActualSpell.isCasting)
            return;

        if (_characterService.CheckMana() < ActualSpell.spell.manaCost)
            return;

        if (_coroutineCharge != null)
            StopCoroutine(_coroutineCharge);

        _coroutineCharge = null;

        if (_coroutineReload != null)
            StopCoroutine(_coroutineReload);

        _coroutineReload = null;

        ActualSpell.canCast = false;
        ActualSpell.isCasting = true;

        _currentCastingSpell = ActualSpell;

        Invoke(nameof(ResetLocalCast), ActualSpell.spell.shootDelay);

        LanzarHechizoRpc(
            spellSpawn.position,
            spellSpawn.rotation,
            spellSpawn.forward,
            _characterService.getIndex()
        );
    }
    //    if (!ActualSpell.canCast || ActualSpell.isCasting) return;
    //    if (_characterService.CheckMana() < ActualSpell.spell.manaCost) return;

    //    //if (_audioService != null)
    //    //    _audioService.PlaySound(_audioClip);

    //    if (_coroutineCharge != null) StopCoroutine(_coroutineCharge);
    //    _coroutineCharge = null;
    //    if (_coroutineReload != null) StopCoroutine(_coroutineReload);
    //    _coroutineReload = null;

    //    _characterService.RemoveMana(ActualSpell.spell.manaCost);
    //    //ActualSpell.ResetSpellShot();
    //    //StartCoroutine(ResetCastAfterDelay(ActualSpell, ActualSpell.spell.shootDelay));

    //    //ActualSpell.isCasting = true;
    //    ActualSpell.canCast = false;
    //    ActualSpell.isCasting = true;
    //    _currentCastingSpell = ActualSpell;
    //    Invoke("ResetLocalCast", ActualSpell.spell.shootDelay);
    //    //if (_coroutineReload != null) StopCoroutine(_coroutineReload);
    //    //_coroutineReload = null;

    //    //ActualSpell.LanzarHechizo(spellSpawn, ActualSpell, layersToHit);


    //    switch (ActualSpell.spell.spell_Type)
    //    {
    //        case SpellType.ray:
    //            DrawRayEffectRpc(spellSpawn.position, spellSpawn.rotation, _characterService.getIndex());
    //            break;
    //        case SpellType.ball:
    //            DrawBallRpc(spellSpawn.position,spellSpawn.forward,ActualSpell.spell.velocity,_characterService.getIndex()
    //        );
    //            break;
    //    }
    //}

    [Rpc(SendTo.Server)]
    private void RequestCastSpellRpc(
        Vector3 position,
        Quaternion rotation,
        Vector3 direction,
        int spellIndex)
    {
        SpellBase spell =
            _characterService
            .getSpell(spellIndex)
            ?.GetComponent<SpellBase>();

        if (spell == null)
            return;

        if (!spell.canCast)
            return;

        if (_characterService.CheckMana() < spell.spell.manaCost)
            return;

        _characterService.RemoveMana(spell.spell.manaCost);

        ExecuteSpellServer(spell, position, rotation, direction);
    }

    private void ExecuteSpellServer(SpellBase spell,Vector3 position,Quaternion rotation,Vector3 direction)
    {
        switch (spell.spell.spell_Type)
        {
            case SpellType.ray:
                ExecuteRay(spell, position, rotation);
                break;

            case SpellType.ball:
                ExecuteBall(spell, position, direction);
                break;
        }
    }

    private void ExecuteRay(SpellBase spell,Vector3 position,Quaternion rotation)
    {
        Vector3 dir = rotation * Vector3.forward;
        Vector3 endPoint;

        if (Physics.Raycast(position, dir, out RaycastHit hit, spell.spell.lifeTime, layersToHit))
        {
            endPoint = hit.point;

            if (hit.collider.TryGetComponent<IHittable>(out var h))
            {
                h.Hit(spell.spell.damage);
            }
        }
        else
        {
            endPoint = position + dir * spell.spell.lifeTime;
        }

        DrawRayClientRpc(position, endPoint);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DrawRayClientRpc(Vector3 start, Vector3 end)
    {
        var spellService = AppContainer.Get<ISpellService>();
        spellService.ShootRay(start, end);
    }



    //[Rpc(SendTo.Server)]
    //private void LanzarHechizoRpc(Vector3 position,Quaternion rotation,Vector3 direction,int spellIndex)
    //{
    //    SpellBase actualSpell = _characterService.getSpell(spellIndex)?.GetComponent<SpellBase>();

    //    if (actualSpell == null)
    //        return;

    //    // gasto de mana SOLO en server
    //    _characterService.RemoveMana(actualSpell.spell.manaCost);

    //    actualSpell.ExecuteSpell(

    //    position,
    //    rotation,
    //    direction,
    //    this,
    //    layersToHit
    //);
    //switch (actualSpell.spell.spell_Type)
    //{
    //    case SpellType.ray:

    //        DrawRayEffect(actualSpell,position,rotation);
    //        break;

    //    case SpellType.ball:
    //        SpawnBall(actualSpell,position,direction);
    //        break;
    //}


    private void DrawRayEffect(
        SpellBase actualSpell,
        Vector3 position,
        Quaternion rotation)
    {
        Vector3 direction =
            rotation * Vector3.forward;

        Vector3 endPoint;

        if (Physics.Raycast(
            position,
            direction,
            out RaycastHit hit,
            actualSpell.spell.lifeTime,
            layersToHit))
        {
            endPoint = hit.point;

            IHittable hittable =
                hit.collider.GetComponent<IHittable>();

            if (hittable != null)
            {
                hittable.Hit(actualSpell.spell.damage);
            }
        }
        else
        {
            endPoint =
                position +
                direction * actualSpell.spell.lifeTime;
        }

        DrawRayEffectRpc(
            position,
            endPoint
        );
    }

    [Rpc(SendTo.Server)]
    private void DrawRayEffectRpc(Vector3 start,Vector3 end)
    {
        var spellService = AppContainer.Get<ISpellService>();
        spellService.ShootRay(start, end);
    }

    private void SpawnBall(SpellBase actualSpell,Vector3 position,Vector3 direction)
    {
        GameObject ball = Instantiate(
                ballPrefab,
                position,
                Quaternion.identity
            );

        NetworkObject networkObject =
            ball.GetComponent<NetworkObject>();

        networkObject.Spawn();

        Rigidbody rb =
            ball.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity =
                direction.normalized *
                actualSpell.spell.velocity;
        }

        ApplyMaterialRpc(
            networkObject.NetworkObjectId,
            _characterService.getIndex()
        );
    }

    //Vector3 position, Quaternion rotation, int spellIndex)
    //{
    //    SpellBase ActualSpell = _characterService.getSpell(spellIndex)?.GetComponent<SpellBase>();
    //    if (ActualSpell == null) return;

    //    Vector3 direction = rotation * Vector3.forward;
    //    Vector3 endPoint;

    //    if (Physics.Raycast(position, direction, out RaycastHit hit,ActualSpell.spell.lifeTime, layersToHit))
    //    {
    //        endPoint = hit.point;
    //        if (hit.collider.gameObject.GetComponent<IHittable>() != null)
    //        {
    //            var hittable = hit.collider.gameObject.GetComponent<IHittable>();
    //            hittable.Hit(ActualSpell.spell.damage);
    //        }
    //    }
    //    else
    //    {
    //        endPoint = position + direction * ActualSpell.spell.lifeTime;
    //    }
    //    DrawRayRpc(position, endPoint);
    //}

    //[Rpc(SendTo.ClientsAndHost)]
    //private void DrawRayRpc(Vector3 start, Vector3 end)
    //{
    //    var spellService = AppContainer.Get<ISpellService>();
    //    spellService.ShootRay(start, end);
    //}

    //[Rpc(SendTo.Server)]
    //private void DrawBallRpc(Vector3 position, Vector3 direction, float velocity, int spellIndex)
    //{
    //    GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity);
    //    ball.GetComponent<NetworkObject>().Spawn();

    //    Rigidbody rb = ball.GetComponent<Rigidbody>();
    //    if (rb != null)
    //        rb.AddForce(direction.normalized * velocity, ForceMode.Impulse);

    //    ApplyMaterialRpc(ball.GetComponent<NetworkObject>().NetworkObjectId, spellIndex);
    //}

    [Rpc(SendTo.ClientsAndHost)]
    public void ApplyMaterialRpc(ulong ballNetworkId, int spellIndex)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects
            .TryGetValue(ballNetworkId, out NetworkObject ballObject))
        {
            SpellBase spell = _characterService.getSpell(spellIndex)?.GetComponent<SpellBase>();
            if (spell == null) return;

            var renderer = ballObject.GetComponent<MeshRenderer>();
            if (renderer != null)
                renderer.materials = spell.spell.RayMaterial.ToArray();
        }
    }

    private void CargarHechizo(SpellBase ActualSpell)
    {

        if (_coroutineReload != null) StopCoroutine(_coroutineReload);
        _coroutineReload = null;
        if (_coroutineCharge != null) return;
        _coroutineCharge = StartCoroutine(ActualSpell.CargarHechizo());
    }

    private void ResetLocalCast()
    {
        if (_currentCastingSpell != null)
            _currentCastingSpell.ResetCast();
    }
}
