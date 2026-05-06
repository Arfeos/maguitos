

using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallSpellType : MonoBehaviour
{
    [SerializeField] private LayerMask layersToHit;
    private Rigidbody rb;
    private ISpellService _spellService;
    private ICharacterService _characterService;
    private Coroutine corutinaCrecer;


    void Awake()
    {
        _spellService = AppContainer.Get<ISpellService>();
        _characterService = AppContainer.Get<ICharacterService>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 2. Comprobar si la capa está dentro de la LayerMask
        // (1 << objectLayer) crea una máscara con un solo bit activado
        int objectLayer = collision.gameObject.layer;
        if ((layersToHit.value & (1 << objectLayer)) == 0) return;
        //TODO hacer la explosion
        Debug.Log("PUM");

        // SphereCast desde el punto de impacto
        Vector3 impactPoint = collision.contacts[0].point;
        float radius = 3f;      // Radio de la explosión

        Collider[] hits = Physics.OverlapSphere(impactPoint, radius, layersToHit);
        gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        if(corutinaCrecer == null) corutinaCrecer = StartCoroutine(ShowExplosionSphere(impactPoint, radius));
        foreach (Collider hit in hits)
        {
            Debug.Log($"Impactado: {hit.gameObject.name}");
            // Aquí aplica daño, knockback, etc.
        }

        
    }

    private IEnumerator ShowExplosionSphere(Vector3 point, float radius)
    {
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(visual.GetComponent<Collider>());
        visual.transform.position = point;
        visual.transform.localScale = Vector3.zero;

        // Copia el material para no modificar el original
        Material originalMat = _characterService.getSpell(_characterService.getIndex()).spell.RayMaterial.First();
        Material mat = new Material(originalMat);
        visual.GetComponent<MeshRenderer>().material = mat;

        float duration = 0.2f;
        float elapsed = 0f;
        float targetScale = radius * 1.1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            visual.transform.localScale = Vector3.one * Mathf.Lerp(0f, targetScale, t);
            yield return null;
        }

        Destroy(visual);
        Destroy(mat); // Limpia el material instanciado
        corutinaCrecer = null;
        _spellService.ReturnBall(gameObject);

    }

}