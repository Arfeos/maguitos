using UnityEngine;
/// <summary>
/// Componente encargado de actualizar continuamente la reproducción de música mediante <see cref="AudioService"/>
/// </summary>
public class AudioUpdater : MonoBehaviour
{
    private AudioService _audioService;
    /// <summary>
    /// Inicializa el componente asignándole una referencia a <see cref="AudioService"/>.
    /// </summary>
    /// <param name="service">Servicio de audio que será gestionado</param>
    public void Initialize(AudioService service)
    {
        _audioService = service;
    }
    /// <summary>
    /// Método ejecutado automáticamente en cada frame que llama a UpdateMusicPlaylist() de <see cref="AudioService"/> para controlar el cambio automático entre canciones.
    /// </summary>
    private void Update()
    {
        _audioService.UpdateMusicPlaylist();
    }
}
