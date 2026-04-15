using UnityEngine;

public interface IAudioService 
{
    void PlaySound(AudioClip clip, bool loop = false);
    void DestroyAudioSources();
}
