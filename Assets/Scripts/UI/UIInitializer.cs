using UnityEngine;
using UnityEngine.UI;

public class UIInitializer : MonoBehaviour
{
    private IUIService uiService;
    private Selectable[] foundButtons;
    void Start()
    {
        foundButtons= GameObject.FindObjectsByType<Selectable>(FindObjectsSortMode.None);
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
