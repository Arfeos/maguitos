using System;
using TMPro;
using UnityEngine;
/// <summary>
/// Componente de Unity encargado de gestionar la creación de perfiles de usuario desde la interfaz. 
/// Obtiene los datos introducidos por el jugador, recibe la imagen seleccionada mediante eventos y utiliza <see cref="IProfileService"/> para crear un objeto <see cref="UserProfile"/>. 
/// También emplea <see cref="IEventService"/> para escuchar cambios relacionados con iconos
/// </summary>
public class SubmitUser : MonoBehaviour
{
    /// <summary>
    /// Campo de entrada utilizado para introducir el nombre del usuario
    /// </summary>
    [SerializeField] private TMP_InputField m_Input;
    /// <summary>
    /// Elemento desplegable utilizado para seleccionar el idioma asociado al perfil
    /// </summary>
    [SerializeField] private TMP_Dropdown dropdown;
    string newIcon;
    IEventService eventService;
    private IProfileService profileService;
    /// <summary>
    /// Método ejecutado al comenzar la escena. Obtiene referencias a los servicios <see cref="IProfileService"/> y <see cref="IEventService"/> mediante <see cref="AppContainer"/> y registra el método saveIcon() como suscriptor del evento <see cref="IconChangeEvent"/>
    /// </summary>
    public void Start()
    {
        profileService = AppContainer.Get<IProfileService>();
            eventService = AppContainer.Get<IEventService>();
        eventService.Subscribe<IconChangeEvent>(saveIcon);

    }
    /// <summary>
    /// Método encargado de almacenar la información del icono recibido mediante un evento <see cref="IconChangeEvent"/>
    /// </summary>
    /// <param name="base">Evento recibido que contiene los datos del nuevo icono. Se convierte internamente a <see cref="IconChangeEvent"/></param>
    private void saveIcon(GameEventBase @base)
    {
        IconChangeEvent data= (IconChangeEvent) @base;
        newIcon = data.newIconUrl;
    }
    /// <summary>
    /// Método encargado de crear un nuevo perfil utilizando la información introducida por el usuario. Obtiene el nombre desde TMP_InputField, configura el idioma mediante <see cref="Languages"/> y crea el perfil utilizando <see cref="IProfileService"/>
    /// </summary>
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