using System.Collections;
using UnityEngine;

/// <summary>
/// Controla el modo Focus del jugador: sincroniza las animaciones del punto de mira y la cámara
/// con la entrada del jugador, y oculta o muestra la capa "CameraIgnore" del culling mask
/// con un pequeño retardo para evitar cambios bruscos.
/// </summary>
public class FocusController : MonoBehaviour
{
    // ── Referencias ──────────────────────────────────────────────────────────
    /// <summary>Animator del punto de mira (este mismo GameObject).</summary>
    private Animator pointAnimator;
    /// <summary>Cámara principal del jugador.</summary>
    private Camera playerCamera;
    /// <summary>Animator de la cámara principal, usado para la animación de zoom al enfocar.</summary>
    private Animator cameraAnimator;

    // ── Estado interno ───────────────────────────────────────────────────────
    /// <summary>
    /// Referencia a la corrutina de cambio de culling en curso.
    /// Evita lanzar múltiples corrutinas simultáneas para el mismo cambio.
    /// </summary>
    private Coroutine currentRoutine;

    /// <summary>Índice de la capa "CameraIgnore", precalculado en <see cref="Start"/>.</summary>
    private int cameraIgnoreLayer;
    // ── Unity Lifecycle ──────────────────────────────────────────────────────

    /// <summary>
    /// Obtiene las referencias necesarias y precalcula el índice de la capa "CameraIgnore".
    /// </summary>
    void Start()
    {
        pointAnimator = GetComponent<Animator>();
        playerCamera = Camera.main;
        cameraAnimator = playerCamera.GetComponent<Animator>();

        cameraIgnoreLayer = LayerMask.NameToLayer("CameraIgnore");
    }
    /// <summary>
    /// Evalúa el input de Focus cada frame.
    /// </summary>
    void Update()
    {
        Focus();
    }
    // ── Lógica de focus ──────────────────────────────────────────────────────

    /// <summary>
    /// Lee el input de Focus, actualiza los parámetros de los animadores y lanza la corrutina
    /// de cambio de culling si no hay ninguna ya en curso.
    /// </summary>
    void Focus()
    {
        bool isFocusing = PlayerInputManager.Actions.Player.Focus.IsPressed();

        pointAnimator.SetBool("Focus", isFocusing);
        cameraAnimator.SetBool("Focus", isFocusing);

        if (currentRoutine == null)
        {
            currentRoutine = StartCoroutine(ChangeCullingAfterDelay(!isFocusing));
        }
    }

    /// <summary>
    /// Espera 0.1 segundos y luego activa o desactiva la capa "CameraIgnore" en el culling mask
    /// de la cámara mediante operaciones de bits.
    /// </summary>
    /// <param name="enableLayer">
    /// Si es <c>true</c>, añade la capa al culling mask (la hace visible).
    /// Si es <c>false</c>, la elimina (la oculta).
    /// </param>
    private IEnumerator ChangeCullingAfterDelay(bool enableLayer)
    {
        yield return new WaitForSeconds(0.1f);

        if (enableLayer)
            playerCamera.cullingMask |= (1 << cameraIgnoreLayer);   // Mostrar capa
        else
            playerCamera.cullingMask &= ~(1 << cameraIgnoreLayer);  // Ocultar capa

        currentRoutine = null;
    }
}
