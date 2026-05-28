using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// MonoBehaviour que inicializa el estado de la UI al activarse,
/// cambiando el mapa de controles a UI y registrando el primer botón seleccionable
/// para la navegación con mando o teclado.
/// </summary>

public class UIInitializer : MonoBehaviour
{
    /// <summary>
    /// Referencia al servicio de UI usado para registrar el primer elemento seleccionable.
    /// </summary>
    private IUIService uiService;

    /// <summary>
    /// Primer botón o elemento seleccionable que recibirá el foco al activarse la UI.
    /// </summary>
    [SerializeField] GameObject firstButton;

    /// <summary>
    /// Al activarse el componente, cambia el mapa de controles a <see cref="PlayerInputManager.ControlMap.UI"/>
    /// y registra el <see cref="firstButton"/> como elemento seleccionado inicial en el <see cref="IUIService"/>.
    /// </summary>
    /// <remarks>
    /// TODO: añadir comprobación para evitar cambiar el mapa de controles si ya está en UI.
    /// </remarks>
    void OnEnable()
    { 
        //TODO: crear un comprobante para ver si el control ya esta en UI
        PlayerInputManager.SwitchControlMap(PlayerInputManager.ControlMap.UI);
        uiService = AppContainer.Get<IUIService>();
        uiService.RegisterFirstButton(firstButton);
    }

}
