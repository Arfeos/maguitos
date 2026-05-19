using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanel : MonoBehaviour
{
    private ISceneService _sceneService;
    private IPauseService _pauseService;
    private ICharacterService _characterService;
    private IScoreService _scoreService;
    private void Start()
    {
        _sceneService = AppContainer.Get<ISceneService>();
        _pauseService = AppContainer.Get<IPauseService>();
        _characterService = AppContainer.Get<ICharacterService>();
        _scoreService = AppContainer.Get<IScoreService>();
    }
    void Update()
    {
        if (PlayerInputManager.Actions.UI.pause.WasPressedThisFrame()) Resume(); 
    }

    public void Resume()
    {
        _pauseService.TogglePause();
    }
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
    public void Settings() { 
        _pauseService.ToggleSettings();
    }
    public void mainMenu() {

            _sceneService.LoadScene(SceneNames.Main_menu);
            Time.timeScale = 1;
       
    }
}
