using System.Collections;
using UnityEngine;

/// <summary>
/// Teletransportador con fade al color que queramos.
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

    // OnRenderObject se llama después de que la cámara termina de renderizar,
    // ideal para superponer cosas sobre toda la escena sin usar UI.
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
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (_audioService == null) _audioService = AppContainer.Get<IAudioService>();
        if (sonidoTeletransporte != null)

        _audioService.PlaySound(sonidoTeletransporte);

        is_inside = true;
        teleportCoroutine = StartCoroutine(TeleportRoutine(other.gameObject));
    }

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