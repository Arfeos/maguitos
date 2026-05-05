using UnityEngine;

public interface IAnimationService
{
    public void WobbleAnimation(GameObject objectToMove, float AnguloDeRebote = 20f, float DuracionDeRebote = 0.8f, int CantidadDeRebote = 4);
    public void WobbleAnimationWithSound(GameObject objectToMove, AudioClip _audioClip, float AnguloDeRebote = 20f, float DuracionDeRebote = 0.8f, int CantidadDeRebote = 4);
}
