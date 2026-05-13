using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.PropertyVariants;
using static UnityEngine.Rendering.DebugUI;

public class AlertService : MonoBehaviour, IAlertService
{
    /// <summary>
    /// Muestra los datos del objeto que se le pasen por pantalla en el Panel adecuado seleccionado
    /// </summary>
    /// <param name="MessageBox">Un panel, con 2 al menos 2 TextMeshProUGUI sobre el que se va a escribir </param>
    /// <param name="message">Data de un scriptable object del tipo ObjectDataScriptable</param>
    public void ShowAlertMessage(GameObject MessageBox, ObjectDataScriptable message)
    {
        if (MessageBox == null) return;

        TextMeshProUGUI[] todosLosTextos = MessageBox.GetComponentsInChildren<TextMeshProUGUI>(true);

        if (todosLosTextos != null && todosLosTextos.Length >= 2)
        {
            TextMeshProUGUI textoNombre = todosLosTextos[0];
            TextMeshProUGUI textoDescripcion = todosLosTextos[1];

            // GetLocalizedStringAsync obtiene el string traducido al idioma activo
            message.objectName.GetLocalizedStringAsync().Completed += handle =>
            {
                textoNombre.text = handle.Result;
            };

            message.objetDescription.GetLocalizedStringAsync().Completed += handle =>
            {
                textoDescripcion.text = handle.Result;
            };

            MessageBox.SetActive(true);
        }

        //if (todosLosTextos != null)
        //{
        //    todosLosTextos[0].text = message.objectName;
        //    todosLosTextos[1].text = message.objetDescription;

        //    MessageBox.gameObject.SetActive(true); // Mostrar UI
        //}

    }

    /// <summary>
    /// Oculta el Panel seleccionado
    /// </summary>
    /// <param name="MessageBox">Un panel, con 2 al menos 2 TextMeshProUGUI sobre el que se va a escribir </param>
    public void HideAlertMessage(GameObject MessageBox)
    {
        if (MessageBox.gameObject == null) return;
        MessageBox.gameObject.SetActive(false); // Ocultar UI
    }
}
