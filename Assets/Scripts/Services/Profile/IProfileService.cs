using System.Collections.Generic;

/// <summary>
/// Implementación de <see cref="IProfileService"/> que persiste los perfiles de usuario
/// como archivos JSON en <see cref="Application.persistentDataPath"/>/profiles.
/// Cada archivo se nombra con el GUID del perfil.
/// </summary>
public interface IProfileService
{
    /// <summary>
    /// Lee todos los archivos JSON de la carpeta de perfiles y los carga en memoria.
    /// Asigna el GUID a cada perfil a partir del nombre del archivo.
    /// Si la carpeta no existe, la crea y retorna sin cargar nada.
    /// </summary>
    void LoadProfiles();

    /// <summary>
    /// Recarga los perfiles desde disco y devuelve la lista actualizada.
    /// </summary>
    /// <returns>Lista de todos los perfiles almacenados.</returns>
    List<UserProfile> GetProfiles();

    /// <summary>
    /// Establece el perfil indicado como activo y aplica su idioma configurado mediante <see cref="IUIService"/>.
    /// </summary>
    /// <param name="profile">Perfil a seleccionar.</param>
    void SelectProfile(UserProfile profile);

    /// <summary>
    /// Crea un nuevo perfil, le asigna un GUID único, lo serializa a JSON y lo guarda en disco.
    /// </summary>
    /// <param name="name">Nombre del perfil.</param>
    /// <param name="settings">Configuración inicial del perfil (idioma, etc.).</param>
    /// <param name="urlImage">Ruta del icono del perfil dentro de Resources. Vacío por defecto.</param>
    void CreateProfile(string name, Settings settings, string urlImage ="");

    // <summary>
    /// Elimina el perfil con el GUID indicado del disco y de la lista en memoria.
    /// Si era el perfil seleccionado, limpia la selección.
    /// </summary>
    /// <param name="guid">GUID del perfil a eliminar.</param>
    void DeleteProfile(string guid);

    /// <summary>
    /// Sobrescribe el archivo JSON del perfil indicado con sus datos actuales.
    /// No hace nada si el perfil es <c>null</c> o no tiene GUID.
    /// </summary>
    /// <param name="profile">Perfil con los datos actualizados a persistir.</param>
    void UpdateProfile(UserProfile profile);

    /// <summary>
    /// Devuelve el perfil activo seleccionado en la sesión actual, o <c>null</c> si no hay ninguno.
    /// </summary>
    public UserProfile getSelectedProfile();
}