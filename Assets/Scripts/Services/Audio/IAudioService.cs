using UnityEngine;

public interface IAudioService 
{
    void PlaySound(AudioClip clip, bool loop = false, bool stopPrevious = false);
    void DestroyAudioSources();
}
