using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Componente de Unity que se encarga de cargar y mostrar perfiles de usuario en la UI al iniciar la escena
/// </summary>
public class ProfileLoader : MonoBehaviour
{
    IProfileService profileService;
    List<UserProfile> profiles;
    [SerializeField]GameObject cardPrefab;
    void Start()
    {
        SetupProfiles();
    }
    /// <summary>
    /// Obtiene la lista de perfiles a través del servicio <see cref="IProfileService"> inyectado desde <see cref="AppContainer">. 
    /// Si no hay perfiles, loguea un mensaje y sale. 
    /// Por cada perfil existente, instancia un cardPrefab como hijo del transform actual y llama a Setup() en su componente <see cref="CardUI"> para inicializarlo con los datos del perfil
    /// </summary>
    public void SetupProfiles()
    {
        profileService= AppContainer.Get<IProfileService>();
        profiles = profileService.GetProfiles();
        if (profiles.Count <= 0) {
            Debug.Log("no hay perfiles");
            return;
        }
        foreach (UserProfile profiledata in profiles)
        {
            var card = Instantiate(cardPrefab, transform).GetComponent<CardUI>();
            card.Setup(profiledata);
        }
    }

}
