using System;
using System.Collections.Generic;
/// <summary>
/// Servicio encargado de gestionar un sistema de eventos basado en publicación y suscripción. 
/// Permite registrar, ejecutar y eliminar acciones asociadas a distintos tipos de eventos derivados de <see cref="GameEventBase"/> para facilitar la comunicación entre sistemas desacoplados del proyecto
/// </summary>
public class EventService : IEventService
{
    private Dictionary<Type, List<Action<GameEventBase>>> _events = new Dictionary<Type, List<Action<GameEventBase>>>();
    /// <summary>
    /// Publica un evento y ejecuta todas las acciones registradas para el tipo específico de evento recibido
    /// </summary>
    /// <param name="action">Evento derivado de <see cref="GameEventBase"/> que será enviado a todos los suscriptores registrados</param>
    public void Publish(GameEventBase action)
    {
        Type type = action.GetType();
        if (this._events.ContainsKey(type))
        {
            foreach (var item in this._events[type])
            {
                item.Invoke(action);
            }
        }
    }
    /// <summary>
    /// Registra una acción para que sea ejecutada cuando se publique un evento del tipo indicado
    /// </summary>
    /// <param name="action">Acción que será asociada al evento derivado de <see cref="GameEventBase"/></param>
    /// <typeparam name="T">Tipo de evento que será escuchado</typeparam>
    public void Subscribe<T>(Action<GameEventBase> action)
    {
        Type type = typeof(T);
        if (!this._events.ContainsKey(type))
            this._events[type] = new List<Action<GameEventBase>>();

        this._events[type].Add(action);
    }
    /// <summary>
    /// Elimina una acción previamente registrada de un tipo de evento determinado
    /// </summary>
    /// <param name="action">Acción que dejará de estar asociada al evento</param>
    /// <typeparam name="T">Tipo de evento del que se eliminará la suscripción</typeparam>
    public void Unsubscribe<T>(Action<GameEventBase> action)
    {
        Type type = typeof(T);

        if (this._events.ContainsKey(type))
            this._events[type].Remove(action);
    }
}
