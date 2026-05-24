
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Servicio encargado de gestionar la carga y navegación entre escenas del juego.
/// Soporta transiciones con efecto de fade y navegación hacia la escena anterior.
/// </summary>
public class SceneService :ISceneService
{
    /// <summary>
    /// Pila que almacena el historial de escenas visitadas para permitir la navegación hacia atrás.
    /// </summary>
    private Stack<string> lastScene= new Stack<string>();

    /// <summary>
    /// Prefab del panel de carga que se muestra durante las transiciones entre escenas.
    /// </summary>
    GameObject prefab;

    /// <summary>
    /// Carga la escena indicada, guardando la escena actual en el historial para poder volver a ella.
    /// </summary>
    /// <param name="scene">Nombre de la escena destino definido en el enum <see cref="SceneNames"/>.</param>
    public void LoadScene(SceneNames scene)
    {

        lastScene.Push(SceneManager.GetActiveScene().name) ;
        //he visto el atentado contra natura hecho por mi compañero Sergio y dada la situacion, me veo en la necesidad de utilizarlo, asi que,
        //ahora es nuestra aberracion, una disculpa de antemano.

        CoroutineRunner.Instance.StartCoroutine(LoadSceneRutine(scene.ToString()));
    }

    /// <summary>
    /// Inicializa el servicio de escenas con la configuración del panel de carga.
    /// </summary>
    /// <param name="so">Scriptable Object que contiene el prefab del panel de transición.</param>
    public SceneService(PanelConfigurationScriptable so)
    {
        this.prefab = so.Panel;
    }

    /// <summary>
    /// Navega a la última escena visitada, extrayéndola del historial.
    /// Si no hay escenas en el historial, no realiza ninguna acción.
    /// </summary>
    public void GoBack()
    {
        //lo siento por esto pero estoy cansado jefe
        if (lastScene.Count!=0)
        CoroutineRunner.Instance.StartCoroutine(LoadSceneRutine(lastScene.Pop().ToString()));
    }

    /// <summary>
    /// Corrutina que gestiona la carga asíncrona de una escena con transición de fade de entrada y salida.
    /// Crea un canvas temporal con el panel de carga, realiza el fade in, espera a que la escena
    /// cargue, activa la escena y finalmente hace fade out antes de destruir el canvas.
    /// </summary>
    /// <param name="sceneName">Nombre de la escena a cargar como string.</param>
    private IEnumerator LoadSceneRutine(string sceneName)
    {
        GameObject canvasObj = new GameObject("LoadingCanvas");
        Object.DontDestroyOnLoad(canvasObj);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();
        
        GameObject loadingScreen = Object.Instantiate(prefab, canvas.transform);
        CanvasGroup canvasGroup = loadingScreen.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = loadingScreen.AddComponent<CanvasGroup>();
        yield return Fade(canvasGroup, 0, 1, 0.5f);
        AsyncOperation operation =SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        while (operation.progress < 0.9f)
            yield return null;
        operation.allowSceneActivation = true;
        yield return null;
        yield return Fade(canvasGroup, 1, 0, 0.5f);
        Object.Destroy(canvasObj);
    }

    /// <summary>
    /// Corrutina que realiza una transición de alpha (fade) sobre un <see cref="CanvasGroup"/>.
    /// </summary>
    /// <param name="canvasGroup">El <see cref="CanvasGroup"/> cuyo alpha se va a interpolar.</param>
    /// <param name="start">Valor de alpha inicial (0 = transparente, 1 = opaco).</param>
    /// <param name="end">Valor de alpha final.</param>
    /// <param name="duration">Duración de la transición en segundos.</param>
    private IEnumerator Fade(CanvasGroup canvasGroup, float start, float end, float duration) {
        float time = 0;

        while (time < duration+0.2f)
        {
            time += Time.deltaTime;

            float t = time / duration;

            canvasGroup.alpha = Mathf.Lerp(start, end, t);

            yield return null;
        }

        canvasGroup.alpha = end;
    }

}
