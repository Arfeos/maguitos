using System;
using UnityEngine;

public class HordeInitializer : MusicInitializer
{

    protected override void Start()
    {
        base.Start();
        _audioService.SetMusicVolume(0.05f);
    }
}
