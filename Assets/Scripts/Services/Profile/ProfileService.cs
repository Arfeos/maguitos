using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Implementación de <see cref="IProfileService"/> que persiste los perfiles de usuario
/// como archivos JSON en <see cref="Application.persistentDataPath"/>/profiles.
/// Cada archivo se nombra con el GUID del perfil.
/// </summary>
public class ProfileService : IProfileService
{
    // ── Estado interno ───────────────────────────────────────────────────────
    /// <summary>Lista de perfiles cargados en memoria.</summary>
    private List<UserProfile> profiles = new List<UserProfile>();

    /// <summary>Perfil activo seleccionado por el jugador en la sesión actual.</summary>
    private UserProfile selectedProfile;
    /// <summary>Servicio de UI, resuelto de forma lazy al seleccionar un perfil para aplicar el idioma.</summary>
    IUIService uiService;
    /// <summary>Ruta de la carpeta donde se almacenan los archivos JSON de perfiles.</summary>
    private string folderPath => Application.persistentDataPath + "/profiles";


    // ── IProfileService ──────────────────────────────────────────────────────

    /// <summary>
    /// Lee todos los archivos JSON de la carpeta de perfiles y los carga en memoria.
    /// Asigna el GUID a cada perfil a partir del nombre del archivo.
    /// Si la carpeta no existe, la crea y retorna sin cargar nada.
    /// </summary>
    public void LoadProfiles()
    {
        profiles.Clear();

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            return;
        }

        string[] files = Directory.GetFiles(folderPath, "*.json");

        foreach (var file in files)
        {
            string json = File.ReadAllText(file);

            UserProfile p = JsonUtility.FromJson<UserProfile>(json);

            // GUID = nombre del archivo
            p.guid = Path.GetFileNameWithoutExtension(file);

            profiles.Add(p);
        }
    }

    /// <summary>
    /// Recarga los perfiles desde disco y devuelve la lista actualizada.
    /// </summary>
    /// <returns>Lista de todos los perfiles almacenados.</returns>
    public List<UserProfile> GetProfiles()
    {
        LoadProfiles();
        return profiles;
    }

    /// <summary>
    /// Crea un nuevo perfil, le asigna un GUID único, lo serializa a JSON y lo guarda en disco.
    /// </summary>
    /// <param name="name">Nombre del perfil.</param>
    /// <param name="settings">Configuración inicial del perfil (idioma, etc.).</param>
    /// <param name="urlImage">Ruta del icono del perfil dentro de Resources. Vacío por defecto.</param>
    public void CreateProfile(string name, Settings settings, string urlImage = "")
    {
        string guid = System.Guid.NewGuid().ToString();

        UserProfile newProfile = new UserProfile(name, urlImage, settings);
        newProfile.guid = guid;

        string json = JsonUtility.ToJson(newProfile, true);

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string path = Path.Combine(folderPath, guid + ".json");

        File.WriteAllText(path, json);

        profiles.Add(newProfile);
    }

    // <summary>
    /// Elimina el perfil con el GUID indicado del disco y de la lista en memoria.
    /// Si era el perfil seleccionado, limpia la selección.
    /// </summary>
    /// <param name="guid">GUID del perfil a eliminar.</param>
    public void DeleteProfile(string guid)
    {
        string path = Path.Combine(folderPath, guid + ".json");

        if (File.Exists(path))
            File.Delete(path);

        profiles.RemoveAll(p => p.guid == guid);

        if (selectedProfile != null && selectedProfile.guid == guid)
            selectedProfile = null;
    }

    /// <summary>
    /// Establece el perfil indicado como activo y aplica su idioma configurado mediante <see cref="IUIService"/>.
    /// </summary>
    /// <param name="profile">Perfil a seleccionar.</param>
    public void SelectProfile(UserProfile profile)
    {
        if(uiService == null)
            uiService = AppContainer.Get<IUIService>();
        selectedProfile = profile;
        Languages lang= (Languages) selectedProfile.settings.language;
        uiService.changeLanguage(lang);
    }

    /// <summary>
    /// Sobrescribe el archivo JSON del perfil indicado con sus datos actuales.
    /// No hace nada si el perfil es <c>null</c> o no tiene GUID.
    /// </summary>
    /// <param name="profile">Perfil con los datos actualizados a persistir.</param>
    public void UpdateProfile(UserProfile profile)
    {
        if (profile == null || string.IsNullOrEmpty(profile.guid))
            return;

        string path = Path.Combine(folderPath, profile.guid + ".json");

        string json = JsonUtility.ToJson(profile, true);

        File.WriteAllText(path, json);
    }
    /// <summary>
    /// Devuelve el perfil activo seleccionado en la sesión actual, o <c>null</c> si no hay ninguno.
    /// </summary>
    public UserProfile getSelectedProfile() => selectedProfile;
}