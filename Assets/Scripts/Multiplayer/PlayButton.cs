using Unity.Services.Matchmaker;
using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class PlayButton : MonoBehaviour
{
    public async void Play()
    {
        //var ticket = await MatchmakerService.Instance.CreateTicketAsync(
        //    new CreateTicketOptions { QueueName = "default" }
        //);

        //StartCoroutine(PollTicket(ticket.Id));
    }

    private IEnumerator PollTicket(string ticketId)
    {
        while (true)
        {
            //var ticket = await MatchmakerService.Instance.GetTicketAsync(ticketId);

            //if (ticket.Status == "Matched")
            //{
            //    Debug.Log("MATCH ENCONTRADO");
            //    //HandleMatch(ticket);
            //    yield break;
            //}

            yield return new WaitForSeconds(1);
        }
    }

    //void HandleMatch(MatchmakingTicket ticket)
    //{
    //    bool isHost = ticket.Players[0].Id == AuthenticationService.Instance.PlayerId;

    //    if (isHost)
    //        StartHostFlow();
    //    else
    //        StartClientFlow(ticket);
    //}

}