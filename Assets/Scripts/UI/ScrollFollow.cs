using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollFollow : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform viewport;

    private GameObject lastSelected;

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
