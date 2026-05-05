using System;
using System.Collections.Generic;

public class EventService : IEventService
{
    private Dictionary<Type, List<Action<GameEventBase>>> _events = new Dictionary<Type, List<Action<GameEventBase>>>();

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

    public void Subscribe<T>(Action<GameEventBase> action)
    {
        Type type = typeof(T);
        if (!this._events.ContainsKey(type))
            this._events[type] = new List<Action<GameEventBase>>();

        this._events[type].Add(action);
    }

    public void Unsubscribe<T>(Action<GameEventBase> action)
    {
        Type type = typeof(T);

        if (this._events.ContainsKey(type))
            this._events[type].Remove(action);
    }
}
