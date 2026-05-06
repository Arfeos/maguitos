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
    void Start()
    {
        sceneService = AppContainer.Get<ISceneService>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async Task<string> StartHostWithRelayAsync(int maxConnections = 4)
    {
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
        NetworkManager.Singleton.StartHost();
        return joinCode;
    }
    public async Task StartClientWithRelayAsync(string joinCode)
    {
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

        sceneService.LoadScene("");
    }
}
