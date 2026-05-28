using System;
using UnityEngine;
/// <summary>
/// Interfaz encargada de definir las operaciones necesarias para gestionar un sistema de eventos basado en publicación y suscripción.
/// Proporciona los métodos necesarios para enviar, registrar y eliminar eventos derivados de <see cref="GameEventBase"/>. 
/// Es implementada por <see cref="EventService"/> para facilitar la comunicación desacoplada entre distintos sistemas del proyecto
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Publica un evento para que sea recibido y procesado por todos los componentes suscritos al tipo correspondiente
    /// </summary>
    /// <param name="action">Evento derivado de <see cref="GameEventBase"/> que será enviado a los suscriptores</param>
    public void Publish(GameEventBase action);
    /// <summary>
    /// Registra una acción para que sea ejecutada cuando se publique un evento de un tipo determinado
    /// </summary>
    /// <param name="action">Acción que será ejecutada cuando ocurra el evento correspondiente</param>
    /// <typeparam name="T">Tipo de evento derivado de <see cref="GameEventBase"/> que será escuchado</typeparam>
    public void Subscribe<T>(Action<GameEventBase> action);
    /// <summary>
    /// Elimina una acción previamente registrada de un tipo específico de evento
    /// </summary>
    /// <param name="action">Acción que dejará de ejecutarse al producirse el evento</param>
    /// <typeparam name="T">Tipo de evento derivado de <see cref="GameEventBase"/> del que se eliminará la suscripción</typeparam>
    public void Unsubscribe<T>(Action<GameEventBase> action);
}
