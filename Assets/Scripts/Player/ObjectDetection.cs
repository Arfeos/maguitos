using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
/// <summary>
/// Componente de Unity encargado de detectar objetos frente al jugador mediante Raycast. 
/// Gestiona la interacción con objetos coleccionables y la visualización de información contextual utilizando los servicios <see cref="IAlertService"/> y <see cref="IAnimationService"/>. 
/// También utiliza <see cref="PlayerInputManager"/> para detectar las acciones del jugador
/// </summary>
public class ObjectDetection : MonoBehaviour
{
    /// <summary>
    /// Variable serializada que define la distancia máxima de detección del Raycast
    /// </summary>
    [SerializeField] private int rango = 5;
    /// <summary>
    /// Referencia al transform de la cámara desde donde se lanzará el Raycast
    /// </summary>
    [SerializeField] Transform camara;
    /// <summary>
    /// Elemento visual utilizado como indicador para mostrar cuándo un objeto puede ser interactuado
    /// </summary>
    [SerializeField] Image Marker;
    /// <summary>
    /// Objeto encargado de mostrar mensajes o información contextual en pantalla mediante <see cref="IAlertService"/>
    /// </summary>
    [SerializeField] GameObject MessageBox;

    IAlertService _alertService;
    IAnimationService _animationService;

    public Color color = Color.yellow;
    private Color colorBase = Color.white;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /// <summary>
    /// Método ejecutado al comenzar la escena. Obtiene referencias a los servicios <see cref="IAnimationService"/> y <see cref="IAlertService"/> mediante <see cref="AppContainer"/> y almacena el color inicial del marcador visual
    /// </summary>
    void Start()
    {
        _animationService = AppContainer.Get<IAnimationService>();
        _alertService = AppContainer.Get<IAlertService>();
        colorBase = Marker.color;
    }
    /// <summary>
    /// Método ejecutado automáticamente a intervalos fijos. Lanza un Raycast desde la posición de la cámara para detectar objetos dentro del rango especificado. 
    /// Si detecta un objeto que implementa <see cref="ICollectable"/>, modifica el color del marcador y permite recogerlo utilizando <see cref="PlayerInputManager"/>. 
    /// Si detecta un objeto <see cref="DataShow"/>, muestra información contextual mediante <see cref="IAlertService"/>
    /// </summary>
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
