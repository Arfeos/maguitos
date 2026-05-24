/// <summary>
/// Clase de evento derivada de <see cref="GameEventBase"/> utilizada para notificar cambios relacionados con la selección o cambio de hechizos dentro del sistema. 
/// Permite comunicar a componentes suscritos mediante <see cref="IEventService"/> que el jugador ha solicitado cambiar el hechizo actualmente seleccionado
/// </summary>
public class SpellChangeEvent : GameEventBase
{
    /// <summary>
    /// Variable pública que almacena la dirección o cantidad de desplazamiento que se aplicará sobre la selección de hechizos
    /// </summary>
    public int cambio;
}
