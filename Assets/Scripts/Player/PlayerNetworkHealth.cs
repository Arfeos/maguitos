using Unity.Netcode;
using UnityEditor.MPE;
using UnityEngine;

public class PlayerNetworkHealth : NetworkBehaviour
{
    public NetworkVariable<int> health = new(100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private ICharacterService _characterService;
    private int maxLife = 100;

    public override void OnNetworkSpawn()
    {
        _characterService = AppContainer.Get<ICharacterService>();
        health.OnValueChanged += OnHealthChanged;
    }

    public override void OnNetworkDespawn()
    {
        health.OnValueChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int oldValue, int newValue)
    {
        Debug.Log($"[OnHealthChanged] IsOwner: {IsOwner} | IsServer: {IsServer} | {oldValue} -> {newValue}");
        if (!IsOwner) return;

        _characterService.SyncHealth(newValue);
    }

    [Rpc(SendTo.Server)]
    public void TakeDamageRpc(int damage, RpcParams rpcParams = default)
    {
        Debug.Log($"[TakeDamage] IsServer: {IsServer} | health actual: {health.Value} | damage: {damage}");
        health.Value -= damage;
    }
    [Rpc(SendTo.Server)]
    public void HealRpc(int amountHealed)
    {
        if (health.Value + amountHealed > maxLife) health.Value = maxLife;
        else health.Value += amountHealed;
    }
}