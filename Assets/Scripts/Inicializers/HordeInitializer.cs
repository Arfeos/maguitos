using System;
using UnityEngine;

public class HordeInitializer : MusicInitializer
{
    protected override void Awake()
    {
        base.Awake();
        
    }
    protected  void Start()
    {
        
        _audioService.SetMusicVolume(0.05f);
    }
}
