using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class AlertService : MonoBehaviour, IAlertService
{
    /// <summary>
    /// Muestra los datos del objeto que se le pasen por pantalla en el Panel adecuado seleccionado
    /// </summary>
    /// <param name="MessageBox">Un PausepanelPrefab, con 2 al menos 2 TextMeshProUGUI sobre el que se va a escribir </param>
    /// <param name="message">Data de un scriptable object del tipo ObjectDataScriptable</param>
    public void ShowAlertMessage(GameObject MessageBox, ObjectDataScriptable message)
    {
        if (MessageBox == null) return;

        TextMeshProUGUI[] todosLosTextos = MessageBox.GetComponentsInChildren<TextMeshProUGUI>(true);
        Image genImage = null;
        foreach (Transform child in MessageBox.transform)
        {
            genImage = child.GetComponentInChildren<Image>(true);
            if (genImage != null) break;
        }

        if (todosLosTextos != null && todosLosTextos.Length >= 2)
        {
            TextMeshProUGUI textoNombre = todosLosTextos[0];
            TextMeshProUGUI textoDescripcion = todosLosTextos[1];

            message.objectName.GetLocalizedStringAsync().Completed += handle =>
            {
                textoNombre.text = handle.Result;
            };

            // Usa stats si hay spellData, sino la descripción normal
            if (message.spellData != null)
            {
                var typeLocalized = new LocalizedString { TableReference = "InfoPanel", TableEntryReference = message.GetTypeKey() };
                var importanceLocalized = new LocalizedString { TableReference = "InfoPanel", TableEntryReference = message.GetImportanceKey() };

                typeLocalized.GetLocalizedStringAsync().Completed += typeHandle =>
                {
                    importanceLocalized.GetLocalizedStringAsync().Completed += importanceHandle =>
                    {
                        string statsEntry = message.spellData.penetrates ? "spellStatsPenetration" : "spellStats";
                        var statsLocalized = new LocalizedString
                        {
                            TableReference = "InfoPanel",
                            TableEntryReference = statsEntry,
                            Arguments = message.spellData.penetrates
                                ? new object[] { importanceHandle.Result, typeHandle.Result, message.spellData.damage, message.spellData.manaCost, message.spellData.lifeTime, message.spellData.penetrationlevel }
                                : new object[] { importanceHandle.Result, typeHandle.Result, message.spellData.damage, message.spellData.manaCost, message.spellData.lifeTime }
                        };

                        statsLocalized.GetLocalizedStringAsync().Completed += statsHandle =>
                        {
                            textoDescripcion.text = statsHandle.Result;
                        };
                    };
                };
            }
            else
            {
                message.objetDescription.GetLocalizedStringAsync().Completed += handle =>
                {
                    textoDescripcion.text = handle.Result;
                };
            }

            MessageBox.SetActive(true);
        }

        if (genImage != null && message.objectSprite != null)
            genImage.sprite = message.objectSprite;

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
    /// <param name="MessageBox">Un PausepanelPrefab, con 2 al menos 2 TextMeshProUGUI sobre el que se va a escribir </param>
    public void HideAlertMessage(GameObject MessageBox)
    {
        if (MessageBox.gameObject == null) return;
        MessageBox.gameObject.SetActive(false); // Ocultar UI
    }
}
