using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
public class MultiplayerFlow : MonoBehaviour
{
    [SerializeField] private LobbyManager lobbyManager;
    [SerializeField] private RelayManager relayManager;
    [SerializeField] private SceneNames GameScene;
    [SerializeField] private SceneNames LobbyScene;


    private ISceneService sceneService;

    async void Start()
    {
        sceneService = AppContainer.Get<ISceneService>();
        var network = AppContainer.Get<INetworkService>();

        //AppContainer.Resolve<INetworkService>();
        await network.WaitUntilReadyAsync(); // espera si aún no terminó
    }

    public async Task HostGameAsync(string lobbyName)
    {
        var lobby = await lobbyManager.CreateLobbyAsync(lobbyName, maxPlayers: 4);
        string relayCode = await relayManager.StartHostWithRelayAsync(3);

        // Guarda el relayCode en el lobby para que los clientes lo lean
        await LobbyService.Instance.UpdateLobbyAsync(lobby.Id,
            new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "RelayJoinCode", new DataObject(
                        DataObject.VisibilityOptions.Member, relayCode) }
                }
            });

        sceneService.LoadScene(GameScene);
        //NetworkManager.Singleton.SceneManager
        //    .LoadScene("GameScene", LoadSceneMode.Single);
    }

    public async Task JoinGameAsync(string lobbyCode)
    {
        var lobby = await lobbyManager.JoinByCodeAsync(lobbyCode);
        string relayCode = lobby.Data["RelayJoinCode"].Value;
        await relayManager.StartClientWithRelayAsync(relayCode);
    }
}