using UnityEngine;

public class BaseButton : MonoBehaviour
{
    protected IAudioService _audioService;
    [SerializeField]protected AudioClip _audioClip;
    protected virtual void Awake()
    {
        _audioService = AppContainer.Get<IAudioService>();
    }
    public virtual void PlaySound()
    {
        _audioService.PlaySound(_audioClip);
    }
}
