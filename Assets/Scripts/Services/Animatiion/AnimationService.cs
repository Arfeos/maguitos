using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationService : IAnimationService
{
    private Coroutine _wobbleCoroutine;
    private Coroutine _FadeOutCoroutine;
    private Coroutine _FadeInCoroutine;
    private Coroutine _RotateCoroutine;


    private Dictionary<GameObject, Coroutine> _RotateCoroutineList = new Dictionary<GameObject, Coroutine>();
    private Vector3 _originalScale;
    private Quaternion _originalRotation;
    private IAudioService _audioService;
    /// <summary>
    /// Hace Rebotar un objeto como si tuviese un muelle, boing
    /// </summary>
    /// <param name="objectToMove"> Objeto que quieres que se realice la animacion</param>
    /// <param name="AnguloDeRebote"> Angulo de giro</param>
    /// <param name="DuracionDeRebote"> Cuanto se tira rebotando </param>
    /// <param name="CantidadDeRebote"> Cuantos rebotes pega </param>
    /// <returns></returns>
    public void WobbleAnimationWithSound(GameObject objectToMove, AudioClip _audioClip, float AnguloDeRebote = 20f , float DuracionDeRebote = 0.8f, int CantidadDeRebote = 4)
    {
        if (_wobbleCoroutine != null)
            CoroutineRunner.Instance.StopCoroutine(_wobbleCoroutine);

        if(_audioService == null) _audioService = AppContainer.Get<IAudioService>();

        _originalScale = objectToMove.transform.localScale;
        _originalRotation = objectToMove.transform.localRotation;
        _audioService.PlaySound(_audioClip);
        _wobbleCoroutine = CoroutineRunner.Instance.StartCoroutine(WobbleRoutine(objectToMove, AnguloDeRebote, DuracionDeRebote, CantidadDeRebote));
    }
    public void WobbleAnimation(GameObject objectToMove,  float AnguloDeRebote = 20f, float DuracionDeRebote = 0.8f, int CantidadDeRebote = 4)
    {
        if (_wobbleCoroutine != null)
            CoroutineRunner.Instance.StopCoroutine(_wobbleCoroutine);

        _originalScale = objectToMove.transform.localScale;
        _originalRotation = objectToMove.transform.localRotation;
        _wobbleCoroutine = CoroutineRunner.Instance.StartCoroutine(WobbleRoutine(objectToMove, AnguloDeRebote, DuracionDeRebote, CantidadDeRebote));
    }
    private IEnumerator WobbleRoutine(GameObject objectToMove, float AnguloDeRebote, float DuracionDeRebote, int CantidadDeRebote)
    {
        Vector3 rotationAxis = Vector3.forward;

        float elapsed = 0f;
        while (elapsed < DuracionDeRebote)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / DuracionDeRebote;
            float damping = 1f - t;
            float oscillation = Mathf.Sin(t * CantidadDeRebote * Mathf.PI * 2f);
            float angle = AnguloDeRebote * oscillation * damping;

            objectToMove.transform.localRotation =
                _originalRotation * Quaternion.AngleAxis(angle, rotationAxis);

            yield return null;
        }

        objectToMove.transform.localRotation = _originalRotation;
        objectToMove.transform.localScale = _originalScale;
        _wobbleCoroutine = null;
    }


    public void FadeInUIAnimation(GameObject target, float duration = 1f)
    {
        if (_FadeInCoroutine != null)
            CoroutineRunner.Instance.StopCoroutine(_FadeInCoroutine);
        _FadeInCoroutine = CoroutineRunner.Instance.StartCoroutine(FadeIn(target, duration));
    }

    private IEnumerator FadeIn(GameObject target, float duration = 1f)
    {
        target.SetActive(true); // Activa antes de empezar

        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }
        else
        {
            UnityEngine.UI.Graphic[] graphics = target.GetComponentsInChildren<UnityEngine.UI.Graphic>();

            // Empieza en alpha 0
            foreach (var graphic in graphics)
            {
                Color c = graphic.color;
                graphic.color = new Color(c.r, c.g, c.b, 0f);
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
                foreach (var graphic in graphics)
                {
                    Color c = graphic.color;
                    graphic.color = new Color(c.r, c.g, c.b, alpha);
                }
                yield return null;
            }
        }
    }

    public void FadeOutUIAnimation(GameObject target, float duration = 1f)
    {
        if (_FadeOutCoroutine != null)
            CoroutineRunner.Instance.StopCoroutine(_FadeOutCoroutine);

        _FadeOutCoroutine = CoroutineRunner.Instance.StartCoroutine(FadeOutUI(target, duration));
    }
    private IEnumerator FadeOutUI(GameObject target, float duration = 1f)
    {
        // Intenta con CanvasGroup (más eficiente para UI)
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            // Fade con CanvasGroup
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                yield return null;
            }
            canvasGroup.alpha = 0f;
        }
        else
        {
            // Fade modificando el color de cada Graphic (Image, Text, etc.)
            UnityEngine.UI.Graphic[] graphics = target.GetComponentsInChildren<UnityEngine.UI.Graphic>();
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                foreach (var graphic in graphics)
                {
                    Color c = graphic.color;
                    graphic.color = new Color(c.r, c.g, c.b, alpha);
                }
                yield return null;
            }
        }

        target.SetActive(false);
    }
}