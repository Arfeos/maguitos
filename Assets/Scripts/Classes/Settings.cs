using UnityEngine;

/// <summary>
/// Clase serializable que representa la configuración de usuario de la aplicación,
/// incluyendo idioma, direcciones de ejes, volúmenes y sensibilidad.
/// Se persiste en disco para mantener las preferencias entre sesiones.
/// </summary>
[System.Serializable]
public class Settings
{
    /// <summary>Idioma seleccionado por el usuario.</summary>
    public Languages language;
    /// <summary>Dirección del eje horizontal del ratón o joystick: 1 para normal, -1 para invertido.</summary>
    public int axisXDirection;
    /// <summary>Dirección del eje vertical del ratón o joystick: 1 para normal, -1 para invertido.</summary>
    public int axisYDirection;
    /// <summary>Volumen de la música de fondo, en un rango de 0 a 1.</summary>
    public float musicVolume;
    /// <summary>Volumen maestro general de la aplicación, en un rango de 0 a 1.</summary>
    public float masterVolume;
    /// <summary>Sensibilidad del ratón o joystick para el control de cámara.</summary>
    public float sensibility;

    /// <summary>
    /// Inicializa la configuración con los valores por defecto:
    /// idioma inglés, ejes sin invertir, volúmenes al máximo y sensibilidad estándar.
    /// </summary>
    public Settings()
    {
        language = Languages.English;
        axisXDirection = 1;
        axisYDirection = 1;
        musicVolume = 1f;
        masterVolume = 1f;
        sensibility = 1f;
    }

    /// <summary>
    /// Inicializa la configuración con los valores especificados por el usuario.
    /// </summary>
    /// <param name="language">Idioma seleccionado.</param>
    /// <param name="axisXDirection">Dirección del eje horizontal: 1 para normal, -1 para invertido.</param>
    /// <param name="axisYDirection">Dirección del eje vertical: 1 para normal, -1 para invertido.</param>
    /// <param name="musicVolume">Volumen de la música de fondo (0 a 1).</param>
    /// <param name="masterVolume">Volumen maestro general (0 a 1).</param>
    /// <param name="sensibility">Sensibilidad del control de cámara.</param>
    public Settings(Languages language, int axisXDirection, int axisYDirection, float musicVolume, float masterVolume, float sensibility)
    {
        this.language = language;
        this.axisXDirection = axisXDirection;
        this.axisYDirection = axisYDirection;
        this.musicVolume = musicVolume;
        this.masterVolume = masterVolume;
        this.sensibility = sensibility;
    }
}
