using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Relay;
using UnityEngine;

public class MatchmakingManager : MonoBehaviour
{
    private string _ticketId;
    [SerializeField] private RelayManager relayManager;

    public async Task FindMatchAsync()
    {
        // Crear ticket de búsqueda
        var ticketResponse = await MatchmakerService.Instance
            .CreateTicketAsync(new List<Player>
            {
                new Player(AuthenticationService.Instance.PlayerId)
            },
            new CreateTicketOptions { QueueName = "default-queue" });

        _ticketId = ticketResponse.Id;

        await PollForMatchAsync(_ticketId);
    }

    private async Task PollForMatchAsync(string ticketId)
    {
        MultiplayAssignment assignment = null;

        while (assignment?.Status != MultiplayAssignment.StatusOptions.Found)
        {
            await Task.Delay(2000); // espera 2 segundos entre consultas

            var ticketStatus = await MatchmakerService.Instance
                .GetTicketAsync(ticketId);





            // Cambio de funcionamiento de Servidor a host con Relay (comprobar unión de Lobby y Relay para un correcto funcionamiento matchmaking)

        //    var results = ticketStatus.Value as MatchmakingResults;

        //    bool isHost = results.Players[0].Id ==
        //                  AuthenticationService.Instance.PlayerId;

        //    if (isHost)
        //    {
        //        // HOST: crea relay
        //        string joinCode = await relayManager.StartHostWithRelayAsync(4);

        //        // IMPORTANTE: compartir este código
        //        PlayerPrefs.SetString("JoinCode", joinCode);
        //    }
        //    else
        //    {
        //        // CLIENTE: espera joinCode (simplificado)
        //        await Task.Delay(2000);

        //        string joinCode = PlayerPrefs.GetString("JoinCode");

        //        var joinAllocation = await RelayService.Instance
        //            .JoinAllocationAsync(joinCode);

        //        var transport = NetworkManager.Singleton
        //            .GetComponent<UnityTransport>();

        //        transport.SetRelayServerData(
        //            new RelayServerData(joinAllocation, "dtls")
        //        );

        //        NetworkManager.Singleton.StartClient();
        //    }

        //    return;
        //}


        if (ticketStatus.Type == typeof(MultiplayAssignment))
                assignment = ticketStatus.Value as MultiplayAssignment;

            if (assignment?.Status == MultiplayAssignment.StatusOptions.Failed)
            {
                Debug.LogError("Matchmaking fallido");
                return;
            }
        }

        Debug.Log($"Partida encontrada: {assignment.Ip}:{assignment.Port}");
        // Conectar con los datos de la asignación...

    }

    public async Task CancelSearchAsync()
    {
        if (!string.IsNullOrEmpty(_ticketId))
            await MatchmakerService.Instance.DeleteTicketAsync(_ticketId);
    }
}