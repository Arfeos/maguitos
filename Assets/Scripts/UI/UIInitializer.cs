using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIInitializer : MonoBehaviour
{
    private IUIService uiService;

    [SerializeField] GameObject firstButton;
    void Start()
    { 
        //TODO: crear un comprobante para ver si el control ya esta en UI
        PlayerInputManager.SwitchControlMap(PlayerInputManager.ControlMap.UI);
        uiService = AppContainer.Get<IUIService>();
        uiService.RegisterFirstButton(firstButton);
    }

}
