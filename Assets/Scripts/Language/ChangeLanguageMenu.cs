using UnityEngine;

public class ChangeLanguage : MonoBehaviour
{
    [SerializeField] Languages language;
    IUIService uiService;
    private void Start()
    {
        uiService = AppContainer.Get<IUIService>();
    }
    public void Change()
    {
        uiService.changeLanguage(language);
    }
}
