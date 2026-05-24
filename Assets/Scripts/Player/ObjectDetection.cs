using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ObjectDetection : MonoBehaviour
{
    [SerializeField] private int rango = 5;
    [SerializeField] Transform camara;
    [SerializeField] Image Marker;
    [SerializeField] GameObject MessageBox;

    IAlertService _alertService;
    IAnimationService _animationService;

    public Color color = Color.yellow;
    private Color colorBase = Color.white;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animationService = AppContainer.Get<IAnimationService>();
        _alertService = AppContainer.Get<IAlertService>();
        colorBase = Marker.color;
    }

    void FixedUpdate()
    {
        Marker.color = colorBase;
        _alertService.HideAlertMessage(MessageBox);

        RaycastHit hit;
        if (Physics.Raycast(camara.transform.position, camara.transform.forward, out hit, rango))
        {
            if (hit.collider.TryGetComponent<ICollectable>(out ICollectable collectable))
            {
                Marker.color = color;

                if (PlayerInputManager.Actions.Player.Interact.IsPressed())
                {
                    collectable.Collect();
                    Debug.Log("Se ha recogido algo");
                }
            }

            if (hit.collider.TryGetComponent<DataShow>(out DataShow data))
            {
                Marker.color = color;
                _alertService.ShowAlertMessage(MessageBox, data.GetData());
                
            }
        }
    }
}
