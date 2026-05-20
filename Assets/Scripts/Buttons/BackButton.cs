using UnityEngine;

public class BackButton : BaseButton
{
    ISceneService sceneService;
    void Start()
    {
        sceneService = AppContainer.Get<ISceneService>();
    }

    public void back()
    {
        sceneService.GoBack();

    }
}
