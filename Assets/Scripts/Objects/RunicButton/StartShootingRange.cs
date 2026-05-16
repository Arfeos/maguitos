using UnityEngine;

public class StartShootingRange : MonoBehaviour, IHittable
{
    private IEventService _eventService;
    public void Hit(float damage)
    {
        TutorialGameEvent startTutorial = new TutorialGameEvent();
        _eventService.Publish(startTutorial);
    }
    private void Awake()
    {
        _eventService = AppContainer.Get<IEventService>();
    }
}
