using UnityEngine;
/// <summary>
/// Componente de Unity encargado de inicializar automáticamente la reproducción de música al cargar una escena. Utiliza el servicio <see cref="IAudioService"/> obtenido desde <see cref="AppContainer"/> para gestionar la reproducción de una lista de pistas musicales
/// </summary>
public class MusicInitializer : MonoBehaviour
{
    /// <summary>
    /// Variable serializada que almacena la lista de pistas musicales que serán reproducidas por <see cref="IAudioService"/>.
    /// </summary>
    [SerializeField] protected AudioClip[] musiclist;
    protected IAudioService _audioService;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /// <summary>
    /// Método ejecutado durante la inicialización del objeto. Obtiene una referencia al servicio <see cref="IAudioService"/> mediante <see cref="AppContainer"/> y comienza la reproducción de la lista de música configurada
    /// </summary>
    protected virtual void Awake()
    {
        _audioService = AppContainer.Get<IAudioService>();
        _audioService.PlayMusic(musiclist);
    }

   
}
