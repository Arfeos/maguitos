using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

/// <summary>
/// Servicio encargado de gestionar la interfaz de usuario,
/// incluyendo el cambio de idioma y el registro del primer elemento seleccionable
/// al navegar con mando o teclado.
/// </summary>
public class UIService: IUIService
{
    /// <summary>
    /// Primer elemento seleccionable de la UI activa, usado como punto de entrada
    /// para la navegación con mando o teclado.
    /// </summary>
    public Selectable FirstButton { get; private set; }

    /// <summary>
    /// Cambia el idioma de la aplicación al indicado, actualizando el locale
    /// del sistema de localización de Unity.
    /// </summary>
    /// <param name="language">Idioma destino definido en el enum <see cref="Languages"/>.</param>
    public void changeLanguage(Languages language)
    {
        switch(language)
        {
            case Languages.English:
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
                break;
            case Languages.Spanish:
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
                break;
            case Languages.Nya:
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[2];
                break;
        }
    }
    /// <summary>
    /// Registra el GameObject indicado como el elemento seleccionado actualmente
    /// en el <see cref="EventSystem"/>, permitiendo la navegación con mando o teclado
    /// desde ese punto.
    /// </summary>
    /// <param name="firstButton">GameObject del primer botón o elemento seleccionable de la UI.</param>
    public void RegisterFirstButton(GameObject firstButton)
    {
        EventSystem.current.SetSelectedGameObject(firstButton);

    }
}
