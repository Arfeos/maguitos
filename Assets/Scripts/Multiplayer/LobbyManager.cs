using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LobbyManager : MonoBehaviour
{
    private Lobby _currentLobby;
    private float _heartbeatTimer;
    private bool _isHost;
    private float _pollTimer;

    // ?? CREAR LOBBY (Host) ??????????????????????????????????????????
    public async Task<Lobby> CreateLobbyAsync(string lobbyName, int maxPlayers)
    {
        var options = new CreateLobbyOptions
        {
            IsPrivate = false,
            Data = new Dictionary<string, DataObject>
            {
                { "RelayJoinCode", new DataObject(
                    DataObject.VisibilityOptions.Member, "") },
                // Estado del lobby: "waiting" o "starting"
                { "State", new DataObject(
                    DataObject.VisibilityOptions.Member, "waiting") }
            }
        };

        _currentLobby = await LobbyService.Instance.CreateLobbyAsync(
            lobbyName, maxPlayers, options);

        _isHost = true;
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

    public async Task StartGameAsync(string relayJoinCode)
    {
        if (!_isHost) return;

        await LobbyService.Instance.UpdateLobbyAsync(_currentLobby.Id,
            new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "RelayJoinCode", new DataObject(
                        DataObject.VisibilityOptions.Member, relayJoinCode) },
                    { "State", new DataObject(
                        DataObject.VisibilityOptions.Member, "starting") }
                }
            });

        Debug.Log($"[LobbyManager] Partida iniciada con code: {relayJoinCode}");
    }

    // Evento que se dispara cuando el lobby pasa a "starting"
    public System.Action<string> OnGameStarted;

    void Update()
    {
        if (_currentLobby == null) return;

        // Heartbeat solo el host
        if (_isHost)
        {
            _heartbeatTimer += Time.deltaTime;
            if (_heartbeatTimer >= 15f)
            {
                _heartbeatTimer = 0f;
                _ = LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
            }
        }

        // Polling solo el cliente para detectar cuando el host inicia
        if (!_isHost)
        {
            _pollTimer += Time.deltaTime;
            if (_pollTimer >= 2f)
            {
                _pollTimer = 0f;
                _ = PollLobbyAsync();
            }
        }
    }

    private async Task PollLobbyAsync()
    {
        _currentLobby = await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);

        var state = _currentLobby.Data["State"].Value;
        if (state == "starting")
        {
            var relayJoinCode = _currentLobby.Data["RelayJoinCode"].Value;
            Debug.Log($"[LobbyManager] Partida detectada, joinCode: {relayJoinCode}");
            OnGameStarted?.Invoke(relayJoinCode);
        }
    }

    public async Task LeaveLobbyAsync()
    {
        if (_currentLobby == null) return;

        if (_isHost)
            await LobbyService.Instance.DeleteLobbyAsync(_currentLobby.Id);
        else
            await LobbyService.Instance.RemovePlayerAsync(
                _currentLobby.Id,
                Unity.Services.Authentication.AuthenticationService.Instance.PlayerId);

        _currentLobby = null;
    }
}