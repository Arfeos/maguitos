using System.Collections;
using UnityEngine;

/// <summary>
/// Teletransportador con fade al color que queramos.
/// Al entrar el jugador en el trigger, inicia un fade de pantalla,
/// espera el retardo configurado y mueve al jugador al punto de destino.
/// Al salir del trigger, cancela la secuencia y restaura el fade.
/// </summary>
public class teleporter_controller : MonoBehaviour
{
    [Header("Destino")]
    [SerializeField] Transform playerSpawn;

    [Header("Tiempos")]
    [Tooltip("Segundos dentro de la zona antes de teletransportar")]
    [SerializeField] float delayTeleport = 1f;

    [Tooltip("Duración del fade a blanco")]
    [SerializeField] float fadeDuration = 1f;

    [Header("Color del fade")]
    [SerializeField] Color fadeColor = Color.white;

    [Header("Sonido de teletransporte")]
    [SerializeField] private AudioClip sonidoTeletransporte;

    [Header("Manager de hordas")]
    [SerializeField] WaveManager WaveManger;

    // ── Estado interno ──────────────────────────────────────────────────────
    private bool is_inside = false;
    private Coroutine teleportCoroutine;
    private float currentAlpha = 0f;   // 0 = transparente, 1 = opaco
    private Material fadeMaterial;
    private IAudioService _audioService;

    private void Awake()
    {
        _audioService = AppContainer.Get<IAudioService>();
    }
    // ── Unity Lifecycle ─────────────────────────────────────────────────────
    /// <summary>
    /// Crea e inicializa el material de fade utilizado para superponer
    /// el color de pantalla completa durante el teletransporte.
    /// Configura blending con transparencia, sin culling ni depth write.
    /// </summary>
    void Start()
    {
        // Material que dibuja color plano con transparencia, sin texturas ni luces
        fadeMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
        fadeMaterial.hideFlags = HideFlags.HideAndDontSave;
        fadeMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        fadeMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        fadeMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        fadeMaterial.SetInt("_ZWrite", 0);
        fadeMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
    }

    void OnDestroy()
    {
        if (fadeMaterial != null)
            Destroy(fadeMaterial);
    }

    /// <summary>
    /// Dibuja el quad de color de pantalla completa usando GL inmediato,
    /// superponiéndolo sobre toda la escena tras el renderizado de la cámara.
    /// Solo se ejecuta cuando <see cref="currentAlpha"/> es mayor que cero.
    /// </summary>
    void OnRenderObject()
    {
        if (currentAlpha <= 0f) return;

        fadeMaterial.SetPass(0);

        GL.PushMatrix();
        GL.LoadOrtho(); // Coordenadas 0-1 sobre la pantalla

        GL.Begin(GL.QUADS);
        GL.Color(new Color(fadeColor.r, fadeColor.g, fadeColor.b, currentAlpha));
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(0, 1, 0);
        GL.Vertex3(1, 1, 0);
        GL.Vertex3(1, 0, 0);
        GL.End();

        GL.PopMatrix();
    }

    // ── Trigger ─────────────────────────────────────────────────────────────
    /// <summary>
    /// Detecta la entrada del jugador en el trigger.
    /// Reproduce el sonido de teletransporte e inicia la corrutina de teletransporte.
    /// Ignora cualquier colisionador que no tenga la etiqueta "Player".
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (_audioService == null) _audioService = AppContainer.Get<IAudioService>();
        if (sonidoTeletransporte != null)

        _audioService.PlaySound(sonidoTeletransporte);

        is_inside = true;
        teleportCoroutine = StartCoroutine(TeleportRoutine(other.gameObject));
    }
    /// <summary>
    /// Detecta la salida del jugador del trigger.
    /// Detiene el sonido de teletransporte, cancela la corrutina activa
    /// y lanza un fade de vuelta a transparente si el fade estaba en curso.
    /// Ignora cualquier colisionador que no tenga la etiqueta "Player".
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        is_inside = false;

        if (teleportCoroutine != null)
        {
            StopCoroutine(teleportCoroutine);
            teleportCoroutine = null;
        }
        if (_audioService == null) _audioService = AppContainer.Get<IAudioService>();
        if (sonidoTeletransporte != null)

        _audioService.StopSound(sonidoTeletransporte);

        // Si el fade estaba a medias, vuelve a transparente
        StartCoroutine(FadeRoutine(to: 0f));
        
    }

    // ── Coroutina principal ─────────────────────────────────────────────────
    /// <summary>
    /// Secuencia principal del teletransporte:
    /// <list type="number">
    ///   <item>Fade a opaco.</item>
    ///   <item>Espera <see cref="delayTeleport"/> segundos comprobando que el jugador sigue dentro.</item>
    ///   <item>Mueve al jugador al punto de destino (<see cref="playerSpawn"/>).</item>
    ///   <item>Fade de vuelta a transparente.</item>
    ///   <item>Inicia la horda a través de <see cref="WaveManger"/>.</item>
    /// </list>
    /// Se cancela automáticamente si el jugador abandona el trigger en cualquier momento.
    /// </summary>
    /// <param name="player">GameObject del jugador a teletransportar.</param>
    private IEnumerator TeleportRoutine(GameObject player)
    {
        yield return StartCoroutine(FadeRoutine(to: 1f));

        float elapsed = 0f;
        while (elapsed < delayTeleport)
        {
            if (!is_inside) yield break;
            elapsed += Time.deltaTime;
            yield return null;
        }

        

        if (!is_inside)
        {
            yield return StartCoroutine(FadeRoutine(to: 0f));
            yield break;
        }

        //Mover al jugador al destino
        MoverJugador(player);

        yield return new WaitForSeconds(0.1f);

        //Fade de vuelta a transparente
        yield return StartCoroutine(FadeRoutine(to: 0f));
        WaveManger.beginHorde();
        teleportCoroutine = null;
    }

    // ── Fade ─────────────────────────────────────────────────────────────────
    /// <summary>
    /// Interpola <see cref="currentAlpha"/> desde su valor actual hasta <paramref name="to"/>
    /// a lo largo de <see cref="fadeDuration"/> segundos.
    /// </summary>
    /// <param name="to">Valor de alpha objetivo (0 = transparente, 1 = opaco).</param>
    private IEnumerator FadeRoutine(float to)
    {
        float from = currentAlpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            currentAlpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        currentAlpha = to;
    }

    // ── Teletransporte ───────────────────────────────────────────────────────
    /// <summary>
    /// Mueve físicamente al jugador a la posición y rotación de <see cref="playerSpawn"/>.
    /// Desactiva el <see cref="CharacterController"/> antes de mover y lo reactiva después,
    /// y pone a cero la velocidad del <see cref="Rigidbody"/> si existe.
    /// </summary>
    /// <param name="player">GameObject del jugador a reubicar.</param>
    private void MoverJugador(GameObject player)
    {
        if (playerSpawn == null)
        {
            Debug.LogWarning("[teleporter_controller] playerSpawn no asignado.");
            return;
        }

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.SetPositionAndRotation(playerSpawn.position, playerSpawn.rotation);

        if (cc != null) cc.enabled = true;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = Vector3.zero;
    }
}