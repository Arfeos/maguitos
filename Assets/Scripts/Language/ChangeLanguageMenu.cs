using UnityEngine;

public class ChangeLanguage : MonoBehaviour
{
    [SerializeField] Languages language;
    IUIService uiService;
    ISceneService sceneService;
    private void Start()
    {
        uiService = AppContainer.Get<IUIService>();
        sceneService = AppContainer.Get<ISceneService>();
    }
    public void Change()
    {
        uiService.changeLanguage(language);
    }
    public void changeByInt(int index) { 
        language = (Languages)index;
            Change();
    }
}
