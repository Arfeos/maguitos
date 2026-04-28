//using Unity.Services.Lobbies;
//using Unity.Services.Relay;
//using Unity.Netcode;
//using Unity.Netcode.Transports.UTP;
//using System.Collections.Generic;
//using UnityEngine.SceneManagement;

//public async void StartHostFlow()
//{
//    // 1. Crear Relay
//    var allocation = await RelayService.Instance.CreateAllocationAsync(4);
//    string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

//    // 2. Crear Lobby
//    var lobby = await LobbyService.Instance.CreateLobbyAsync(
//        "AutoLobby",
//        4,
//        new CreateLobbyOptions
//        {
//            IsPrivate = false,
//            Data = new Dictionary<string, DataObject>
//            {
//                {
//                    "joinCode",
//                    new DataObject(DataObject.VisibilityOptions.Public, joinCode)
//                }
//            }
//        });

//    // 3. Configurar transporte
//    var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

//    transport.SetRelayServerData(
//        allocation.RelayServer.IpV4,
//        (ushort)allocation.RelayServer.Port,
//        allocation.AllocationIdBytes,
//        allocation.Key,
//        allocation.ConnectionData
//    );

//    // 4. Start Host
//    NetworkManager.Singleton.StartHost();

//    // 5. Cambiar escena
//    NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
//}