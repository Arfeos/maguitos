using System.Linq;
using UnityEngine;

public class PauseService : IPauseService
{

    private GameObject panelPrefab;
    private GameObject panelInstance;
    public PauseService(PanelConfigurationScriptable config)
    {       
        this.panelPrefab= config.Panel;
    }

    public void TogglePause()
    {
        panelInstance = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(g => g.name == panelPrefab.name);
        Debug.Log(panelInstance);
        if (panelInstance == null) {
            Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
            panelInstance = GameObject.Instantiate(panelPrefab, canvas.transform);
        }
        if (panelInstance != null) {
            if (!panelInstance.activeInHierarchy)
            {
                //if (tipo == GameType.offline) Time.timeScale = 0;
                Time.timeScale = 0;
                panelInstance.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                PlayerInputManager.SwitchControlMap(PlayerInputManager.ControlMap.UI);
            }
            else {
                //if (tipo == GameType.offline) Time.timeScale = 1;
                Time.timeScale = 1;
                panelInstance.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                PlayerInputManager.SwitchControlMap(PlayerInputManager.ControlMap.Player);
            }
        }
       
    }
}
