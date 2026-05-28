using UnityEngine;

/// <summary>
/// Define el contrato para mostrar y ocultar mensajes de alerta en la UI.
/// </summary>
public interface IAlertService
{
    /// <summary>
    /// Muestra los datos del objeto que se le pasen por pantalla en el Panel adecuado seleccionado
    /// </summary>
    /// <param name="MessageBox">Un PausepanelPrefab, con 2 al menos 2 TextMeshProUGUI sobre el que se va a escribir </param>
    /// <param name="message">Data de un scriptable object del tipo ObjectDataScriptable</param>
    public void ShowAlertMessage(GameObject MessageBox, ObjectDataScriptable message);


    /// <summary>
    /// Oculta el Panel seleccionado
    /// </summary>
    /// <param name="MessageBox">Un PausepanelPrefab, con 2 al menos 2 TextMeshProUGUI sobre el que se va a escribir </param>
    public void HideAlertMessage(GameObject MessageBox);
}