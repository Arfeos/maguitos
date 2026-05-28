using UnityEngine;
/// <summary>
/// Componente de Unity encargado de controlar el comportamiento de un objeto flotante coleccionable. 
/// Gestiona la animación de flotación, la detección del jugador, la persecución automática y la aplicación de efectos al personaje mediante los servicios <see cref="ICharacterService"/> y <see cref="IAudioService"/>.
/// </summary>
public class ballController : MonoBehaviour
{
    [Header("Flotación")]
    [Tooltip("Altura máxima del movimiento de flotación")]
    public float alturaFlotacion = 0.3f;

    [Tooltip("Velocidad de la animación flotante")]
    public float velocidadFlotacion = 2f;

    [Header("Detección del jugador")]
    [Tooltip("Tag del jugador en la escena")]
    public string tagJugador = "Player";

    [Tooltip("Distancia a la que el objeto empieza a perseguir al jugador")]
    public float radioDeteccion = 5f;

    [Tooltip("Distancia a la que el objeto se destruye (recogida)")]
    public float radioRecogida = 0.5f;

    [Header("Movimiento de persecución")]
    [Tooltip("Velocidad a la que el objeto se mueve hacia el jugador")]
    public float velocidadPersecucion = 4f;

    [Tooltip("Altura a la que vuela el objeto mientras persigue al jugador")]
    public float alturaVuelo = 1f;

    [Tooltip("Velocidad de interpolación para subir a alturaVuelo")]
    public float suavizadoAltura = 5f;

    [Header("Efectos (opcional)")]
    [Tooltip("Sonido que se reproduce al ser cogido")]
    public AudioClip efectoRecogida;

    [Header("Valores de recogida")]
    [Tooltip("Efectos que se añaden al jugador al hacer contacto")]
    public int mana;
    public int vida;
    // ── Estado interno ──────────────────────────────────────────────────────
    private Transform jugador;
    private Vector3 posicionInicial;
    private bool persiguiendo = false;
    private ICharacterService _characterService;

    private IAudioService _audioService;

    // ── Unity Lifecycle ─────────────────────────────────────────────────────
    /// <summary>
    /// Método ejecutado al inicializar el objeto. Obtiene las referencias a los servicios <see cref="ICharacterService"/> y <see cref="IAudioService"/> mediante <see cref="AppContainer"/>.
    /// </summary>
    private void Awake()
    {
        _characterService = AppContainer.Get<ICharacterService>();

        _audioService = AppContainer.Get<IAudioService>();
    }
    /// <summary>
    /// Método ejecutado al comenzar la escena. Guarda la posición inicial del objeto y busca al jugador utilizando el tag especificado para almacenar su referencia.
    /// </summary>
    void Start()
    {
        posicionInicial = transform.position;

        // Buscar al jugador por tag
        GameObject objJugador = GameObject.FindGameObjectWithTag(tagJugador);
        if (objJugador != null)
        {
            jugador = objJugador.transform;
        }
        else
        {
            Debug.LogWarning($"[ObjetoFlotante] No se encontró ningún objeto con el tag '{tagJugador}'. " +
                             "Asegúrate de que el jugador tiene ese tag asignado.");
        }
    }
    /// <summary>
    /// Método ejecutado automáticamente a intervalos fijos. 
    /// Controla el comportamiento del objeto comprobando la distancia con el jugador. 
    /// Si el jugador se encuentra dentro del rango de detección, activa el modo persecución; cuando el jugador alcanza el rango de recogida, ejecuta Recoger().
    /// </summary>
    void FixedUpdate()
    {
        if (jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (!persiguiendo)
        {
            AnimarFlotacion();

            // ¿El jugador ya está dentro del radio de detección?
            if (distancia <= radioDeteccion)
            {
                persiguiendo = true;
            }
        }
        else
        {
            PerseguirJugador();

            // ¿Llegó al jugador?
            if (distancia <= radioRecogida)
            {
                Recoger();
            }
        }
    }

    // ── Métodos privados ────────────────────────────────────────────────────

    /// <summary>Animación senoidal de flotación sobre el punto inicial.</summary>
    void AnimarFlotacion()
    {
        float nuevaY = posicionInicial.y + Mathf.Sin(Time.time * velocidadFlotacion) * alturaFlotacion;
        transform.position = new Vector3(posicionInicial.x, nuevaY, posicionInicial.z);
    }

    /// <summary>Mueve el objeto hacia el jugador, elevándose a alturaVuelo.</summary>
    void PerseguirJugador()
    {
        // Objetivo: posición del jugador + altura de vuelo
        Vector3 objetivo = new Vector3(
            jugador.position.x,
            jugador.position.y + alturaVuelo,
            jugador.position.z
        );

        // Movimiento suavizado
        transform.position = Vector3.MoveTowards(
            transform.position,
            objetivo,
            velocidadPersecucion * Time.deltaTime
        );

    }

    /// <summary>Gestiona la recogida del objeto. 
    /// Reproduce un sonido mediante <see cref="IAudioService"/> si existe un efecto asignado, aplica los efectos correspondientes sobre el personaje mediante <see cref="ICharacterService"/> y elimina el objeto de la escena.</summary>
    void Recoger()
    {
        if (efectoRecogida != null)
        {
            _audioService.PlaySound(efectoRecogida);
        }
        _characterService.AddMana(mana);
        _characterService.Heal(vida);

        Destroy(gameObject);
    }
}
