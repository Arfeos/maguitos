using UnityEngine;

public static class Program
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Main()
    {
        // Registramos los servicios necesarios

        //    
        AppContainer.Register<IAudioService>(() => new AudioService());


    }
}
  