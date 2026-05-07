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
        Debug.Log($"[SpawnPoint] Start - IsServer: {NetworkManager.Singleton.IsServer}");
        if (!NetworkManager.Singleton.IsServer) return;

        Debug.Log("[SpawnPoint] Suscribiendo eventos");
        //NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        sceneReady = true;
        Debug.Log($"[SpawnPoint] Clientes conectados: {NetworkManager.Singleton.ConnectedClientsList.Count}");
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            Debug.Log($"[SpawnPoint] Spawneando cliente: {client.ClientId}");
            SpawnPlayer(client.ClientId);
        }


        Debug.Log("Prueba");
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
        Debug.Log($"[SpawnPoint] Spawneando {clientId} en {spawn.position}");
        GameObject player = Instantiate(playerPrefab, spawn.position, spawn.rotation);

        player.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
    }
}