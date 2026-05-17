using UnityEngine;

public class AudioUpdater : MonoBehaviour
{
    private AudioService _audioService;

    public void Initialize(AudioService service)
    {
        _audioService = service;
    }

    private void Update()
    {
        _audioService.UpdateMusicPlaylist();
    }
}
