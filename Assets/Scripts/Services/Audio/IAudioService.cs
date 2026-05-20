using UnityEngine;

public interface IAudioService 
{
    void PlayMusic(AudioClip[] clip);

    void StopMusic();
    public void SetMusicVolume(float volume);
    void PlaySound(AudioClip clip);
    void StopSound(AudioClip clip);
    public void SetSFXVolume(float volume);
    void DestroyAudioSources();
    public void PlayLoopSound(AudioClip clip, float pitch = 1);
}
