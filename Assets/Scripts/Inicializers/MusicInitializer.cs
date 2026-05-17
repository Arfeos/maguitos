using UnityEngine;

public class MusicInitializer : MonoBehaviour
{
    [SerializeField] AudioClip[] musiclist;
    IAudioService _audioService;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioService = AppContainer.Get<IAudioService>();
        _audioService.PlayMusic(musiclist);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
