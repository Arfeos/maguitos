using UnityEngine;

public class TargetController : MonoBehaviour, IHittable
{
    IAnimationService _animationService;
    [SerializeField]AudioClip audioWhenHit;
    [SerializeField] int minVelocity;
    [SerializeField] int maxVelocity;
    [SerializeField] int puntos;
    private int velocity;
    private int direction = 1;
    private Vector3 InitialPosition;
    public void Hit()
    {
        _animationService.WobbleAnimationWithSound(this.gameObject, audioWhenHit);
        Debug.Log("Has ganado " + puntos + " puntos");
    }
    private void Awake()
    {
        _animationService = AppContainer.Get<IAnimationService>();
        velocity = Random.Range(minVelocity, maxVelocity);
        InitialPosition = this.transform.position;
    }

    private void Update()
    {
        float step = velocity * direction * Time.deltaTime;

        // Lanza un ray en la dirección de movimiento
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.right * direction, out hit, Mathf.Abs(step) + 0.5f))
        {
            if (hit.collider.CompareTag("BounceZone"))
            {
                direction *= -1;
            }
        }

        transform.position += step * Vector3.right;
    }
}
