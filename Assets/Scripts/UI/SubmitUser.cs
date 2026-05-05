using System;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

public class SubmitUser : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_Input;
    [SerializeField] private TMP_Dropdown dropdown;
    string newIcon;
    IEventService eventService;
    private IProfileService profileService;
   
    public void Start()
    {
        profileService = AppContainer.Get<IProfileService>();
            eventService = AppContainer.Get<IEventService>();
        eventService.Subscribe<IconChangeEvent>(saveIcon);

    }

    private void saveIcon(GameEventBase @base)
    {
        IconChangeEvent data= (IconChangeEvent) @base;
        newIcon = data.newIconUrl;
    }

    public void CreateUser()
    {
        string name = m_Input.text;

        if (string.IsNullOrWhiteSpace(name))
        {
            Debug.Log("Nombre vacío");
            return;
        }

        Settings settings = new Settings();
        Languages selectedLang = (Languages)dropdown.value;
        settings.language = selectedLang;
        if (newIcon != null) 
            profileService.CreateProfile(name, settings, newIcon);
        
        else 
            profileService.CreateProfile(name, settings);


        Debug.Log("Perfil creado: " + name);
    }
}