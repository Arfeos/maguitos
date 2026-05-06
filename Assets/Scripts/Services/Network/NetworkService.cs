using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class NetworkService : INetworkService
{
    public bool IsReady { get; private set; }
    public string PlayerId { get; private set; }

    private TaskCompletionSource<bool> _readyTcs = new TaskCompletionSource<bool>();

    public async Task InitializeAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            PlayerId = AuthenticationService.Instance.PlayerId;
            IsReady = true;
            _readyTcs.SetResult(true);
            Debug.Log($"[NetworkService] Listo. PlayerId: {PlayerId}");
        }
        catch (System.Exception e)
        {
            _readyTcs.SetException(e);
            Debug.LogError($"[NetworkService] Error al inicializar: {e.Message}");
        }
    }

    public Task WaitUntilReadyAsync() => _readyTcs.Task;
}