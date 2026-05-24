using UnityEngine;

/// <summary>
/// Expone un <see cref="ObjectDataScriptable"/> asignado desde el Inspector para que otros
/// componentes puedan consultarlo sin necesidad de buscarlo directamente en el proyecto.
/// </summary>
public class DataShow : MonoBehaviour
{
    // ── Configuración ────────────────────────────────────────────────────────
    /// <summary>Datos del objeto que este componente expone.</summary>
    [SerializeField] private ObjectDataScriptable data;

    // ── API pública ──────────────────────────────────────────────────────────

    /// <summary>
    /// Devuelve el <see cref="ObjectDataScriptable"/> asignado en el Inspector.
    /// </summary>
    /// <returns>Referencia al scriptable object de datos, o <c>null</c> si no está asignado.</returns>
    public ObjectDataScriptable GetData()
    {
        return data;
    }
}