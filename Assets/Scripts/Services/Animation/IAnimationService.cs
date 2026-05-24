using UnityEngine;

/// <summary>
/// Define el contrato para animaciones de UI reutilizables: wobble y fade.
/// </summary>
public interface IAnimationService
{
    /// <summary>
    /// Inicia una animación de rebote sobre un objeto y reproduce un sonido mediante <see cref="IAudioService"/>
    /// </summary>
    /// <param name="objectToMove"> Objeto que quieres que se realice la animacion</param>
    /// <param name="AnguloDeRebote"> Angulo de giro</param>
    /// <param name="DuracionDeRebote"> Cuanto se tira rebotando </param>
    /// <param name="CantidadDeRebote"> Cuantos rebotes pega </param>
    /// <returns></returns>
    public void WobbleAnimation(GameObject objectToMove, float AnguloDeRebote = 20f, float DuracionDeRebote = 0.8f, int CantidadDeRebote = 4);

    /// <summary>
    /// Inicia una animación de rebote sobre un objeto sin reproducir sonido
    /// </summary>
    /// <param name="objectToMove">Objeto sobre el que se aplicará la animación</param>
    /// <param name="AnguloDeRebote">Ángulo máximo de rotación durante el rebote. Valor por defecto: 20f</param>
    /// <param name="DuracionDeRebote">Duración total del efecto</param>
    /// <param name="CantidadDeRebote">Número de oscilaciones</param>
    public void WobbleAnimationWithSound(GameObject objectToMove, AudioClip _audioClip, float AnguloDeRebote = 20f, float DuracionDeRebote = 0.8f, int CantidadDeRebote = 4);

    /// <summary>
    /// Inicia una animación de desaparición progresiva (Fade Out) sobre un elemento de interfaz
    /// </summary>
    /// <param name="target">Elemento de UI que recibirá la animación</param>
    /// <param name="duration">Duración de la transición. Valor por defecto: 1f</param>
    public void FadeOutUIAnimation(GameObject target, float duration = 1f);

    /// <summary>
    /// Inicia una animación de aparición progresiva (Fade In) sobre un elemento de interfaz
    /// </summary>
    /// <param name="target">Elemento de UI que recibirá la animación</param>
    /// <param name="duration">Duración de la transición. Valor por defecto: 1f</param>
    public void FadeInUIAnimation(GameObject target, float duration = 1f);
}