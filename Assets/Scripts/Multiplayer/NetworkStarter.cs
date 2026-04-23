using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkStarter : NetworkBehaviour
{
    private ISceneService sceneService;
    private void Awake()
    {
        sceneService = AppContainer.Get<ISceneService>();
    }
    public void Host()
    {
        //StartCoroutine(LoadSceneNextFrame());
        NetworkManager.Singleton.StartHost();
        
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