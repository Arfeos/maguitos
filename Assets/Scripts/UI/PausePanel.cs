using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanel : MonoBehaviour
{
    private ISceneService _sceneService;
    private IPauseService _pauseService;

    private void Start()
    {
        _sceneService = AppContainer.Get<ISceneService>();
        _pauseService = AppContainer.Get<IPauseService>();
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
            _sceneService.LoadScene(actualScene);
            Time.timeScale = 1;
        }
        else Debug.Log("enum no valido");
    }
}
