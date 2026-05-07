using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class MatchmakingManager : MonoBehaviour
{
    private string _ticketId;
    [SerializeField] private RelayManager relayManager;

    private string _lobbyId;
    private Lobby _currentLobby;


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

    private async Task StartAsHostAsync()
    {
        string joinCode = await relayManager.StartHostWithRelayAsync(maxConnections: 4);

        _currentLobby = await LobbyService.Instance.CreateLobbyAsync(
            "Partida",
            maxPlayers: 2,
            new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    { "joinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode) }
                }
            });

        _lobbyId = _currentLobby.Id;

        //NetworkManager.Singleton.StartHost();
        Debug.Log($"Host iniciado. Join code: {joinCode}");
    }

    private async Task JoinAsClientAsync(string lobbyId)
    {
        // Unirse al lobby
        _currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
        _lobbyId = _currentLobby.Id;

        // Obtener el join code guardado en el lobby
        string joinCode = _currentLobby.Data["joinCode"].Value;

        // Unirse a la asignación Relay con el join code
        await relayManager.StartClientWithRelayAsync(joinCode);

        Debug.Log($"Cliente conectado via join code: {joinCode}");
    }

    public async Task CancelSearchAsync()
    {
        if (!string.IsNullOrEmpty(_ticketId)) {
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

    public void CancelSearch()
    {
        CancelSearchAsync().ContinueWith(t =>
        {
            if (t.IsFaulted)
                Debug.LogError($"Error cancelando búsqueda: {t.Exception}");
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

}