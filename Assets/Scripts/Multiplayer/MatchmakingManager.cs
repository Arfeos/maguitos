using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using System;

public class MatchmakingManager : MonoBehaviour
{
    private string _ticketId;
    [SerializeField] private RelayManager relayManager;

    private string _lobbyId;
    private Lobby _currentLobby;

    public string LobbyCode { get; private set; }
    public string LobbyId { get; private set; }
    public System.Action<string> OnLobbyCreated;

    private async void Awake()
    {
        await InitializeAsync();
    }

    public async Task FindMatchAsync()
    {
        var lobbies = await LobbyService.Instance.QueryLobbiesAsync(new QueryLobbiesOptions
        {
            Filters = new List<QueryFilter>
            {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            }
        });

        if (lobbies.Results.Count > 0)
        {
            await JoinAsClientAsync(lobbies.Results[0].Id);
        }
        else
        {
            await StartAsHostAsync();
        }
    }

    public async Task StartAsHostAsync()
    {
        string joinCode = await relayManager.StartHostWithRelayAsync(maxConnections: 4);
        
        _currentLobby = await LobbyService.Instance.CreateLobbyAsync(
            "Partida",
            maxPlayers: 4,
            new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    { "joinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode) }
                }
            });
        //print(_currentLobby);
        _lobbyId = _currentLobby.Id;
        print($"LobbyID: {_lobbyId}");
        //NetworkManager.Singleton.StartHost();
        Debug.Log($"Host iniciado. Join code: {joinCode}");
    }

    public async Task JoinAsClientAsync(string lobbyId)
    {
        _currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
        _lobbyId = _currentLobby.Id;

        string joinCode = _currentLobby.Data["joinCode"].Value;

        await relayManager.StartClientWithRelayAsync(joinCode);

        Debug.Log($"Cliente conectado via join code: {joinCode}");
    }

    public async Task CancelSearchAsync()
    {
        if (!string.IsNullOrEmpty(_ticketId))
        {
            await MatchmakerService.Instance.DeleteTicketAsync(_ticketId);
            _lobbyId = null;
        }

        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
            NetworkManager.Singleton.Shutdown();
    }

    public void FindMatch()
    {
        FindMatchAsync().ContinueWith(t =>
        {
            if (t.IsFaulted)
                Debug.LogError($"Error en matchmaking: {t.Exception}");
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    public void StartHost()
    {
        StartAsHostAsync().ContinueWith(t =>
        {
            if (t.IsFaulted)
                Debug.LogError($"Error en matchmaking: {t.Exception}");
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    public async Task CreateLobbyAsync()
    {
        _currentLobby = await LobbyService.Instance.CreateLobbyAsync(
            "Partida",
            maxPlayers: 4,
            new CreateLobbyOptions { IsPrivate = false });

        _lobbyId = _currentLobby.Id;
        LobbyCode = _currentLobby.LobbyCode;
        LobbyId = _currentLobby.Id;

        OnLobbyCreated?.Invoke(LobbyCode);
    }

    public void CreateLobby() =>
    CreateLobbyAsync().ContinueWith(t =>
    {
        if (t.IsFaulted) Debug.LogError($"Error: {t.Exception}");
    }, TaskScheduler.FromCurrentSynchronizationContext());

    public async Task StartGameAsync()
    {
        Debug.Log("LOBBY ID EN MATCHMAKING: " + _lobbyId);

        string joinCode = await relayManager.StartHostWithRelayAsync(maxConnections: 4);

        await LobbyService.Instance.UpdateLobbyAsync(_lobbyId, new UpdateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
        {
            { "joinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode) },
            { "state", new DataObject(DataObject.VisibilityOptions.Public, "starting") }
        }
        });
    }
    public void StartGame() =>
    StartGameAsync().ContinueWith(t =>
    {
        if (t.IsFaulted) Debug.LogError($"Error: {t.Exception}");
    }, TaskScheduler.FromCurrentSynchronizationContext());


    public void CancelSearch()
    {
        CancelSearchAsync().ContinueWith(t =>
        {
            if (t.IsFaulted)
                Debug.LogError($"Error cancelando búsqueda: {t.Exception}");
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private async Task InitializeAsync()
    {
        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
                await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (AuthenticationException e) when (e.Message.Contains("already signing in"))
        {
            // Otra instancia ya está haciendo sign in, esperar hasta que termine
            while (!AuthenticationService.Instance.IsSignedIn)
                await Task.Yield();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error inicializando Unity Services: {e.Message}");
        }
    }
}