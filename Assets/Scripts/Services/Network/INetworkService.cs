using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public interface INetworkService
{
    bool IsReady { get; }
    string PlayerId { get; }
    Task WaitUntilReadyAsync();
}
