using UnityEngine;
using System.Collections;

public class AnimationService : IAnimationService
{
    private Coroutine _wobbleCoroutine;
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
        Vector3 rotationAxis = Vector3.right;

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
}