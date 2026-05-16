using UnityEngine;

public interface IAudioService 
{
    void PlayMusic(AudioClip clip);
    void StopMusic();
    void PlaySound(AudioClip clip);
    void StopSound(AudioClip clip);
    void DestroyAudioSources();
    public void PlayLoopSound(AudioClip clip, float pitch = 1);
}
