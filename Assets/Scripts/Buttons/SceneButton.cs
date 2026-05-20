using UnityEngine;

public class SceneButton : BaseButton
{
    [SerializeField] private SceneNames sceneName;

    private ISceneService sceneService;

    private void Start()
    {
        sceneService = AppContainer.Get<ISceneService>();
    }

    public void LoadScene()
    {
        sceneService.LoadScene(sceneName);
    }
}