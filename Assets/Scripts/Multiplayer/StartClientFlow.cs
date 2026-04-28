//using Unity.Services.Lobbies;
//using Unity.Services.Relay;
//using Unity.Netcode;
//using Unity.Netcode.Transports.UTP;

//public async void StartClientFlow(MatchmakingTicket ticket)
//{
//    // 1. Unirse al Lobby (ejemplo simplificado)
//    var lobby = await LobbyService.Instance.JoinLobbyByIdAsync(ticket.LobbyId);

//    // 2. Leer joinCode
//    string joinCode = lobby.Data["joinCode"].Value;

//    // 3. Conectarse a Relay
//    var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

//    var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

//    transport.SetRelayServerData(
//        allocation.RelayServer.IpV4,
//        (ushort)allocation.RelayServer.Port,
//        allocation.AllocationIdBytes,
//        allocation.Key,
//        allocation.ConnectionData,
//        allocation.HostConnectionData
//    );

//    // 4. Start Client
//    NetworkManager.Singleton.StartClient();
//}