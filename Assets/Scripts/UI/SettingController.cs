using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SettingController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown Language;
    [SerializeField] private Toggle invertX;
    [SerializeField] private Toggle invertY;
    [SerializeField] private Slider MusicSound;
    [SerializeField] private Slider sfxSound;
    [SerializeField] private Slider sensibility;
    IEventService _eventService;
    IProfileService profileService;
    IPauseService pauseService;
    IAudioService audioService;
    private UserProfile profile;
     private void Awake()
     {
         profileService = AppContainer.Get<IProfileService>();
        pauseService = AppContainer.Get<IPauseService>();
        _eventService = AppContainer.Get<IEventService>();
        audioService = AppContainer.Get<IAudioService>();

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
     yield return null;
        profile = profileService.getSelectedProfile();
        
        if (profile == null)
            yield break;
        LoadSettings();
        AddListeners();

    }
    private void OnDestroy()
    {
        Language.onValueChanged.RemoveListener(OnLanguageChanged);
        invertX.onValueChanged.RemoveListener(OnInvertXChanged);
        invertY.onValueChanged.RemoveListener(OnInvertYChanged);
        MusicSound.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        sfxSound.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        sensibility.onValueChanged.RemoveListener(OnSensibilityChanged);
    }

    private void AddListeners()
    {
        Language.onValueChanged.AddListener(OnLanguageChanged);
        invertX.onValueChanged.AddListener(OnInvertXChanged);
        invertY.onValueChanged.AddListener(OnInvertYChanged);
        MusicSound.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSound.onValueChanged.AddListener(OnSFXVolumeChanged);
        sensibility.onValueChanged.AddListener(OnSensibilityChanged);

    }


    private void LoadSettings()
    {
        Language.value = (int)profile.settings.language;
        invertX.isOn = profile.settings.axisXDirection == -1;
        invertY.isOn = profile.settings.axisYDirection == -1;
        MusicSound.value = profile.settings.musicVolume;
        sfxSound.value = profile.settings.masterVolume;
        sensibility.value = profile.settings.sensibility;
    }
    private void OnSFXVolumeChanged(float value)
    {
        profile.settings.masterVolume = value;
        audioService.SetSFXVolume(value);
        save();
    }

    private void OnMusicVolumeChanged(float value)
    {
        profile.settings.musicVolume = value;
        audioService.SetMusicVolume(value);
        save();
    }

    private void OnInvertYChanged(bool value)
    {
        profile.settings.axisYDirection = value ? -1 : 1;
        save();
    }

    private void OnInvertXChanged(bool value)
    {
        profile.settings.axisXDirection = value ? -1 : 1;
        save();
    }

    private void OnLanguageChanged(int language)
    {
        profile.settings.language = (Languages)language;
        save();
    }
    private void OnSensibilityChanged(float value)
    {
        profile.settings.sensibility = value;
        save();
    }
    private void save()
    {
        profileService.UpdateProfile(profile);
        _eventService.Publish(new PreferenceChangeEvent());
       
    }
    public void ReOpenPause() { 
        pauseService.ToggleSettings();
    }
}
