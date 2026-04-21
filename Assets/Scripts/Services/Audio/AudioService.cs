using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class AudioService : IAudioService
{
    private readonly GameObject _audioRoot;
    private List<AudioSource> _audioSources = new List<AudioSource>();
    private float _minInterval = 0.01f;
    private Dictionary<AudioClip, float> _lastPlayTime = new();
    public AudioService()
    {
        _audioRoot = new GameObject("AudioService");
        Object.DontDestroyOnLoad(_audioRoot);
    }

    public void PlaySound(AudioClip clip, bool loop = false, bool stopPrevious = false)
    {
        if (clip == null) return;
        AudioSource existingSource = _audioSources
            .FirstOrDefault(x => x.isPlaying && x.clip == clip);

        if (existingSource != null)
        {
           
            if (_lastPlayTime.TryGetValue(clip, out float lastTime))
            {
                if (Time.time - lastTime < _minInterval)
                    return; 
            }

            _lastPlayTime[clip] = Time.time;

            if (stopPrevious)
            {
                existingSource.Stop();
                existingSource.loop = loop;
                existingSource.Play();
            }

            return;
        }
        var audioSource = GetOrCreateAudioSource();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    private AudioSource GetOrCreateAudioSource()
    {
        AudioSource audioSource = _audioSources
            .FirstOrDefault(x => !x.isPlaying);

        if (audioSource == null)
        {
            audioSource = _audioRoot.AddComponent<AudioSource>();
            _audioSources.Add(audioSource);
        }

        return audioSource;
    }

    public void DestroyAudioSources()
    {
        foreach (var audioSource in _audioSources)
        {
            Object.Destroy(audioSource);
        }
        _audioSources.Clear();
    }
}
