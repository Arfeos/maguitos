using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SpellBase : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public float Velocity { get; } = 1f;
    public float LifeTime { get; } = 1f;
    public bool ProducesLine { get; set; } = false;
    public GameObject ProducedParticle { get; }

    private void Start()
    {
        Debug.Log("Line");
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; // Solo inicio y fin
    }

    public void createLine(Vector3 posicionInicio, Ray ray, RaycastHit hit)
    {
        lineRenderer.SetPosition(0, posicionInicio);
        if (hit.point != null)
        {
            // Si choca, el fin es el punto de impacto
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            // Si no choca, el fin es la distancia máxima
            lineRenderer.SetPosition(1, ray.origin + ray.direction * LifeTime);
        }
    }
}
