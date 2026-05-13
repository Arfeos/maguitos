using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPoint : MonoBehaviour
{
    //Este es el prefab
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoint;
    private int index = 0;
    private bool sceneReady = false;
    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        //NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        sceneReady = true;
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            SpawnPlayer(client.ClientId);
        }
    }


    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (sceneReady)
            SpawnPlayer(clientId);
    }
    
    private void SpawnPlayer(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        Transform spawn = spawnPoint[index % spawnPoint.Length];
        index++;
        GameObject player = Instantiate(playerPrefab, spawn.position, spawn.rotation);

        player.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
    }
}