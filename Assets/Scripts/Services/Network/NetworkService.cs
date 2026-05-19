using System;
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
            var options = new InitializationOptions();

            #if UNITY_EDITOR
                options.SetProfile($"Player_{Guid.NewGuid().ToString()[..8]}");
            #endif

            await UnityServices.InitializeAsync(options);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            PlayerId = AuthenticationService.Instance.PlayerId;
            IsReady = true;
            _readyTcs.SetResult(true);
        }
        catch (System.Exception e)
        {
            _readyTcs.SetException(e);
            Debug.LogError($"[NetworkService] Error al inicializar: {e.Message}");
        }
    }

    public Task WaitUntilReadyAsync() => _readyTcs.Task;
}