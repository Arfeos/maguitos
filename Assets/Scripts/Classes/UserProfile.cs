/// <summary>
/// Clase serializable que representa el perfil de un usuario,
/// almacenando su identificador único, nombre, imagen y configuración personal.
/// Se persiste en disco para mantener los datos entre sesiones.
/// </summary>
[System.Serializable]
public class UserProfile
{
    /// <summary>Identificador único del perfil, generado automáticamente para distinguir perfiles con el mismo nombre.</summary>
    public string guid;
    /// <summary>Nombre visible del perfil de usuario.</summary>
    public string name;
    /// <summary>URL o ruta de la imagen asociada al perfil.</summary>
    public string urlImage;
    /// <summary>Configuración personal del usuario, incluyendo idioma, volúmenes y sensibilidad.</summary>
    public Settings settings;

    /// <summary>
    /// Crea un nuevo perfil de usuario con los datos proporcionados.
    /// El <see cref="guid"/> no se asigna en el constructor; debe generarse externamente.
    /// </summary>
    /// <param name="name">Nombre visible del perfil.</param>
    /// <param name="urlImage">URL o ruta de la imagen del perfil.</param>
    /// <param name="settings">Configuración personal inicial del perfil.</param>
    public UserProfile(string name, string urlImage, Settings settings)
    {
        this.name = name;
        this.urlImage = urlImage;
        this.settings = settings;
    }
}