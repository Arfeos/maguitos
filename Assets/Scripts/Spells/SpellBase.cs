using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SpellBase : MonoBehaviour
{
#nullable enable
    enum SpellType { 
    ray,
    ball,
    buff,
    structure}
    enum castType{
        auto,
        semi
    }
    public int AmmoSpace;
    public LineRenderer lineRenderer;
    [SerializeField] protected float Velocity  = 1f;
    [SerializeField] protected float LifeTime = 1f;
    [SerializeField] protected float ShootDelay;
    public bool ProducesLine { get; set; } = false;

    public GameObject? SpawnPrefab { get; }
    public GameObject? ProducedParticle { get; }
    public GameObject? hitParticle { get; }

    private void Awake()
    {

    }

    public void createLine(Vector3 posicionInicio, Ray ray, RaycastHit hit)
    {
        lineRenderer.SetPosition(0, posicionInicio);
        if (hit.point != null)
        {
            // Si choca, el fin es el punto de impacto
            lineRenderer.SetPosition(1, new Vector3(10,10,10));
        }
        else
        {
            // Si no choca, el fin es la distancia máxima
            lineRenderer.SetPosition(1, ray.origin + ray.direction * LifeTime);
        }
    }
    public virtual void LanzarHechizo() {
        Debug.Log("Line");
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; // Solo inicio y fin
    }
}
