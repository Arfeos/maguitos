using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Objeto coleccionable que representa una linterna. Al ser recogida por el jugador,
/// se acopla al socket correspondiente, oculta su mesh y permite encenderla o apagarla
/// con el input configurado en <see cref="PlayerInputManager"/>.
/// </summary>
public class LanternCollect : MonoBehaviour, ICollectable
{
    // ── Estado interno ───────────────────────────────────────────────────────
    /// <summary>Indica si la linterna ya ha sido recogida y equipada por el jugador.</summary>
    private bool equipped = false;

    /// <summary>Componente <see cref="Light"/> hijo que se activa y desactiva al usar la linterna.</summary>
    private Light lanternLight;

    /// <summary>Transform de la cámara del jugador, usado para sincronizar la rotación de la linterna.</summary>
    private Transform cameraTransform;
   
    // ── Configuración ────────────────────────────────────────────────────────
    /// <summary>Offset de rotación aplicado sobre la rotación de la cámara para orientar visualmente la linterna.</summary>
    [SerializeField]
    private Vector3 rotationOffset = new Vector3(75f, 0f, 0f);

    // ── Unity Lifecycle ──────────────────────────────────────────────────────

    /// <summary>
    /// Obtiene el componente de luz hijo y suscribe el toggle de la linterna al input del jugador.
    /// </summary>
    private void Start()
    {
        lanternLight = GetComponentInChildren<Light>();
        PlayerInputManager.Actions.Player.Lantern.performed += TurnLantern;
    }

    /// <summary>
    /// Actualiza la rotación de la linterna tras el Update de la cámara para evitar jitter visual.
    /// Solo actúa si la linterna está equipada y la referencia a la cámara es válida.
    /// </summary>
    private void LateUpdate()
    {
        if (!equipped || cameraTransform == null)
            return;

        SetRotation();
    }

    /// <summary>
    /// Desuscribe el input al destruir el objeto para evitar referencias nulas en el action map.
    /// </summary>
    private void OnDestroy()
    {
        PlayerInputManager.Actions.Player.Lantern.performed -= TurnLantern;
    }
  
    // ── ICollectable ─────────────────────────────────────────────────────────

    /// <summary>
    /// Recoge la linterna: la adjunta al socket del jugador, oculta sus renderers,
    /// desactiva su collider y guarda la referencia a la cámara para la rotación.
    /// No hace nada si no hay ningún <see cref="PlayerController"/> en escena.
    /// </summary>
    public void Collect()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();

        if (player == null)
            return;

        cameraTransform = player.GetComponentInChildren<Camera>().transform;

        SetPosition(player.transform);
        DisableRender();

        equipped = true;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Aplica la rotación de la cámara más el <see cref="rotationOffset"/> a la linterna.
    /// </summary>
    private void SetRotation()
    {
        Quaternion offset = Quaternion.Euler(rotationOffset);
        transform.rotation = cameraTransform.rotation * offset;
    }

    /// <summary>
    /// Alterna el estado de la luz de la linterna al recibir el input correspondiente.
    /// No hace nada si la linterna no está equipada o el componente de luz es nulo.
    /// </summary>
    /// <param name="ctx">Contexto del input action, requerido por la firma del delegado.</param>
    private void TurnLantern(InputAction.CallbackContext ctx)
    {
        if (!equipped || lanternLight == null)
            return;

        lanternLight.enabled = !lanternLight.enabled;
    }

    /// <summary>
    /// Adjunta la linterna al <see cref="LanternSocket"/> hijo del jugador,
    /// resetea posición y rotación local, y desactiva el collider para evitar interacciones físicas.
    /// No hace nada si el socket no existe en la jerarquía del jugador.
    /// </summary>
    /// <param name="player">Transform raíz del jugador.</param>
    private void SetPosition(Transform player)
    {
        LanternSocket socket = player.GetComponentInChildren<LanternSocket>();

        if (socket == null)
            return;

        transform.SetParent(socket.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity; // La rotación la controla LateUpdate

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
    }

    /// <summary>
    /// Desactiva todos los <see cref="MeshRenderer"/> hijos de la linterna.
    /// Se llama al recogerla para ocultarla visualmente mientras sigue activa en escena.
    /// </summary>
    private void DisableRender()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer r in renderers)
            r.enabled = false;
    }
}