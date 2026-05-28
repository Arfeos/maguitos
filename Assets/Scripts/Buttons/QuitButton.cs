using UnityEngine;


/// <summary>
/// Botón que cierra la aplicación al ser pulsado.
/// Hereda la reproducción de sonido de <see cref="BaseButton"/>.
/// </summary>
public class QuitButton : BaseButton
{
    /// <summary>
    /// Cierra la aplicación. Llamar desde el evento OnClick del botón en el Inspector.
    /// En el editor no tiene efecto visible; usa <c>Application.Quit()</c> solo en builds.
    /// </summary>
    public void Quit()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}