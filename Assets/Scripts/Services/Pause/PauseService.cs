using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseService : IPauseService
{

    private GameObject PausepanelPrefab;
    private GameObject PausepanelInstance;
    private GameObject SettingpanelPrefab;
    private GameObject SettingpanelInstance;
    
    public PauseService(PanelConfigurationScriptable PauseConfig, PanelConfigurationScriptable SettingConfig)

    {       
        this.SettingpanelPrefab = SettingConfig.Panel;
        this.PausepanelPrefab= PauseConfig.Panel;
    }

    public void TogglePause()
    {
        PausepanelInstance = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(g => (g.name == PausepanelPrefab.name && g.scene == SceneManager.GetActiveScene()) || (g.name == PausepanelPrefab.name+"(Clone)" && g.scene == SceneManager.GetActiveScene()) );
        Debug.Log(PausepanelInstance);
        if (PausepanelInstance == null) {

            //No queremos que encuentre canvas 3d la distincion entre un canvas 2d y uno 3d es RenderMode.WorldSpace 
            Canvas canvas = Resources.FindObjectsOfTypeAll<Canvas>()
                        .FirstOrDefault(c => c.renderMode != RenderMode.WorldSpace
                            && c.gameObject.scene == SceneManager.GetActiveScene());
            PausepanelInstance = GameObject.Instantiate(PausepanelPrefab, canvas.transform);
        }
        if (PausepanelInstance != null) {
            if (!PausepanelInstance.activeInHierarchy)
            {
                //if (tipo == GameType.offline) Time.timeScale = 0;
                
                Time.timeScale = 0;
                PausepanelInstance.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                PlayerInputManager.SwitchControlMap(PlayerInputManager.ControlMap.UI);
            }
            else {
                //if (tipo == GameType.offline) Time.timeScale = 1;
                Time.timeScale = 1;
                PausepanelInstance.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                PlayerInputManager.SwitchControlMap(PlayerInputManager.ControlMap.Player);
            }
        }
       
    }
    public void ToggleSettings()
    {
        if (SettingpanelInstance == null)
        {
            SettingpanelInstance = GameObject.Instantiate(SettingpanelPrefab);
            SettingpanelInstance.SetActive(true);
            PausepanelInstance.SetActive(false);
        }
        else
        {
            if (!SettingpanelInstance.activeInHierarchy)
            {
                SettingpanelInstance.SetActive(true);
                PausepanelInstance.SetActive(false);
            }
            else
            {
                SettingpanelInstance.SetActive(false);
                PausepanelInstance.SetActive(true);
            }
        }



    }  
}
