using UnityEngine;

public static class Program
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Main()
    {
        // Registramos los servicios necesarios

        //    
        AppContainer.Register<IAudioService>(() => new AudioService());
        AppContainer.Register<ICoconutService>(() => new CoconutService());
        AppContainer.Register<IEventService>(() => new EventService());
        AppContainer.Register<IHudService>(() => new HudService());
        AppContainer.Register<ICharacterService>(() => new CharacterService());
        AppContainer.Register<IProfileService>(() => new ProfileService());
        AppContainer.Register<ISceneService>(() => new SceneService());
        AppContainer.Register<IScoreService>(() => new ScoreService());
        AppContainer.Register<IAlertService>(() => new AlertService());
        AppContainer.Register<ISpellService>(() => new SpellService(Resources.Load<GameObject>("Prefabs/RayPrefab")));
    }
}
  