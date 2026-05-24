using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Panel que aparece al morir el jugador. Escucha el input de pausa para reiniciar la partida:
/// resetea el personaje, la puntuación y recarga la escena actual restaurando el <see cref="Time.timeScale"/>.
/// La acción solo puede ejecutarse una vez por sesión de panel gracias a <see cref="performedAction"/>.
/// </summary>
public class DeathPanel : MonoBehaviour
{
    // ── Servicios ────────────────────────────────────────────────────────────
    /// <summary>Servicio de puntuación usado para resetear el score al reiniciar.</summary>
    private IScoreService _scoreService;

    /// <summary>Servicio de personaje usado para resetear su estado al reiniciar.</summary>
    private ICharacterService _characterService;

    /// <summary>Servicio de escenas usado para recargar la escena actual.</summary>
    private ISceneService _sceneService;


    // ── Estado interno ───────────────────────────────────────────────────────
    /// <summary>
    /// Evita que el reinicio se ejecute más de una vez si el input se detecta en varios frames seguidos.
    /// </summary>
    private bool performedAction = false;


    // ── Unity Lifecycle ──────────────────────────────────────────────────────

    /// <summary>
    /// Resuelve los servicios necesarios desde el contenedor de la aplicación.
    /// </summary>
    private void Start()
    {
        _sceneService = AppContainer.Get<ISceneService>();
        _scoreService = AppContainer.Get<IScoreService>();
        _characterService = AppContainer.Get<ICharacterService>();
    }

    /// <summary>
    /// Escucha el input de pausa cada frame. Al detectarlo, resetea el personaje y la puntuación,
    /// restaura el <see cref="Time.timeScale"/> y recarga la escena actual.
    /// Solo actúa si el nombre de la escena activa puede parsearse como <see cref="SceneNames"/>.
    /// </summary>
    private void Update()
    {
        if (performedAction)
            return;

        if (PlayerInputManager.Actions.UI.pause.WasPerformedThisFrame())
        {
            performedAction = true;

            if (Enum.TryParse<SceneNames>(
                SceneManager.GetActiveScene().name,
                out SceneNames actualScene))
            {
                Time.timeScale = 1;

                _characterService.ResetCharacter();
                _scoreService.resetScore();

                _sceneService.LoadScene(actualScene);
            }
            else
            {
                Debug.LogWarning("El nombre de la escena activa no coincide con ningún Valor en SceneNames");
            }
        }
    }
}
