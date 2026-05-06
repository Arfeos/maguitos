using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using System.Collections;
using Unity.Netcode;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
//using UnityEditor.VersionControl;
using System.Runtime.CompilerServices;
using Unity.Services.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using Unity.Networking.Transport.Relay;


public class PlayButton : MonoBehaviour
{
    private bool isHost;

    private async void Awake()
    {
        //sceneService = AppContainer.Get<ISceneService>();

        await UnityServices.InitializeAsync();

    }

    public async Task Play()
    {

        //var ownPlayer = new List<Player>
        //{
        //    new Player(
        //    id: AuthenticationService.Instance.PlayerId,
        //    customData: new Dictionary<string, object>
        //    {
        //        { "region", "EU" }
        //    }
        //)};

        //var ticket = await MatchmakerService.Instance.CreateTicketAsync(
        //    ownPlayer,
        //    new CreateTicketOptions { QueueName = "default" }
        //);

        //StartCoroutine(PollTicket(ticket.Id));
    }

    private IEnumerator PollTicket(string ticketId)
    {
        while (true)
        {
            var ticketTask = MatchmakerService.Instance.GetTicketAsync(ticketId);

            yield return new WaitUntil(() => ticketTask.IsCompleted);

            if (ticketTask.Exception != null)
            {
                Debug.LogError(ticketTask.Exception);
                yield break;
            }

            var ticket = ticketTask.Result;
            Debug.Log(ticket.GetType());
            if (ticket.Type.ToString() == "Matched")
            {
                Debug.Log("MATCH ENCONTRADO");
                HandleMatch(ticket);
                yield break;
            }

            yield return new WaitForSeconds(1);
        }
    }

    void HandleMatch(TicketStatusResponse ticket)
    {
        //bool isHost = ticket.Players[0].Id == AuthenticationService.Instance.PlayerId;

        if (isHost)
            StartHostFlow();
        else
            StartClientFlow(ticket);
    }


    public async void StartHostFlow()
    {
        // 1. Crear Relay
        var allocation = await RelayService.Instance.CreateAllocationAsync(4);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        // 2. Crear Lobby
        var lobby = await LobbyService.Instance.CreateLobbyAsync(
            "AutoLobby",
            4,
            new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    {
                        "joinCode",
                        new DataObject(DataObject.VisibilityOptions.Public, joinCode)
                    }
                }
            });

        // 3. Configurar transporte
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        transport.SetRelayServerData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData
        );

        // 4. Start Host
        NetworkManager.Singleton.StartHost();

        // 5. Cambiar escena
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public async void StartClientFlow(TicketStatusResponse ticket)
    {
        // 1. Unirse al Lobby (ejemplo simplificado)
        //var lobby = await LobbyService.Instance.JoinLobbyByIdAsync(ticket.LobbyId);

        //// 2. Leer joinCode
        //string joinCode = lobby.Data["joinCode"].Value;

        //// 3. Conectarse a Relay
        //var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        //var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        //transport.SetRelayServerData(
        //    allocation.RelayServer.IpV4,
        //    (ushort)allocation.RelayServer.Port,
        //    allocation.AllocationIdBytes,
        //    allocation.Key,
        //    allocation.ConnectionData,
        //    allocation.HostConnectionData
        //);

        string joinCode = PlayerPrefs.GetString("JoinCode");

        // 4. Start Client
        NetworkManager.Singleton.StartClient();

        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        //var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        //transport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls", RelayServerData.ConnectionType.UDP));

        

        NetworkManager.Singleton.StartClient();

    }









    public async Task<List<Lobby>> QueryLobbiesAsync()
    {
        var response = await LobbyService.Instance.QueryLobbiesAsync();
        return response.Results;
    }

}