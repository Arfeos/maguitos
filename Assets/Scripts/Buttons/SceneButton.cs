using UnityEngine;

/// <summary>
/// Botón que carga una escena específica usando <see cref="ISceneService"/>.
/// La escena de destino se configura desde el Inspector mediante el enum <see cref="SceneNames"/>.
/// Hereda la reproducción de sonido de <see cref="BaseButton"/>.
/// </summary>
public class SceneButton : BaseButton
{
    // ── Configuración ────────────────────────────────────────────────────────
    /// <summary>Escena de destino que se cargará al pulsar el botón.</summary>
    [SerializeField] private SceneNames sceneName;

    // ── Servicios ────────────────────────────────────────────────────────────
    /// <summary>Servicio de escenas usado para realizar la carga.</summary>
    private ISceneService sceneService;

    // ── Unity Lifecycle ──────────────────────────────────────────────────────

    /// <summary>
    /// Resuelve el servicio de escenas desde el contenedor de la aplicación.
    /// </summary>
    private void Start()
    {
        sceneService = AppContainer.Get<ISceneService>();
    }

    // ── Callbacks ────────────────────────────────────────────────────────────

    /// <summary>
    /// Carga la escena configurada en <see cref="sceneName"/>. 
    /// Llamar desde el evento OnClick del botón en el Inspector.
    /// </summary>
    public void LoadScene()
    {
        sceneService.LoadScene(sceneName);
    }
}