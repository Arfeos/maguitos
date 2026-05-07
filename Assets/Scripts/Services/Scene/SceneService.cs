using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneService : ISceneService
{
    private static Stack<string> sceneHistory = new Stack<string>();
    private HashSet<ulong> readyPlayers = new HashSet<ulong>();
    public SceneService()
    {
        SceneManager.activeSceneChanged += SaveScene;
        Application.quitting += Cleanup;
    }

    private void Cleanup()
    {
        SceneManager.activeSceneChanged -= SaveScene;
        Application.quitting -= Cleanup;
    }



    #region Scene charger (InGame & Lobby)

    public void LoadScene(string sceneName)
    {
        if (!NetworkManager.Singleton.IsListening)
        {
            // offline
            SceneManager.LoadScene(sceneName);
            return;
        }

        if (NetworkManager.Singleton.IsServer)
        {
            //online
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            return;
        }
    }


    [ServerRpc]
    private void RequestLoadSceneServerRpc(string sceneName, ServerRpcParams rpcParams = default)
    {
        ulong senderId = rpcParams.Receive.SenderClientId;

        if (!CanRequestSceneChange(senderId))
            return;

        LoadSceneInternal(sceneName);
    }
    private bool CanRequestSceneChange(ulong clientId)
    {
        return clientId == NetworkManager.ServerClientId;
        //return readyPlayers.Count == NetworkManager.ConnectedClients.Count;
    }

    private void LoadSceneInternal(string sceneName)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    #endregion



    public void GoBack()
    {
        if (sceneHistory.Count > 0)
        {
            SceneManager.LoadScene(sceneHistory.Pop());
        }
    }
    public void SaveScene(Scene oldScene, Scene newScene)
    {
        sceneHistory.Push(oldScene.name);
    }
}