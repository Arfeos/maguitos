using UnityEngine;

/// <summary>
/// Servicio encargado de gestionar la reproducción de música y efectos de sonido del juego mediante el componente <see cref="AudioUpdater"/>
/// </summary>
public interface IAudioService
{

    /// <summary>
    /// Inicia la reproducción de una lista de pistas musicales y establece la primera canción como reproducción actual
    /// </summary>
    /// <param name="clips">Lista de clips de audio que formarán la lista de reproducción</param>
    void PlayMusic(AudioClip[] clip);

    /// <summary>
    /// Detiene la reproducción de la música actual
    /// </summary>
    void StopMusic();

    /// <summary>
    /// Modifica el volumen de la música de fondo
    /// </summary>
    /// <param name="volume">Valor del volumen que se aplicará a la música</param>
    void SetMusicVolume(float volume);

    /// <summary>
    /// Reproduce un efecto de sonido una sola vez. Incluye una restricción temporal para evitar reproducir repetidamente el mismo sonido en intervalos muy cortos
    /// </summary>
    /// <param name="clip">Clip de audio que se reproducirá como efecto de sonido</param>
    void PlaySound(AudioClip clip);

    /// <summary>
    /// Detiene la reproducción de un efecto de sonido específico
    /// </summary>
    /// <param name="clip">Clip de audio que dejará de reproducirse</param>
    void StopSound(AudioClip clip);

    /// <summary>
    /// Modifica el volumen de todos los efectos de sonido activos
    /// </summary>
    /// <param name="volume">Valor del volumen que se aplicará a los efectos de sonido</param>
    void SetSFXVolume(float volume);

    /// <summary>
    /// Destruye todas las fuentes de audio creadas y limpia las listas internas asociadas
    /// </summary>
    void DestroyAudioSources();

    /// <summary>
    /// Reproduce un efecto de sonido en bucle continuo y permite modificar su velocidad o tono
    /// </summary>
    /// <param name="clip">Clip de audio que se reproducirá en bucle</param>
    /// <param name="pitch">Valor que determina el tono o velocidad del sonido. Valor por defecto: 1f</param>
    void PlayLoopSound(AudioClip clip, float pitch = 1);
}