using UnityEngine;
using UnityEngine.UI;

public class UIInitializer : MonoBehaviour
{
    private IUIService uiService;
    [SerializeField] private Button[] foundButtons;
    void Start()
    {
        uiService = AppContainer.Get<IUIService>();
        uiService.RegisterFirstButton(foundButtons);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
