using System;
using UnityEditor;
using UnityEngine;

public class EventReciver : MonoBehaviour
{
    IEventService eventService;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        eventService = AppContainer.Get<IEventService>();
    }

    private void OnEnable()
    {
        eventService.Subscribe<TestEvent>(escribirPorConsola);
    }
    private void OnDisable()
    {
        eventService.Unsubscribe<TestEvent>(escribirPorConsola);
    }

    private void escribirPorConsola(GameEventBase evento)
    {
        TestEvent data= (TestEvent)evento;
        Debug.Log(data.Message);
    }
}
