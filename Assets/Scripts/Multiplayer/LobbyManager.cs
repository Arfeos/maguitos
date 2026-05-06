using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LobbyManager : MonoBehaviour
{
    private Lobby _currentLobby;
    private float _heartbeatTimer;

    // ?? CREAR LOBBY (Host) ??????????????????????????????????????????
    public async Task<Lobby> CreateLobbyAsync(string lobbyName, int maxPlayers)
    {
        var options = new CreateLobbyOptions
        {
            IsPrivate = false,
            Data = new Dictionary<string, DataObject>
            {
                // El JoinCode de Relay se guarda aquí para que otros lo lean
                { "RelayJoinCode", new DataObject(
                    visibility: DataObject.VisibilityOptions.Member,
                    value: "") }
            }
        };

        _currentLobby = await LobbyService.Instance.CreateLobbyAsync(
            lobbyName, maxPlayers, options);

        Debug.Log($"Lobby creado: {_currentLobby.LobbyCode}");
        return _currentLobby;
    }

    // ?? BUSCAR LOBBIES PÚBLICOS ?????????????????????????????????????
    public async Task<List<Lobby>> QueryLobbiesAsync()
    {
        var response = await LobbyService.Instance.QueryLobbiesAsync();
        return response.Results;
    }

    // ?? UNIRSE POR CÓDIGO ???????????????????????????????????????????
    public async Task<Lobby> JoinByCodeAsync(string lobbyCode)
    {
        _currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
        return _currentLobby;
    }

    // ?? HEARTBEAT (evita que el lobby expire) ??????????????????????
    void Update()
    {
        if (_currentLobby == null) return;
        _heartbeatTimer += Time.deltaTime;
        if (_heartbeatTimer >= 15f)
        {
            _heartbeatTimer = 0f;
            _ = LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
        }
    }
}