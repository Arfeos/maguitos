using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Genera una cuadrícula de botones de selección de avatar a partir de una lista de sprites.
/// Al pulsar un botón, publica un <see cref="IconChangeEvent"/> con la ruta del icono seleccionado.
/// </summary>
public class AvatarGrid : MonoBehaviour
{
    // ── Configuración ────────────────────────────────────────────────────────
    [Header("Configuración")]
    /// <summary>Prefab del botón que se instancia por cada avatar. Debe tener <see cref="Image"/> y <see cref="Button"/>.</summary>
    public GameObject buttonPrefab;

    /// <summary>Contenedor del grid donde se instancian los botones (normalmente un GridLayoutGroup).</summary>
    public Transform gridContent;

    /// <summary>Lista de sprites de avatar disponibles para seleccionar.</summary>
    public List<Sprite> allAvatars;

    // ── Servicios ────────────────────────────────────────────────────────────
    /// <summary>Servicio de eventos usado para publicar el cambio de icono al seleccionar un avatar.</summary>
    private IEventService eventService;

    // ── Estado interno ───────────────────────────────────────────────────────
    /// <summary>Imagen del botón del avatar actualmente seleccionado.</summary>
    private Image selectedImage;


    // ── Unity Lifecycle ──────────────────────────────────────────────────────

    /// <summary>
    /// Resuelve el servicio de eventos y genera la cuadrícula de avatares.
    /// </summary>
    void Start()
    {
        eventService = AppContainer.Get<IEventService>();
        GenerateGrid();
    }

    // ── Generación de UI ─────────────────────────────────────────────────────

    /// <summary>
    /// Instancia un botón por cada sprite en <see cref="allAvatars"/>, asigna el sprite
    /// a su <see cref="Image"/> y registra el listener de click.
    /// </summary>
    void GenerateGrid()
    {
        foreach (Sprite avatar in allAvatars)
        {
            GameObject newButton = Instantiate(buttonPrefab, gridContent);
            Image btnImg = newButton.GetComponent<Image>();
            btnImg.sprite = avatar;

            Button btn = newButton.GetComponent<Button>();
            btn.onClick.AddListener(() => OnAvatarClicked(btnImg));
        }
    }

    // ── Callbacks ────────────────────────────────────────────────────────────

    /// <summary>
    /// Registra el avatar pulsado como seleccionado y publica un <see cref="IconChangeEvent"/>
    /// con la ruta "Icons/{nombre del sprite}" para que otros sistemas actualicen el icono del perfil.
    /// </summary>
    /// <param name="clickedImage">Imagen del botón pulsado, cuyo sprite identifica el avatar elegido.</param>
    void OnAvatarClicked(Image clickedImage)
    {
        selectedImage = clickedImage;
        eventService.Publish(new IconChangeEvent("Icons/" + clickedImage.sprite.name));
        Debug.Log("Avatar seleccionado: " + clickedImage.sprite.name);
    }
} 