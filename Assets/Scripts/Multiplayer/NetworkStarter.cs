using System.Collections;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Matchmaker.Models;

public class NetworkStarter : NetworkBehaviour
{
    private ISceneService sceneService;
    private async void Awake()
    {
        sceneService = AppContainer.Get<ISceneService>();
        
        await UnityServices.InitializeAsync();
        Debug.Log("UGS inicializado correctamente");
        
    }
    public void Host()
    {
        //StartCoroutine(LoadSceneNextFrame());
        //NetworkManager.Singleton.StartHost();

        var ticket = MatchmakerService.Instance.GetTicketAsync("10100100");
        Debug.Log(ticket.GetType());

        sceneService.LoadScene("SampleScene");
    }
    public void Client()
    {
        NetworkManager.Singleton.StartClient();
    }
    IEnumerator LoadSceneNextFrame()
    {
        yield return null;

        NetworkManager.Singleton.StartHost();
        sceneService.LoadScene("SampleScene");
    }

}