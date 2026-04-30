using UnityEngine;

public class TargetController : MonoBehaviour, IHittable
{
    IAnimationService _animationService;
    [SerializeField]AudioClip audioWhenHit;
    public void Hit()
    {
        _animationService.WobbleAnimationWithSound(this.gameObject, audioWhenHit);
    }

    private void Awake()
    {
        _animationService = AppContainer.Get<IAnimationService>();
    }
    
}
