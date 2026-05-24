

using System.Collections;
using System.Linq;
using UnityEngine;
/// <summary>
/// Componente de Unity encargado de gestionar el comportamiento de un hechizo tipo esfera. 
/// Controla las colisiones, detecta enemigos dentro de un área de impacto, aplica daño mediante <see cref="ICharacterService"/> y devuelve el proyectil al sistema de reutilización mediante <see cref="ISpellService"/>
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BallSpellType : MonoBehaviour
{
    [SerializeField] private LayerMask layersToHit;
    private Rigidbody rb;
    private ISpellService _spellService;
    private ICharacterService _characterService;
    private Coroutine corutinaCrecer;

    /// <summary>
    /// Método ejecutado durante la inicialización del objeto. Obtiene referencias a los servicios <see cref="ISpellService"/> y <see cref="ICharacterService"/> mediante <see cref="AppContainer"/> y almacena una referencia al componente <see cref="Rigidbody"/>
    /// </summary>
    void Awake()
    {
        _spellService = AppContainer.Get<ISpellService>();
        _characterService = AppContainer.Get<ICharacterService>();
        rb = GetComponent<Rigidbody>();
    }
    /// <summary>
    /// Método ejecutado automáticamente cuando el objeto colisiona con otro elemento. 
    /// Comprueba si el objeto impactado pertenece a las capas permitidas, calcula el área de explosión, detecta los objetos afectados y aplica daño a aquellos que implementen la interfaz <see cref="IHittable">
    /// </summary>
    /// <param name="collision">Información de la colisión producida, incluyendo contactos y objeto impactado</param>
    private void OnCollisionEnter(Collision collision)
    {
        // 2. Comprobar si la capa está dentro de la LayerMask
        // (1 << objectLayer) crea una máscara con un solo bit activado
        int objectLayer = collision.gameObject.layer;
        if ((layersToHit.value & (1 << objectLayer)) == 0) return;
        

        // SphereCast desde el punto de impacto
        Vector3 impactPoint = collision.contacts[0].point;
        float radius = 3f;      // Radio de la explosión

        Collider[] hits = Physics.OverlapSphere(impactPoint, radius, layersToHit);
        gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        if(corutinaCrecer == null) corutinaCrecer = StartCoroutine(ShowExplosionSphere(impactPoint, radius));
        foreach (Collider hit in hits)
        {
            if (hit.GetComponent<IHittable>() != null) hit.GetComponent<IHittable>().Hit(_characterService.getSpell(_characterService.getIndex()).spell.damage);
        }

        
    }
    /// <summary>
    /// Corrutina encargada de crear una representación visual temporal de la explosión. Genera una esfera, aplica un material obtenido desde <see cref="ICharacterService"/>, realiza una animación de crecimiento progresivo y posteriormente elimina el efecto visual antes de devolver el proyectil mediante <see cref="ISpellService"/>
    /// </summary>
    /// <param name="point">Posición donde se generará la explosión</param>
    /// <param name="radius">Radio del área visual de explosión</param>
    /// <returns>Corrutina utilizada para ejecutar la animación progresiva de la explosión</returns>
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