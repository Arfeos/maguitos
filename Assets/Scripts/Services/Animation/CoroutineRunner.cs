using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;
    //Esto es una puñetera solo para que las clases que no debiesen tirar corrutinas puedan hacerlo
    //Es esto correcto, no lo se, pero a esta hora de la noche solo Dios puede juzgarme
    //y por lo tanto mi creación contra natura se queda y es feliz viviendo como una clase apartada de los ojos de Dios
    public static CoroutineRunner Instance
    {
        get
        {
            if (_instance != null) return _instance;
            var go = new GameObject("CoroutineRunner");
            _instance = go.AddComponent<CoroutineRunner>();
            Object.DontDestroyOnLoad(go);
            return _instance;
        }
    }
}

