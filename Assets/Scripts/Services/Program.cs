using UnityEngine;

public static class Program
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Main()
    {
        // Registramos los servicios necesarios

        //    
        AppContainer.Register<IAudioService>(() => new AudioService());
        AppContainer.Register<IEventService>(() => new EventService());
        AppContainer.Register<IHudService>(() => new HudService());
        AppContainer.Register<ICharacterService>(() => new CharacterService());
        AppContainer.Register<IProfileService>(() => new ProfileService());
        AppContainer.Register<ISceneService>(() => new SceneService(Resources.Load<PanelConfigurationScriptable>("Configuration/LoadingConfiguration")));
        AppContainer.Register<IScoreService>(() => new ScoreService());
        AppContainer.Register<IAlertService>(() => new AlertService());
	    AppContainer.Register<IUIService>(() => new UIService());
        AppContainer.Register<IAnimationService>(() => new AnimationService());
        AppContainer.Register<ISpellService>(() => new SpellService(Resources.Load<GameObject>("Prefabs/RayPrefab"), Resources.Load<GameObject>("Prefabs/SpherePrefab")));
        AppContainer.Register<IPauseService>(() => new PauseService(Resources.Load<PanelConfigurationScriptable>("Configuration/PauseConfiguration")));
    }
}
  