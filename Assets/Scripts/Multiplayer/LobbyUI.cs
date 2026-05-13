using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private MatchmakingManager matchmakingManager;
    [SerializeField] private TMPro.TextMeshProUGUI lobbyCodeText;
    [SerializeField] private TMPro.TextMeshProUGUI playerListText;

    private string _lobbyId;
    private float _pollTimer;

    private void OnEnable()
    {
        matchmakingManager.OnLobbyCreated += OnLobbyCreated;
    }

    private void OnDisable()
    {
        matchmakingManager.OnLobbyCreated -= OnLobbyCreated;
    }

    private void OnLobbyCreated(string code)
    {
        lobbyCodeText.text = $"Código: {code}";
        _lobbyId = matchmakingManager.LobbyId; // expón el Id igual que LobbyCode
    }

    private void Update()
    {
        if (string.IsNullOrEmpty(_lobbyId)) return;

        _pollTimer += Time.deltaTime;
        if (_pollTimer >= 2f)
        {
            _pollTimer = 0f;
            _ = RefreshPlayerListAsync();
        }
    }

    private async System.Threading.Tasks.Task RefreshPlayerListAsync()
    {
        var lobby = await Unity.Services.Lobbies.LobbyService.Instance
            .GetLobbyAsync(_lobbyId);

        var names = new System.Text.StringBuilder();
        foreach (var player in lobby.Players)
            names.AppendLine($"• {player.Id}");

        playerListText.text = names.ToString();
    }
}