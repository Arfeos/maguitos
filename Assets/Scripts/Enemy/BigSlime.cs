using System.Collections;
using UnityEngine;

public class BigSlime : BasicSlimeController
{
    [Header("ChildCreation")]
    [SerializeField] private GameObject slimeChild;

    [SerializeField] private float spawnRadius = 2f;
    [SerializeField] private float checkRadius = 0.5f;

    [SerializeField] private LayerMask slimeLayer;

    [SerializeField] private int minNumberOfChilds = 2;
    [SerializeField] private int maxNumberOfChilds = 6; 

    
    private Collider myCollider;

    protected override void Awake()
    {
        base.Awake();
        myCollider = GetComponent<Collider>();
    }

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

            // Asegura que NO sea hijo
            child.transform.parent = null;

            // Ignorar colisión con el padre
            Collider childCollider = child.GetComponent<Collider>();

            if (childCollider != null && myCollider != null)
            {
                Physics.IgnoreCollision(myCollider, childCollider);
            }
        }
    }

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

            validPosition = !Physics.CheckSphere(
                position,
                checkRadius,
                slimeLayer
            );

            attempts++;
        }

        return position;
    }
}