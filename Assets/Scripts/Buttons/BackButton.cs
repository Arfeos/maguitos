using UnityEngine;

public class BackButton : MonoBehaviour
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
