using UnityEngine;

 
/// <summary>
/// Botón que navega a la escena anterior usando <see cref="ISceneService"/>.
/// Hereda la reproducción de sonido de <see cref="BaseButton"/>.
/// </summary>
public class BackButton : BaseButton
{
    // ── Servicios ────────────────────────────────────────────────────────────
    /// <summary>Servicio de escenas usado para volver a la escena anterior.</summary>
    private ISceneService sceneService;
 
    // ── Unity Lifecycle ──────────────────────────────────────────────────────
 
    /// <summary>
    /// Resuelve el servicio de escenas desde el contenedor de la aplicación.
    /// </summary>
    void Start()
    {
        sceneService = AppContainer.Get<ISceneService>();
    }
 
    // ── Callbacks ────────────────────────────────────────────────────────────
 
    /// <summary>
    /// Navega a la escena anterior. Llamar desde el evento OnClick del botón en el Inspector.
    /// </summary>
    public void Back()
    {
        sceneService.GoBack();
    }
}
