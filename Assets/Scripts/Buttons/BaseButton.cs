using UnityEngine;

/// <summary>
/// Clase base para botones de la UI que reproducen un sonido al ser pulsados.
/// Las subclases pueden sobreescribir <see cref="PlaySound"/> para añadir comportamiento extra
/// o asignar su propio <see cref="_audioClip"/> desde el Inspector.
/// </summary>
public class BaseButton : MonoBehaviour
{
    // ── Servicios ────────────────────────────────────────────────────────────
    /// <summary>Servicio de audio inyectado desde el contenedor de dependencias.</summary>
    protected IAudioService _audioService;

    // ── Configuración ────────────────────────────────────────────────────────
    /// <summary>Sonido que se reproduce al pulsar el botón.</summary>
    [SerializeField] protected AudioClip _audioClip;


    // ── Unity Lifecycle ──────────────────────────────────────────────────────

    /// <summary>
    /// Resuelve el servicio de audio desde el contenedor de la aplicación.
    /// </summary>
    protected virtual void Awake()
    {
        _audioService = AppContainer.Get<IAudioService>();
    }

    // ── Sonido ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Reproduce el <see cref="_audioClip"/> asignado a través del servicio de audio.
    /// Sobreescribe en subclases para añadir lógica adicional al pulsar el botón.
    /// </summary>
    public virtual void PlaySound()
    {
        _audioService.PlaySound(_audioClip);
    }
}
