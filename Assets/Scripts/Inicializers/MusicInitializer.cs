using UnityEngine;

public class MusicInitializer : MonoBehaviour
{
    [SerializeField] protected AudioClip[] musiclist;
    protected IAudioService _audioService;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Awake()
    {
        _audioService = AppContainer.Get<IAudioService>();
        _audioService.PlayMusic(musiclist);
    }

   
}
