using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Servicio encargado de gestionar la reproducción de música y efectos de sonido del juego mediante el componente <see cref="AudioUpdater"/>
/// </summary>
public class AudioService : IAudioService
{
    private readonly GameObject _audioRoot;

    private AudioSource _musicSource;

    private List<AudioSource> _sfxSources = new();

    private float _minInterval = 0.3f;

    private Dictionary<AudioClip, float> _lastPlayTime = new();
    private float _musicVolume = 1f;
    private float _sfxVolume = 1f;
    private AudioClip[] _musicPlaylist;
    private int _currentTrackIndex = 0;
    /// <summary>
    /// Constructor encargado de crear el objeto principal de audio y asociarlo con <see cref="AudioUpdater"/> para controlar la actualización automática de la música
    /// </summary>
    public AudioService()
    {
        _audioRoot = new GameObject("AudioService");

        Object.DontDestroyOnLoad(_audioRoot);
        AudioUpdater updater = _audioRoot.AddComponent<AudioUpdater>();
        updater.Initialize(this);
        CreateMusicSource();
    }
    //Musica
    /// <summary>
    /// Crea e inicializa la fuente de audio utilizada para reproducir música de fondo
    /// </summary>
    private void CreateMusicSource()
    {
        _musicSource = _audioRoot.AddComponent<AudioSource>();
        Object.DontDestroyOnLoad(_musicSource);
        _musicSource.loop = false;
        _musicSource.volume = _musicVolume;
    }

    /// <summary>
    /// Inicia la reproducción de una lista de pistas musicales y establece la primera canción como reproducción actual
    /// </summary>
    /// <param name="clips">Lista de clips de audio que formarán la lista de reproducción</param>
    public void PlayMusic(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
            return;
        _musicPlaylist = clips;
        _currentTrackIndex = 0;
        PlayCurrentTrack();
    }
    /// <summary>
    /// Reproduce la pista actual de la lista de reproducción cargada
    /// </summary>
    private void PlayCurrentTrack()
    {
        if (_musicPlaylist == null || _musicPlaylist.Length == 0)
            return;
        _musicSource.Stop();
        _musicSource.clip = _musicPlaylist[_currentTrackIndex];
        _musicSource.Play();
    }
    /// <summary>
    /// Detiene la reproducción de la música actual
    /// </summary>
    public void StopMusic()
    {
        _musicSource.Stop();
    }
    /// <summary>
    /// Modifica el volumen de la música de fondo
    /// </summary>
    /// <param name="volume">Valor del volumen que se aplicará a la música</param>
    public void SetMusicVolume(float volume)
    {
        _musicVolume = volume;

        _musicSource.volume = volume;
    }
    /// <summary>
    /// Comprueba si la canción actual ha finalizado y, en caso necesario, avanza automáticamente a la siguiente pista de la lista
    /// </summary>
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
    /// <summary>
    /// Reproduce un efecto de sonido una sola vez. Incluye una restricción temporal para evitar reproducir repetidamente el mismo sonido en intervalos muy cortos
    /// </summary>
    /// <param name="clip">Clip de audio que se reproducirá como efecto de sonido</param>
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
    /// <summary>
    /// Reproduce un efecto de sonido en bucle continuo y permite modificar su velocidad o tono
    /// </summary>
    /// <param name="clip">Clip de audio que se reproducirá en bucle</param>
    /// <param name="pitch">Valor que determina el tono o velocidad del sonido. Valor por defecto: 1f</param>
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
    /// <summary>
    /// Busca una fuente de audio disponible para efectos de sonido o crea una nueva si no existe ninguna libre
    /// </summary>
    /// <returns>Fuente de audio disponible para reproducir efectos.</returns>
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
    /// <summary>
    /// Modifica el volumen de todos los efectos de sonido activos
    /// </summary>
    /// <param name="volume">Valor del volumen que se aplicará a los efectos de sonido</param>
    public void SetSFXVolume(float volume)
    {
        _sfxVolume = volume;

        foreach (var source in _sfxSources)
        {
            source.volume = volume;
        }
    }
    /// <summary>
    /// Destruye todas las fuentes de audio creadas y limpia las listas internas asociadas
    /// </summary>
    public void DestroyAudioSources()
    {
        foreach (var source in _sfxSources)
        {
            Object.Destroy(source);
        }

        _sfxSources.Clear();

        Object.Destroy(_musicSource);
    }
    /// <summary>
    /// Detiene la reproducción de un efecto de sonido específico
    /// </summary>
    /// <param name="clip">Clip de audio que dejará de reproducirse</param>
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