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

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
    }

    private void OnDestroy()
    {
        int numberOfChilds = Random.Range(minNumberOfChilds, maxNumberOfChilds+1);
        SpawnChilds(numberOfChilds);
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