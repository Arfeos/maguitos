using Unity.Netcode;
using UnityEngine;

public class NetworkObjectDebugger : MonoBehaviour
{
    private float _timer = 0f;

    private void Update()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer) return;

        _timer += Time.deltaTime;
        if (_timer < 3f) return;
        _timer = 0f;

        var allNetworkObjects = FindObjectsByType<NetworkObject>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        Debug.Log($"[Debug] Total NetworkObjects: {allNetworkObjects.Length}");
        foreach (var no in allNetworkObjects)
        {
            Debug.Log($"[Debug] GO: {GetPath(no.transform)} | " +
                      $"Active: {no.gameObject.activeSelf} | " +
                      $"Spawned: {no.IsSpawned} | " +
                      $"NetworkManager null: {no.NetworkManager == null}");
        }
    }

    private string GetPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null) { t = t.parent; path = t.name + "/" + path; }
        return path;
    }
}