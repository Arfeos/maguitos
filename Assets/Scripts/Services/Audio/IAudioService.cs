using UnityEngine;

public interface IAudioService 
{
    void PlayMusic(AudioClip clip);
    void StopMusic();
    void PlaySound(AudioClip clip);
    void DestroyAudioSources();
}
