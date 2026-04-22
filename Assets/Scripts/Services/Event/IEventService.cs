using System;
using UnityEngine;

public interface IEventService
{
    public void Publish(GameEventBase action);
    public void Subscribe<T>(Action<GameEventBase> action);
    public void Unsubscribe<T>(Action<GameEventBase> action);
}
