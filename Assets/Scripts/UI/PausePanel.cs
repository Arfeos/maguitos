using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// MonoBehaviour que gestiona el panel de pausa del juego,
/// permitiendo reanudar la partida, reiniciar el nivel, acceder a ajustes
/// o volver al menú principal.
/// </summary>
public class PausePanel : MonoBehaviour
{
    /// <summary>Referencia al servicio de escenas para cargar o reiniciar niveles.</summary>
    private ISceneService _sceneService;
    /// <summary>Referencia al servicio de pausa para activar o desactivar el estado de pausa.</summary>
    private IPauseService _pauseService;
    /// <summary>Referencia al servicio de personaje para resetear su estado al salir de la partida.</summary>
    private ICharacterService _characterService;
    /// <summary>Referencia al servicio de puntuación para resetear los puntos al salir de la partida.</summary>
    private IScoreService _scoreService;

    /// <summary>
    /// Obtiene las referencias a los servicios necesarios al inicio del ciclo de vida del componente.
    /// </summary>
    private void Start()
    {
        _sceneService = AppContainer.Get<ISceneService>();
        _pauseService = AppContainer.Get<IPauseService>();
        _characterService = AppContainer.Get<ICharacterService>();
        _scoreService = AppContainer.Get<IScoreService>();
    }

    /// <summary>
    /// Comprueba cada frame si el jugador ha pulsado el botón de pausa para reanudar la partida.
    /// </summary>
    void Update()
    {
        if (PlayerInputManager.Actions.UI.pause.WasPressedThisFrame()) Resume(); 
    }

    /// <summary>
    /// Reanuda la partida alternando el estado de pausa mediante el <see cref="IPauseService"/>.
    /// </summary>
    public void Resume()
    {
        _pauseService.TogglePause();
    }

    /// <summary>
    /// Reinicia el nivel actual reseteando el personaje y la puntuación,
    /// y recargando la escena activa. Restaura el <see cref="Time.timeScale"/> a 1
    /// para asegurar que el juego corre a velocidad normal.
    /// Si el nombre de la escena activa no corresponde a ningún valor de <see cref="SceneNames"/>,
    /// registra un error en consola y no realiza ninguna acción.
    /// </summary>
    public void RestartLevel()
    {

        if (Enum.TryParse<SceneNames>(SceneManager.GetActiveScene().name, out SceneNames actualScene)) {
            _characterService.ResetCharacter();
            _scoreService.resetScore();
            _sceneService.LoadScene(actualScene);
            Time.timeScale = 1;
        }
        else Debug.Log("enum no valido");
    }

    /// <summary>
    /// Abre o cierra el panel de ajustes mediante el <see cref="IPauseService"/>.
    /// </summary>
    public void Settings() { 
        _pauseService.ToggleSettings();
    }

    /// <summary>
    /// Vuelve al menú principal reseteando el personaje y la puntuación,
    /// cargando la escena <see cref="SceneNames.Main_menu"/> y restaurando
    /// el <see cref="Time.timeScale"/> a 1.
    /// </summary>
    public void mainMenu() {
        _characterService.ResetCharacter();
        _scoreService.resetScore();
        _sceneService.LoadScene(SceneNames.Main_menu);
            Time.timeScale = 1;
       
    }
}
