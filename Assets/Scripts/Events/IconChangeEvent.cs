/// <summary>
/// Clase de evento derivada de <see cref="GameEventBase"/> utilizada para notificar cambios relacionados con un icono dentro del sistema. Almacena la información necesaria para actualizar la imagen o recurso gráfico asociado
/// </summary>
public class IconChangeEvent : GameEventBase
{
    /// <summary>
    /// Variable pública que almacena la dirección o ruta del nuevo icono asociado al evento
    /// </summary>
    public string newIconUrl;
    /// <summary>
    /// Constructor encargado de crear una nueva instancia del evento asignando la dirección o ruta del nuevo icono
    /// </summary>
    /// <param name="newIconUrl">Ruta, URL o identificador del nuevo icono que será utilizado para reemplazar el actual</param>
    public IconChangeEvent(string newIconUrl)
    {
        this.newIconUrl = newIconUrl;
    }
}
