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
    private AudioClip[] _musicPlaylist;
    private int _currentTrackIndex = 0;
    public AudioService()
    {
        _audioRoot = new GameObject("AudioService");

        Object.DontDestroyOnLoad(_audioRoot);
        AudioUpdater updater = _audioRoot.AddComponent<AudioUpdater>();
        updater.Initialize(this);
        CreateMusicSource();
    }
    //Musica
    private void CreateMusicSource()
    {
        _musicSource = _audioRoot.AddComponent<AudioSource>();
        Object.DontDestroyOnLoad(_musicSource);
        _musicSource.loop = false;
        _musicSource.volume = _musicVolume;
    }


    public void PlayMusic(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
            return;
        _musicPlaylist = clips;
        _currentTrackIndex = 0;
        PlayCurrentTrack();
    }
    private void PlayCurrentTrack()
    {
        if (_musicPlaylist == null || _musicPlaylist.Length == 0)
            return;
        _musicSource.Stop();
        _musicSource.clip = _musicPlaylist[_currentTrackIndex];
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

    public void UpdateMusicPlaylist()
    {
        if (_musicPlaylist == null || _musicPlaylist.Length == 0)
            return;

        if (_musicSource.isPlaying)
            return;

        _currentTrackIndex++;

        if (_currentTrackIndex >= _musicPlaylist.Length)
        {
            _currentTrackIndex = 0;
        }

        PlayCurrentTrack();
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