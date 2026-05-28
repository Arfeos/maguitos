using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// MonoBehaviour que desplaza automáticamente un <see cref="ScrollRect"/> horizontal
/// para mantener visible el elemento de UI actualmente seleccionado con mando o teclado.
/// </summary>
public class ScrollFollow : MonoBehaviour
{
    /// <summary>Referencia al <see cref="ScrollRect"/> que se desplazará para seguir la selección.</summary>
    [SerializeField] private ScrollRect scrollRect;
    /// <summary>RectTransform del contenido desplazable que contiene los elementos seleccionables.</summary>
    [SerializeField] private RectTransform content;
    /// <summary>RectTransform del viewport que define el área visible del scroll.</summary>
    [SerializeField] private RectTransform viewport;

    /// <summary>Último elemento seleccionado, usado para evitar recalcular el scroll si la selección no ha cambiado.</summary>
    private GameObject lastSelected;

    /// <summary>
    /// Comprueba cada frame si el elemento seleccionado ha cambiado y,
    /// si pertenece al contenido del scroll, desplaza la vista para mantenerlo visible.
    /// </summary>
    void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected == null || selected == lastSelected)
            return;

        if (!selected.transform.IsChildOf(content))
            return;

        lastSelected = selected;

        RectTransform selectedRect =
            selected.GetComponent<RectTransform>();

        ScrollTo(selectedRect);
    }

    // <summary>
    /// Calcula la posición horizontal normalizada del target dentro del contenido
    /// y aplica el desplazamiento al <see cref="ScrollRect"/> para centrarlo en el viewport.
    /// </summary>
    /// <param name="target">RectTransform del elemento al que se debe desplazar la vista.</param>
    void ScrollTo(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        Vector2 viewportLocalPos = viewport.localPosition;
        Vector2 childLocalPos = target.localPosition;

        float contentWidth = content.rect.width;
        float viewportWidth = viewport.rect.width;

        float normalized =
            Mathf.Clamp01(
                (Mathf.Abs(childLocalPos.x) - viewportWidth * 0.5f)
                / (contentWidth - viewportWidth)
            );

        scrollRect.horizontalNormalizedPosition = normalized;
    }
}
