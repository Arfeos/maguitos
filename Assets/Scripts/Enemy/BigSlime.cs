using System.Collections;
using UnityEngine;

/// <summary>
/// Variante grande del slime básico que, al morir, genera un número aleatorio de slimes hijos
/// en posiciones válidas alrededor de su cuerpo antes de destruirse.
/// Hereda el movimiento por salto y el ataque por OverlapSphere de <see cref="BasicSlimeController"/>.
/// </summary>
public class BigSlime : BasicSlimeController
{
    // ── Generación de hijos ──────────────────────────────────────────────────
    [Header("ChildCreation")]
    /// <summary>Prefab del slime hijo que se instancia al morir.</summary>
    [SerializeField] private GameObject slimeChild;
    /// <summary>Radio máximo alrededor del BigSlime en el que pueden aparecer los hijos.</summary>
    [SerializeField] private float spawnRadius = 2f;
    /// <summary>Radio de la esfera usada para comprobar si una posición de spawn está libre de otros slimes.</summary>
    [SerializeField] private float checkRadius = 0.5f;

    /// <summary>Capas que se consideran ocupadas al buscar posiciones de spawn válidas.</summary>
    [SerializeField] private LayerMask slimeLayer;

    /// <summary>Número mínimo de hijos que pueden generarse al morir.</summary>
    [SerializeField] private int minNumberOfChilds = 2;
    /// <summary>Número máximo de hijos que pueden generarse al morir.</summary>
    [SerializeField] private int maxNumberOfChilds = 6;


    // ── Estado interno ───────────────────────────────────────────────────────
    /// <summary>Collider propio del BigSlime, usado para ignorar colisiones con los hijos recién creados.</summary>
    private Collider myCollider;

    // ── Override lifecycle ───────────────────────────────────────────────────
    /// <summary>
    /// Obtiene el collider propio además de resolver las dependencias de la base.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        myCollider = GetComponent<Collider>();
    }

    /// <summary>
    /// Sobreescribe la disolución base para añadir la generación de hijos justo antes de destruirse.
    /// El proceso de disolución visual es idéntico al de la base; al completarse instancia entre
    /// <see cref="minNumberOfChilds"/> y <see cref="maxNumberOfChilds"/> slimes hijos.
    /// </summary>
    protected override IEnumerator DissolveAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeStart);

        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        // Cambiar shader en todos los materiales
        foreach (Renderer r in renderers)
        {
            foreach (Material mat in r.materials)
            {
                mat.shader = Shader.Find("Custom/Dissolve");
                mat.SetTexture("_DissolveTex", dissolveTexture);
                mat.SetColor("_DissolveColor", dissolveColor);
            }
        }

        // Animar la disolución
        while (dissolveProgress < 1f)
        {
            dissolveProgress += Time.deltaTime / dissolveTime;

            foreach (Renderer r in renderers)
                foreach (Material mat in r.materials)
                    mat.SetFloat("_DissolveThreshold", dissolveProgress);

            yield return null;
        }

        int numberOfChilds = Random.Range(minNumberOfChilds, maxNumberOfChilds + 1);
        SpawnChilds(numberOfChilds);
        Destroy(gameObject);
    }

    // ── Generación de hijos ──────────────────────────────────────────────────
    /// <summary>
    /// Instancia <paramref name="numberOfChilds"/> slimes hijos en posiciones válidas alrededor del BigSlime.
    /// Cada hijo se desvincula de cualquier jerarquía y se ignora la colisión entre él y el padre.
    /// </summary>
    /// <param name="numberOfChilds">Número de hijos a generar.</param>
    private void SpawnChilds(int numberOfChilds)
    {
        for (int i = 0; i < numberOfChilds; i++)
        {
            Vector3 spawnPosition = GetValidSpawnPosition();

            GameObject child = Instantiate(
                slimeChild,
                spawnPosition,
                Quaternion.identity
            );

            // Asegura que NO sea hijo en la jerarquía de escena
            child.transform.parent = null;

            // Ignorar colisión entre el hijo recién creado y el padre (que aún no se ha destruido)
            Collider childCollider = child.GetComponent<Collider>();
            if (childCollider != null && myCollider != null)
            {
                Physics.IgnoreCollision(myCollider, childCollider);
            }
        }
    }

    /// <summary>
    /// Busca una posición aleatoria dentro de <see cref="spawnRadius"/> que no esté ocupada
    /// por ningún collider de <see cref="slimeLayer"/>. Realiza un máximo de 20 intentos;
    /// si no encuentra ninguna posición libre, devuelve la última generada.
    /// </summary>
    /// <returns>Posición mundial válida (o la mejor encontrada) para instanciar un hijo.</returns>
    private Vector3 GetValidSpawnPosition()
    {
        Vector3 position = transform.position;
        bool validPosition = false;
        int attempts = 0;

        while (!validPosition && attempts < 20)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;

            position = transform.position +
                       new Vector3(randomCircle.x, 0f, randomCircle.y);

            validPosition = !Physics.CheckSphere(position, checkRadius, slimeLayer);

            attempts++;
        }

        return position;
    }
}