using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;
using System.Net;
using System.Linq;

public class RelayManager : MonoBehaviour
{
    private ISceneService sceneService;

    [SerializeField] private SceneNames scene;
    [SerializeField] private SceneNames scene1;
    [SerializeField] private SceneNames scene2;

    private void Awake()
    {
        sceneService = AppContainer.Get<ISceneService>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async Task<string> StartHostWithRelayAsync(int maxConnections = 4)
    {
        NetworkManager.Singleton.NetworkConfig.PlayerPrefab = null;
        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
        NetworkManager.Singleton.ConnectionApprovalCallback = (request, response) =>
        {
            response.CreatePlayerObject = false;
            response.Approved = true;
            response.Pending = false;
        };
        Allocation allocation = await RelayService.Instance
            .CreateAllocationAsync(maxConnections);

        string joinCode = await RelayService.Instance
            .GetJoinCodeAsync(allocation.AllocationId);

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        var endpoint = allocation.ServerEndpoints
            .First(e => e.Secure && e.Network == RelayServerEndpoint.NetworkOptions.Udp);

        transport.SetRelayServerData(
            endpoint.Host,
            (ushort)endpoint.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData,
            allocation.ConnectionData,
            true                       
        );

        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
        NetworkManager.Singleton.ConnectionApprovalCallback = (request, response) =>
        {
            response.CreatePlayerObject = false;
            response.Approved = true;
            response.Pending = false;
        };
        NetworkManager.Singleton.StartHost();
        sceneService.LoadScene(scene);
        return joinCode;
    }
    public async Task StartClientWithRelayAsync(string joinCode)
    {
        NetworkManager.Singleton.NetworkConfig.PlayerPrefab = null;
        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;

        if (NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
            // Espera un frame para que el shutdown complete
            await Task.Yield();
        }

        JoinAllocation join = await RelayService.Instance.JoinAllocationAsync(joinCode);
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        var endpoint = join.ServerEndpoints
            .First(e => e.Secure && e.Network == RelayServerEndpoint.NetworkOptions.Udp);

        transport.SetRelayServerData(
            endpoint.Host,
            (ushort)endpoint.Port,
            join.AllocationIdBytes,
            join.Key,
            join.ConnectionData,
            join.HostConnectionData,
            true
        );
        
        NetworkManager.Singleton.StartClient();
    }
}
