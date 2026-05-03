using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class UIService: IUIService
{
     public Selectable FirstButton { get; private set; }

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

    public void RegisterFirstButton(Selectable[] foundButtons)
    {

        if (foundButtons.Length > 0)
        {
            FirstButton = foundButtons[0];
            EventSystem.current.SetSelectedGameObject(FirstButton.gameObject);
        }
        else
        {
            FirstButton = null;
        }
    }
}
