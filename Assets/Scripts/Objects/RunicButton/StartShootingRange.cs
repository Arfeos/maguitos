using UnityEngine;

public class StartShootingRange : MonoBehaviour, IHittable
{
    private IEventService _eventService;
    public void Hit()
    {
        TutorialGameEvent startTutorial = new TutorialGameEvent();
        _eventService.Publish(startTutorial);
    }
    private void Awake()
    {
        _eventService = AppContainer.Get<IEventService>();
    }
}
