using System.Collections;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Matchmaker.Models;

public class LocalGame : NetworkBehaviour
{
    private ISceneService sceneService;
    [SerializeField] private SceneNames scene;
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

        sceneService.LoadScene(scene);
    }
    public void Client()
    {
        NetworkManager.Singleton.StartClient();
    }
    IEnumerator LoadSceneNextFrame()
    {
        yield return null;

        NetworkManager.Singleton.StartHost();
        sceneService.LoadScene(scene);
    }

}