using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class CursorMouse : MonoBehaviour
{
    [SerializeField] private InputActionReference _pointerPositionAction;
    private RectTransform _cursorTransform;
    private Canvas _parentCanvas;
    private RectTransform _canvasRectTransform;
    private Camera _canvasCamera;

    void Awake()
    {
        _cursorTransform = GetComponent<RectTransform>();
        _parentCanvas = GetComponentInParent<Canvas>();
        if (_parentCanvas != null)
        {
            _canvasRectTransform = _parentCanvas.GetComponent<RectTransform>();
            _canvasCamera = _parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _parentCanvas.worldCamera;
        }
    }

    void OnEnable()
    {
        //Cursor.visible = false;
        _pointerPositionAction.action.performed += OnPointerPositionChanged;
    }

    void OnDisable()
    {
        Cursor.visible = true;
        _pointerPositionAction.action.performed -= OnPointerPositionChanged;
    }

    private void OnPointerPositionChanged(InputAction.CallbackContext ctx)
    {
        if (_cursorTransform == null || _canvasRectTransform == null)
            return;

        var mousePosition = ctx.ReadValue<Vector2>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform, mousePosition, _canvasCamera, out var localPoint))
        {
            _cursorTransform.anchoredPosition = localPoint;
        }
    }

}
