
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneService :ISceneService
{
    private string lastScene;
    public void LoadScene(SceneNames scene)
    {
        lastScene = SceneManager.GetActiveScene().name;
        //he visto el atentado contra natura hecho por mi compañero Sergio y dada la situacion, me veo en la necesidad de utilizarlo, asi que,
        //ahora es nuestra aberracion, una disculpa de antemano.

        CoroutineRunner.Instance.StartCoroutine(LoadSceneRutine(scene.ToString()));
    }
    public void GoBack()
    {
        //lo siento por esto pero estoy cansado jefe
        if (!string.IsNullOrEmpty(lastScene))
        CoroutineRunner.Instance.StartCoroutine(LoadSceneRutine(lastScene));
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
        GameObject prefab = Resources.Load<GameObject>("Prefabs/LoginPanel");
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
