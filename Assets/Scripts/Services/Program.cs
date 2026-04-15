using UnityEngine;

public static class Program
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Main()
    {
        // Registramos los servicios necesarios

        // LogService se encarga de gestionar todos los logs de la aplicación
        AppContainer.Register<IAudioService>(() => new AudioService());


    }
}
