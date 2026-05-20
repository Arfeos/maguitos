using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathPanel : MonoBehaviour
{   
    private IScoreService _scoreService;
    private ICharacterService _characterService;
    private ISceneService _sceneService;
    private bool performedAction=false;
    private void Start()
    {
        _sceneService = AppContainer.Get<ISceneService>();
        _scoreService = AppContainer.Get<IScoreService>(); 
        _characterService = AppContainer.Get<ICharacterService>();
    }

    void Update()
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
        }
    }
}
