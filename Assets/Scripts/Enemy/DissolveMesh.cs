using System.Collections;
using UnityEngine;

/// <summary>
/// Componente standalone que aplica el shader Custom/Dissolve al <see cref="Renderer"/> del objeto
/// y lo destruye al terminar la animación. Versión simplificada de la disolución de <see cref="SlimeBase"/>
/// para objetos que no son slimes (props, decorados, etc.).
/// </summary>
[RequireComponent(typeof(Renderer))]
public class DissolveMesh : MonoBehaviour
{
    // ── Configuración ────────────────────────────────────────────────────────
    /// <summary>Textura de ruido usada por el shader para controlar el patrón de disolución.</summary>
    public Texture2D dissolveTexture;

    /// <summary>Color del borde que aparece durante la disolución.</summary>
    public Color dissolveColor = Color.red;

    /// <summary>Segundos que se esperan antes de iniciar la animación de disolución.</summary>
    public float delayBeforeStart = 3f;

    /// <summary>Duración en segundos de la animación de disolución completa.</summary>
    public float dissolveTime = 2f;

    // ── Estado interno ───────────────────────────────────────────────────────
    /// <summary>Instancia del material del renderer, modificada en tiempo de ejecución para aplicar el shader.</summary>
    private Material material;

    /// <summary>Progreso actual de la disolución, de 0 (sin disolver) a 1 (completamente disuelto).</summary>
    private float dissolveProgress = 0.3f;

    // ── Unity Lifecycle ──────────────────────────────────────────────────────

    /// <summary>
    /// Inicia la corrutina de disolución al activarse el componente.
    /// </summary>
    private void Start()
    {
        StartCoroutine(DissolveAfterDelay());
    }

    // ── Disolución ───────────────────────────────────────────────────────────

    /// <summary>
    /// Espera <see cref="delayBeforeStart"/> segundos, aplica el shader Custom/Dissolve al material
    /// del renderer y anima el progreso hasta destruir el objeto.
    /// </summary>
    private IEnumerator DissolveAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeStart);

        material = GetComponent<Renderer>().material;
        material.shader = Shader.Find("Custom/Dissolve");
        material.SetTexture("_DissolveTex", dissolveTexture);
        material.SetColor("_DissolveColor", dissolveColor);

        while (dissolveProgress < 1f)
        {
            dissolveProgress += Time.deltaTime / dissolveTime;
            material.SetFloat("_DissolveThreshold", dissolveProgress);
            yield return null;
        }

        Destroy(gameObject);
    }
}