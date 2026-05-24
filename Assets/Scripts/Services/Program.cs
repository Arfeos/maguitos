using UnityEngine;
/// <summary>
/// Clase estática encargada de inicializar y registrar todos los servicios de la aplicación antes de cargar cualquier escena utilizando <see cref="AppContainer"/>.
/// </summary>
public static class Program
{
    /// <summary>
    /// Método estático ejecutado automáticamente antes de cargar la primera escena gracias al atributo RuntimeInitializeOnLoadMethod. 
    ///Registra en <see cref="AppContainer"/> los distintos servicios necesarios para el funcionamiento del juego, incluyendo <see cref="AudioService"/>, 
    ///<see cref="EventService"/>, <see cref="HudService"/>, <see cref="CharacterService"/>, <see cref="ProfileService"/>, <see cref="SceneService"/>, <see cref="ScoreService"/>, 
    ///<see cref="AlertService"/>, <see cref="UIService"/>, <see cref="AnimationService"/>, <see cref="SpellService"/> y <see cref="PauseService"/>.
    /// </summary>
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
        AppContainer.Register<IPauseService>(() => new PauseService(Resources.Load<PanelConfigurationScriptable>("Configuration/PauseConfiguration"), Resources.Load<PanelConfigurationScriptable>("Configuration/SettingConfiguration")));
    }
}
  