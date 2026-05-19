using System;
using UnityEngine;

public class HordeInitializer : MonoBehaviour
{
    private IAudioService _audioService;

    [SerializeField] private AudioClip _audioClip;
    private void Awake()
    {
        _audioService = AppContainer.Get<IAudioService>();
    }

    private void Start()
    {
        
        
        _audioService.PlayMusic(_audioClip);
        _audioService.SetMusicVolume(0.05f);
    }
}
