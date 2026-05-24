using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Servicio encargado de gestionar distintas animaciones y efectos visuales mediante corrutinas. Puede interactuar con el servicio <see cref="IAudioService"/> para reproducir sonidos durante determinadas animaciones.
/// </summary>
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
    /// Inicia una animación de rebote sobre un objeto y reproduce un sonido mediante <see cref="IAudioService"/>
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
    /// <summary>
    /// Inicia una animación de rebote sobre un objeto sin reproducir sonido
    /// </summary>
    /// <param name="objectToMove">Objeto sobre el que se aplicará la animación</param>
    /// <param name="AnguloDeRebote">Ángulo máximo de rotación durante el rebote. Valor por defecto: 20f</param>
    /// <param name="DuracionDeRebote">Duración total del efecto</param>
    /// <param name="CantidadDeRebote">Número de oscilaciones</param>
    public void WobbleAnimation(GameObject objectToMove,  float AnguloDeRebote = 20f, float DuracionDeRebote = 0.8f, int CantidadDeRebote = 4)
    {
        if (_wobbleCoroutine != null)
            CoroutineRunner.Instance.StopCoroutine(_wobbleCoroutine);

        _originalScale = objectToMove.transform.localScale;
        _originalRotation = objectToMove.transform.localRotation;
        _wobbleCoroutine = CoroutineRunner.Instance.StartCoroutine(WobbleRoutine(objectToMove, AnguloDeRebote, DuracionDeRebote, CantidadDeRebote));
    }
    /// <summary>
    /// Corrutina encargada de calcular y aplicar el movimiento oscilatorio mediante una función seno para generar el efecto de rebote
    /// </summary>
    /// <param name="objectToMove">Objeto que recibirá la animación</param>
    /// <param name="AnguloDeRebote">Ángulo máximo de rotación</param>
    /// <param name="DuracionDeRebote">Duración total del efecto</param>
    /// <param name="CantidadDeRebote">Número de oscilaciones</param>
    /// <returns></returns>
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

    /// <summary>
    /// Inicia una animación de aparición progresiva (Fade In) sobre un elemento de interfaz
    /// </summary>
    /// <param name="target">Elemento de UI que recibirá la animación</param>
    /// <param name="duration">Duración de la transición. Valor por defecto: 1f</param>
    public void FadeInUIAnimation(GameObject target, float duration = 1f)
    {
        if (_FadeInCoroutine != null)
            CoroutineRunner.Instance.StopCoroutine(_FadeInCoroutine);
        _FadeInCoroutine = CoroutineRunner.Instance.StartCoroutine(FadeIn(target, duration));
    }
    /// <summary>
    /// Corrutina encargada de aumentar progresivamente la transparencia de un objeto hasta hacerlo completamente visible
    /// </summary>
    /// <param name="target">Elemento de UI a mostrar</param>
    /// <param name="duration">Tiempo de duración de la animación</param>
    /// <returns></returns>
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
    /// <summary>
    /// Inicia una animación de desaparición progresiva (Fade Out) sobre un elemento de interfaz
    /// </summary>
    /// <param name="target">Elemento de UI que recibirá la animación</param>
    /// <param name="duration">Duración de la transición. Valor por defecto: 1f</param>
    public void FadeOutUIAnimation(GameObject target, float duration = 1f)
    {
        if (_FadeOutCoroutine != null)
            CoroutineRunner.Instance.StopCoroutine(_FadeOutCoroutine);

        _FadeOutCoroutine = CoroutineRunner.Instance.StartCoroutine(FadeOutUI(target, duration));
    }
    /// <summary>
    /// Corrutina encargada de disminuir progresivamente la transparencia de un objeto hasta hacerlo invisible y desactivarlo
    /// </summary>
    /// <param name="target">Elemento de UI que desaparecerá</param>
    /// <param name="duration">Tiempo total de la animación</param>
    /// <returns></returns>
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