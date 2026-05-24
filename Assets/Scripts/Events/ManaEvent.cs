/// <summary>
/// Clase de evento derivada de <see cref="GameEventBase"/> utilizada para notificar cambios relacionados con el maná del personaje dentro del sistema de eventos gestionado por <see cref="EventService"/>. 
/// Permite comunicar actualizaciones del valor de maná a otros sistemas suscritos mediante <see cref="IEventService"/>
/// </summary>
public class ManaEvent : GameEventBase
{
    /// <summary>
    /// Variable pública que almacena la nueva cantidad de maná que deberá ser aplicada o actualizada
    /// </summary>
    public int ManaToChange;    
}
