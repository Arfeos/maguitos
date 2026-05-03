using TMPro;
using UnityEngine;

public class SubmitUser : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_Input;
    [SerializeField] private TMP_Dropdown dropdown;

    private IProfileService profileService;
   
    public void Start()
    {
        profileService = AppContainer.Get<IProfileService>();
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
        //settings.language = selectedLang;
        profileService.CreateProfile(name, settings);
        Debug.Log("Perfil creado: " + name);
    }
}