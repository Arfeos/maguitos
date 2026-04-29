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
    public Color color = Color.white;
    private Color colorBase = Color.white;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _alertService = AppContainer.Get<IAlertService>();
        colorBase = Marker.color;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Marker.color = colorBase;
        _alertService.HideAlertMessage(MessageBox);
        //HideMessage();

        RaycastHit hit;
        if(Physics.Raycast(camara.transform.position, camara.transform.forward, out hit, rango)){


            if (hit.collider.TryGetComponent<ICollectable>(out ICollectable Collectable) && PlayerInputManager.Actions.Player.Interact.WasPressedThisFrame())
            {
                Marker.color = color;
                //TODO: Cambiar a un color distinto para cada cosa
                Collectable.Collect();
            }

            if (hit.collider.TryGetComponent<DataShow>(out DataShow data))
            {
                Marker.color = color;
                //ShowMessage(data.getData());
                _alertService.ShowAlertMessage(MessageBox,data.getData());
                Debug.Log("Estas apuntando a cosas");
            }
            

        }
    }
}
