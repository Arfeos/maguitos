using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class AlertService : MonoBehaviour, IAlertService
{
    /// <summary>
    /// Muestra los datos del objeto que se le pasen por pantalla en el Panel adecuado seleccionado
    /// </summary>
    /// <param name="MessageBox">Un panelPrefab, con 2 al menos 2 TextMeshProUGUI sobre el que se va a escribir </param>
    /// <param name="message">Data de un scriptable object del tipo ObjectDataScriptable</param>
    public void ShowAlertMessage(GameObject MessageBox, ObjectDataScriptable message)
    {
        if(MessageBox.gameObject == null) return;
        TextMeshProUGUI[] todosLosTextos = MessageBox.GetComponentsInChildren<TextMeshProUGUI>(true);
        if (todosLosTextos != null)
        {
            todosLosTextos[0].text = message.objectName;
            todosLosTextos[1].text = message.objetDescription;

            MessageBox.gameObject.SetActive(true); // Mostrar UI
        }

    }

    /// <summary>
    /// Oculta el Panel seleccionado
    /// </summary>
    /// <param name="MessageBox">Un panelPrefab, con 2 al menos 2 TextMeshProUGUI sobre el que se va a escribir </param>
    public void HideAlertMessage(GameObject MessageBox)
    {
        if (MessageBox.gameObject == null) return;
        MessageBox.gameObject.SetActive(false); // Ocultar UI
    }
}
