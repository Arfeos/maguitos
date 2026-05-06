using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class MatchmakingManager : MonoBehaviour
{
    private string _ticketId;

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

        // Polling hasta encontrar partida
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