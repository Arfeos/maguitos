using UnityEngine;
using UnityEngine.UI;

public class UIInitializer : MonoBehaviour
{
    private IUIService uiService;
    private Button[] foundButtons;
    void Start()
    {
        foundButtons= GameObject.FindObjectsByType<Button>(FindObjectsSortMode.None);
        //TODO: crear un comprobante para ver si el control ya esta en UI
        PlayerInputManager.SwitchControlMap(PlayerInputManager.ControlMap.UI);
        uiService = AppContainer.Get<IUIService>();
        uiService.RegisterFirstButton(foundButtons);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
