using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPoint : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoint;
    private int index = 0;
    public override void OnNetworkSpawn()
    {

        if (!IsServer) return;

        var sceneManager = NetworkManager.Singleton.SceneManager;

        sceneManager.OnLoadEventCompleted += OnSceneLoaded;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnSceneLoaded(string sceneName, LoadSceneMode mode,
    List<ulong> clientsCompleted,
    List<ulong> clientsTimedOut)
    {
        if (sceneName != "SampleScene") return;

        SpawnPlayers();
    }

    private void SpawnPlayer(ulong clientId)
    {
        if (!IsServer) return;

        Transform spawn = spawnPoint[index % spawnPoint.Length];
        index++;

        GameObject player = Instantiate(playerPrefab, spawn.position, spawn.rotation);

        player.GetComponent<NetworkObject>()
              .SpawnWithOwnership(clientId);
    }

    private void SpawnPlayers()
    {
        var clients = NetworkManager.Singleton.ConnectedClientsList;

        for (int i = 0; i < clients.Count; i++)
        {
            ulong clientId = clients[i].ClientId;
            index++;
            Transform spawn = spawnPoint[i % spawnPoint.Length];

            GameObject player = Instantiate(playerPrefab, spawn.position, spawn.rotation);

            player.GetComponent<NetworkObject>()
                  .SpawnWithOwnership(clientId);
        }
    }
    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;

        SpawnPlayer(clientId);
    }
}