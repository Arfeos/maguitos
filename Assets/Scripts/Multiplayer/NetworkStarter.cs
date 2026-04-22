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
        //if (NetworkManager.Singleton != null && NetworkManager.Singleton != GetComponent<NetworkManager>())
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        //DontDestroyOnLoad(NetworkManager.Singleton.gameObject);

        //NetworkManager.Singleton.OnServerStarted += () =>
        //{
        //    sceneService.LoadScene("SampleScene");
        //};
    

}
    public void Host()
    {
        //StartCoroutine(LoadSceneNextFrame());
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(
        "SampleScene",
        LoadSceneMode.Single
    );


    }
    //public override void OnNetworkSpawn()
    //{
    //    sceneService.LoadScene("SampleScene");
    //}
    public void Client()
    {
        NetworkManager.Singleton.StartClient();
    }
    //void OnGUI()
    //{
    //    if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
    //    {
    //        if (GUI.Button(new Rect(10, 10, 100, 30), "Host"))
    //            NetworkManager.Singleton.StartHost();

    //        if (GUI.Button(new Rect(10, 50, 100, 30), "Client"))
    //            NetworkManager.Singleton.StartClient();
    //    }
    //}

    IEnumerator LoadSceneNextFrame()
    {
        yield return null;

        NetworkManager.Singleton.StartHost();
        sceneService.LoadScene("SampleScene");
    }

}