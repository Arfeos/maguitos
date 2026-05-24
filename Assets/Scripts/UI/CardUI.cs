using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Representa visualmente un perfil de usuario en la pantalla de selección de perfiles.
/// Muestra el nombre e icono del perfil y permite seleccionarlo o eliminarlo.
/// </summary>
public class CardUI : MonoBehaviour
{
    // ── Referencias UI ───────────────────────────────────────────────────────
    /// <summary>Texto que muestra el nombre del perfil.</summary>
    [SerializeField] private TMP_Text nameText;

    /// <summary>Imagen que muestra el icono del perfil, cargado desde Resources.</summary>
    [SerializeField] private Image icon;

    // ── Estado interno ───────────────────────────────────────────────────────
    /// <summary>Identificador único del perfil, usado para eliminarlo sin mantener el objeto completo.</summary>
    private String guid;

    /// <summary>Datos completos del perfil representado por esta tarjeta.</summary>
    private UserProfile profile;

    // ── Servicios ────────────────────────────────────────────────────────────
    /// <summary>Servicio de perfiles para seleccionar o eliminar el perfil.</summary>
    private IProfileService _profileService;

    /// <summary>Servicio de escenas para navegar al menú principal tras seleccionar el perfil.</summary>
    private ISceneService _sceneService;


    // ── Setup ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Inicializa la tarjeta con los datos del perfil: asigna nombre, guid, icono y resuelve servicios.
    /// Debe llamarse justo después de instanciar la tarjeta.
    /// </summary>
    /// <param name="profile">Perfil de usuario cuyos datos se mostrarán en la tarjeta.</param>
    public void Setup(UserProfile profile)
    {
        nameText.text = profile.name;
        this.guid = profile.guid;
        this.profile = profile;

        _profileService = AppContainer.Get<IProfileService>();
        _sceneService = AppContainer.Get<ISceneService>();

        Debug.Log("Cargando imagen desde URL: " + profile.urlImage);
        icon.sprite = Resources.Load<Sprite>(profile.urlImage);
    }

    // ── Callbacks ────────────────────────────────────────────────────────────

    /// <summary>
    /// Selecciona este perfil como activo y navega al menú principal.
    /// Llamar desde el evento OnClick de la tarjeta en el Inspector.
    /// </summary>
    public void OnClick()
    {
        _profileService.SelectProfile(profile);

        Debug.Log("Perfil seleccionado: " + _profileService.getSelectedProfile().name +
                  " GUID: " + _profileService.getSelectedProfile().guid);

        _sceneService.LoadScene(SceneNames.Main_menu);
    }

    /// <summary>
    /// Elimina el perfil del servicio de perfiles y recarga la escena actual para refrescar la lista.
    /// Llamar desde el evento OnClick del botón de eliminar en el Inspector.
    /// </summary>
    public void DeleteProfile()
    {
        _profileService.DeleteProfile(guid);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
