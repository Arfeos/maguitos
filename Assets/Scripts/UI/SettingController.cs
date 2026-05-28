using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// MonoBehaviour que gestiona el panel de ajustes de usuario,
/// cargando la configuración del perfil activo, enlazando los controles de UI
/// con los valores de <see cref="Settings"/> y persistiendo los cambios en tiempo real.
/// </summary>
public class SettingController : MonoBehaviour
{
    /// <summary>Dropdown para seleccionar el idioma de la aplicación.</summary>
    [SerializeField] private TMP_Dropdown Language;
    /// <summary>Toggle para activar o desactivar la inversión del eje horizontal.</summary>
    [SerializeField] private Toggle invertX;
    /// <summary>Toggle para activar o desactivar la inversión del eje vertical.</summary>
    [SerializeField] private Toggle invertY;
    /// <summary>Slider para ajustar el volumen de la música de fondo.</summary>
    [SerializeField] private Slider MusicSound;
    /// <summary>Slider para ajustar el volumen de los efectos de sonido.</summary>
    [SerializeField] private Slider sfxSound;
    /// <summary>Slider para ajustar la sensibilidad del control de cámara.</summary>
    [SerializeField] private Slider sensibility;
    /// <summary>Referencia al servicio de eventos para publicar <see cref="PreferenceChangeEvent"/> al guardar.</summary>
    IEventService _eventService;
    /// <summary>Referencia al servicio de perfiles para obtener y actualizar el perfil activo.</summary>
    IProfileService profileService;
    /// <summary>Referencia al servicio de pausa para cerrar el panel de ajustes.</summary>    
    IPauseService pauseService;
    /// <summary>Referencia al servicio de audio para aplicar los cambios de volumen en tiempo real.</summary>
    IAudioService audioService;
    /// <summary>Perfil del usuario activo cuyos ajustes se están editando.</summary>
    private UserProfile profile;

    /// <summary>
    /// Obtiene las referencias a los servicios necesarios al inicializarse el componente.
    /// </summary>
    private void Awake()
     {
         profileService = AppContainer.Get<IProfileService>();
        pauseService = AppContainer.Get<IPauseService>();
        _eventService = AppContainer.Get<IEventService>();
        audioService = AppContainer.Get<IAudioService>();

    }


    /// <summary>
    /// Espera un frame para asegurar que el perfil esté disponible, lo carga
    /// e inicializa los controles de UI y sus listeners.
    /// Si no hay perfil seleccionado, no realiza ninguna acción.
    /// </summary>
    IEnumerator Start()
    {
     yield return null;
        profile = profileService.getSelectedProfile();
        
        if (profile == null)
            yield break;
        LoadSettings();
        AddListeners();

    }

    /// <summary>
    /// Elimina todos los listeners de los controles de UI al destruirse el componente,
    /// evitando llamadas a métodos sobre objetos destruidos.
    /// </summary>
    private void OnDestroy()
    {
        Language.onValueChanged.RemoveListener(OnLanguageChanged);
        invertX.onValueChanged.RemoveListener(OnInvertXChanged);
        invertY.onValueChanged.RemoveListener(OnInvertYChanged);
        MusicSound.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        sfxSound.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        sensibility.onValueChanged.RemoveListener(OnSensibilityChanged);
    }


    /// <summary>
    /// Registra los callbacks de cambio en todos los controles de UI
    /// para detectar modificaciones del usuario en tiempo real.
    /// </summary>
    private void AddListeners()
    {
        Language.onValueChanged.AddListener(OnLanguageChanged);
        invertX.onValueChanged.AddListener(OnInvertXChanged);
        invertY.onValueChanged.AddListener(OnInvertYChanged);
        MusicSound.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSound.onValueChanged.AddListener(OnSFXVolumeChanged);
        sensibility.onValueChanged.AddListener(OnSensibilityChanged);

    }

    /// <summary>
    /// Inicializa los controles de UI con los valores actuales del perfil cargado.
    /// </summary>
    private void LoadSettings()
    {
        Language.value = (int)profile.settings.language;
        invertX.isOn = profile.settings.axisXDirection == -1;
        invertY.isOn = profile.settings.axisYDirection == -1;
        MusicSound.value = profile.settings.musicVolume;
        sfxSound.value = profile.settings.masterVolume;
        sensibility.value = profile.settings.sensibility;
    }

    /// <summary>
    /// Llamado al cambiar el slider de efectos de sonido.
    /// Actualiza el volumen en el perfil, lo aplica al <see cref="IAudioService"/> y guarda.
    /// </summary>
    /// <param name="value">Nuevo valor del volumen de efectos de sonido (0 a 1).</param>
    private void OnSFXVolumeChanged(float value)
    {
        profile.settings.masterVolume = value;
        audioService.SetSFXVolume(value);
        save();
    }

    /// <summary>
    /// Llamado al cambiar el slider de música.
    /// Actualiza el volumen en el perfil, lo aplica al <see cref="IAudioService"/> y guarda.
    /// </summary>
    /// <param name="value">Nuevo valor del volumen de música (0 a 1).</param>
    private void OnMusicVolumeChanged(float value)
    {
        profile.settings.musicVolume = value;
        audioService.SetMusicVolume(value);
        save();
    }

    /// <summary>
    /// Llamado al cambiar el toggle de inversión del eje vertical.
    /// Guarda -1 si está activado, 1 si está desactivado.
    /// </summary>
    /// <param name="value"><c>true</c> para invertir el eje Y; <c>false</c> para dirección normal.</param>
    private void OnInvertYChanged(bool value)
    {
        profile.settings.axisYDirection = value ? -1 : 1;
        save();
    }

    /// <summary>
    /// Llamado al cambiar el toggle de inversión del eje horizontal.
    /// Guarda -1 si está activado, 1 si está desactivado.
    /// </summary>
    /// <param name="value"><c>true</c> para invertir el eje X; <c>false</c> para dirección normal.</param>
    private void OnInvertXChanged(bool value)
    {
        profile.settings.axisXDirection = value ? -1 : 1;
        save();
    }
    /// <summary>
    /// Llamado al cambiar el dropdown de idioma.
    /// Actualiza el idioma en el perfil y guarda.
    /// </summary>
    /// <param name="language">Índice del idioma seleccionado, mapeado al enum <see cref="Languages"/>.</param>
    private void OnLanguageChanged(int language)
    {
        profile.settings.language = (Languages)language;
        save();
    }

    /// <summary>
    /// Llamado al cambiar el slider de sensibilidad.
    /// Actualiza la sensibilidad en el perfil y guarda.
    /// </summary>
    /// <param name="value">Nuevo valor de sensibilidad.</param>
    private void OnSensibilityChanged(float value)
    {
        profile.settings.sensibility = value;
        save();
    }

    /// <summary>
    /// Persiste el perfil actualizado mediante el <see cref="IProfileService"/>
    /// y publica un <see cref="PreferenceChangeEvent"/> para notificar al resto del sistema.
    /// </summary>
    private void save()
    {
        profileService.UpdateProfile(profile);
        _eventService.Publish(new PreferenceChangeEvent());
       
    }

    /// <summary>
    /// Cierra el panel de ajustes volviendo al estado de pausa anterior
    /// mediante el <see cref="IPauseService"/>.
    /// </summary>
    public void ReOpenPause() { 
        pauseService.ToggleSettings();
    }
}
