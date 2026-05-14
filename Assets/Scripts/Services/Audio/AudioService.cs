using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioService : IAudioService
{
    private readonly GameObject _audioRoot;

    private AudioSource _musicSource;

    private List<AudioSource> _sfxSources = new();

    private float _minInterval = 0.01f;

    private Dictionary<AudioClip, float> _lastPlayTime = new();
    private float _musicVolume = 1f;
    private float _sfxVolume = 1f;
    public AudioService()
    {
        _audioRoot = new GameObject("AudioService");

        Object.DontDestroyOnLoad(_audioRoot);

        CreateMusicSource();
    }
    //Musica
    private void CreateMusicSource()
    {
        _musicSource = _audioRoot.AddComponent<AudioSource>();
        Object.DontDestroyOnLoad(_musicSource);
        _musicSource.loop = true;
        _musicSource.volume = _musicVolume;
    }


    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
            return;

        _musicSource.Stop();

        _musicSource.clip = clip;

        _musicSource.Play();
    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }
    public void SetMusicVolume(float volume)
    {
        _musicVolume = volume;

        _musicSource.volume = volume;
    }



    //efectos de sonido
    public void PlaySound(AudioClip clip)
    {
        if (clip == null)
            return;

        if (_lastPlayTime.TryGetValue(clip, out float lastTime))
        {
            if (Time.time - lastTime < _minInterval)
                return;
        }

        _lastPlayTime[clip] = Time.time;

        AudioSource source = GetOrCreateSFXSource();

        source.clip = clip;

        source.loop = false;

        source.Play();
    }
    public void PlayLoopSound(AudioClip clip, float pitch = 1f)
    {
        if (clip == null)
            return;

        foreach (var sound in _sfxSources)
        {
            if (sound.clip == clip && sound.isPlaying)
            {
                sound.pitch = pitch;
                return;
            }
        }

        AudioSource source = GetOrCreateSFXSource();

        source.clip = clip;
        source.pitch = pitch;
        source.loop = true;

        source.Play();
    }

    private AudioSource GetOrCreateSFXSource()
    {
        AudioSource source = _sfxSources
            .FirstOrDefault(x => !x.isPlaying);

        if (source == null)
        {
            source = _audioRoot.AddComponent<AudioSource>();
            source.volume = _sfxVolume;
            _sfxSources.Add(source);
        }

        return source;
    }
    public void SetSFXVolume(float volume)
    {
        _sfxVolume = volume;

        foreach (var source in _sfxSources)
        {
            source.volume = volume;
        }
    }

    public void DestroyAudioSources()
    {
        foreach (var source in _sfxSources)
        {
            Object.Destroy(source);
        }

        _sfxSources.Clear();

        Object.Destroy(_musicSource);
    }

    public void StopSound(AudioClip clip)
    {
        if (clip == null)
            return;

        foreach (var source in _sfxSources)
        {
            if (source.clip == clip && source.isPlaying)
            {
                source.Stop();
            }
        }
    }
}