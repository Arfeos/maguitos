using UnityEngine;

/// <summary>
/// Componente de UI que permite cambiar el idioma de la aplicación a través de <see cref="IUIService"/>.
/// Puede recibir el idioma directamente desde el Inspector o como índice entero desde un evento de UI
/// (por ejemplo, un Dropdown).
/// </summary>
public class ChangeLanguage : MonoBehaviour
{
    // ── Configuración ────────────────────────────────────────────────────────
    /// <summary>Idioma al que se cambiará al llamar a <see cref="Change"/>.</summary>
    [SerializeField] private Languages language;

    // ── Servicios ────────────────────────────────────────────────────────────
    /// <summary>Servicio de UI usado para aplicar el cambio de idioma.</summary>
    private IUIService uiService;


    // ── Unity Lifecycle ──────────────────────────────────────────────────────

    /// <summary>
    /// Resuelve los servicios necesarios desde el contenedor de la aplicación.
    /// </summary>
    private void Start()
    {
        uiService = AppContainer.Get<IUIService>();
    }

    // ── Callbacks ────────────────────────────────────────────────────────────

    /// <summary>
    /// Aplica el idioma configurado en <see cref="language"/>.
    /// Llamar desde el evento OnClick de un botón en el Inspector.
    /// </summary>
    public void Change()
    {
        uiService.changeLanguage(language);
    }

    /// <summary>
    /// Convierte un índice entero al enum <see cref="Languages"/> correspondiente y aplica el cambio.
    /// Útil para enlazar directamente con el evento OnValueChanged de un Dropdown.
    /// </summary>
    /// <param name="index">Índice del idioma en el enum <see cref="Languages"/>.</param>
    public void ChangeByInt(int index)
    {
        language = (Languages)index;
        Change();
    }
}