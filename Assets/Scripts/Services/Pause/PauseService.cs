using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Implementación de <see cref="IPauseService"/> que gestiona los paneles de pausa y ajustes.
/// Busca o instancia el panel de pausa en el canvas 2D de la escena activa,
/// controla el <see cref="Time.timeScale"/> y cambia el mapa de control del jugador al pausar o reanudar.
/// </summary>
public class PauseService : IPauseService
{
    // ── Prefabs ──────────────────────────────────────────────────────────────
    /// <summary>Prefab del panel de pausa, obtenido del <see cref="PanelConfigurationScriptable"/> en el constructor.</summary>
    private GameObject PausepanelPrefab;

    
    /// <summary>Prefab del panel de ajustes, obtenido del <see cref="PanelConfigurationScriptable"/> en el constructor.</summary>
    private GameObject SettingpanelPrefab;

    // ── Instancias ───────────────────────────────────────────────────────────
    /// <summary>Instancia activa del panel de pausa en la escena. Puede ser <c>null</c> si aún no se ha creado.</summary>
    private GameObject PausepanelInstance;
    /// <summary>Instancia activa del panel de ajustes en la escena. Puede ser <c>null</c> si aún no se ha creado.</summary>
    private GameObject SettingpanelInstance;

    // ── Constructor ──────────────────────────────────────────────────────────

    /// <summary>
    /// Inicializa el servicio con los prefabs de los paneles de pausa y ajustes.
    /// </summary>
    /// <param name="PauseConfig">Scriptable object que contiene el prefab del panel de pausa.</param>
    /// <param name="SettingConfig">Scriptable object que contiene el prefab del panel de ajustes.</param>
    public PauseService(PanelConfigurationScriptable PauseConfig, PanelConfigurationScriptable SettingConfig)

    {       
        this.SettingpanelPrefab = SettingConfig.Panel;
        this.PausepanelPrefab= PauseConfig.Panel;
    }

    // ── IPauseService ────────────────────────────────────────────────────────

    /// <summary>
    /// Alterna el estado del panel de pausa. Si no existe una instancia en la escena activa,
    /// la crea en el primer canvas 2D encontrado. Al pausar detiene el tiempo, libera el cursor
    /// y cambia al mapa de control UI; al reanudar restaura el tiempo, bloquea el cursor
    /// y vuelve al mapa de control del jugador.
    /// </summary>
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
            if (!PausepanelInstance.activeInHierarchy)
            {
                //if (tipo == GameType.offline) Time.timeScale = 0;

                Time.timeScale = 0;
                PausepanelInstance.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                PlayerInputManager.SwitchControlMap(PlayerInputManager.ControlMap.UI);
            }
            else
            {
                //if (tipo == GameType.offline) Time.timeScale = 1;
                Time.timeScale = 1;
                PausepanelInstance.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                PlayerInputManager.SwitchControlMap(PlayerInputManager.ControlMap.Player);
            }
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

    /// <summary>
    /// Alterna la visibilidad del panel de ajustes. Si no existe instancia, la crea y oculta el panel
    /// de pausa. Si ya existe, alterna entre mostrar ajustes (ocultando pausa) y mostrar pausa
    /// (ocultando ajustes).
    /// </summary>
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
