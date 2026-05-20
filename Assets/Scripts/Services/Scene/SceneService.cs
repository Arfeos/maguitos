using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneService :ISceneService
{
    private Stack<string> lastScene = new Stack<string>();
    GameObject prefab;
    public void LoadScene(SceneNames scene)
    {
        lastScene.Push(SceneManager.GetActiveScene().name);
        //he visto el atentado contra natura hecho por mi compañero Sergio y dada la situacion, me veo en la necesidad de utilizarlo, asi que,
        //ahora es nuestra aberracion, una disculpa de antemano.

        CoroutineRunner.Instance.StartCoroutine(LoadSceneRutine(scene.ToString()));
    }
    public SceneService(PanelConfigurationScriptable so)
    {
        this.prefab = so.Panel;
    }   
    public void GoBack()
    {
        //lo siento por esto pero estoy cansado jefe
        if (lastScene.Count != 0)
            CoroutineRunner.Instance.StartCoroutine(LoadSceneRutine(lastScene.Pop().ToString()));
    }
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

        if (NetworkManager.Singleton != null)
        {
            if (NetworkManager.Singleton.IsListening)
            {
                Debug.Log("entra aquí");
                NetworkManager.Singleton.SceneManager.LoadScene(
                    sceneName,
                    LoadSceneMode.Single
                );
            }
        }
        else
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;
            while (operation.progress < 0.9f)
                yield return null;
            operation.allowSceneActivation = true;
        }
        
        
        yield return null;
        yield return Fade(canvasGroup, 1, 0, 0.5f);
        Object.Destroy(canvasObj);
    }


    private IEnumerator Fade(CanvasGroup canvasGroup, float start, float end, float duration)
    {
        float time = 0;
        while (time < duration + 0.2f)
        {
            time += Time.deltaTime;
            float t = time / duration;
            canvasGroup.alpha = Mathf.Lerp(start, end, t);
            yield return null;
        }
        canvasGroup.alpha = end;
    }
}
