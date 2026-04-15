using UnityEngine;

public interface IEventService
{
    public void Publish();
    public void Subscribe();
    public void UnSubscribe();
}
