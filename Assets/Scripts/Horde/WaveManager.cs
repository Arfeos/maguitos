
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

/// <summary>
/// MonoBehaviour que gestiona el sistema de oleadas de enemigos,
/// controlando el spawning, la progresión de hordas, la puntuación
/// y la pantalla de muerte al morir el jugador.
/// </summary>
public class WaveManager : MonoBehaviour
{
    [Header("References")]
    /// <summary>Array de prefabs de enemigos que pueden aparecer en las hordas.</summary>
    [SerializeField] GameObject[] enemy;
    /// <summary>Prefab del panel informativo que muestra el contador de horda y tiempo.</summary>
    [SerializeField] GameObject HordeInfoPrefab;
    /// <summary>Referencia al GameObject del jugador, usado para posicionar la UI y calcular distancias de spawn.</summary>
    [SerializeField] GameObject player;
    /// <summary>Centro del mapa usado como origen para calcular las posiciones de spawn.</summary>
    [SerializeField] GameObject Mapcenter;
    /// <summary>Prefab del panel que se muestra al morir el jugador.</summary>
    [SerializeField] GameObject DeathPanel;
    /// <summary>Límite inferior del área de spawn relativo al centro del mapa.</summary>
    [SerializeField] private Vector2 mapMinBounds;
    /// <summary>Límite superior del área de spawn relativo al centro del mapa.</summary>
    [SerializeField] private Vector2 mapMaxBounds;
    /// <summary>Máscara de capas usada para detectar obstáculos al buscar posiciones de spawn válidas.</summary>
    [SerializeField] private LayerMask obstacleMask;
    /// <summary>Referencia al servicio de eventos para suscribirse al <see cref="DieEvent"/>.</summary>
    IEventService _eventService;
    /// <summary>Referencia al servicio de puntuación para sumar puntos y registrar la partida.</summary>
    IScoreService _scoreService;
    /// <summary>Referencia al servicio de perfiles para obtener el perfil activo del jugador.</summary>
    IProfileService _profileService;
    [Header("Wave Settings")]
    /// <summary>Distancia mínima entre el jugador y una posición de spawn para que sea válida.</summary>
    [SerializeField] private float minSpawnDistance = 2f;
    /// <summary>Tiempo máximo en segundos que dura cada horda antes de pasar a la siguiente.</summary>
    [SerializeField] private int maxCount = 30;
    /// <summary>Número de enemigos que se generan al inicio de cada horda.</summary>
    [SerializeField] private int enemiesPerHorde = 2;
    /// <summary>Puntos base que se otorgan al completar cada horda, escalados con el progreso.</summary>
    [SerializeField] private int pointsPerHorde=1000;
    /// <summary>Temporizador de cuenta atrás para la horda actual.</summary>
    private float timer;
    /// <summary>Número de la horda actual, incrementado al comenzar cada nueva oleada.</summary>
    private int horde = 1;
    /// <summary>Indica si hay una horda activa en curso.</summary>
    private bool isInHorde = false;
    /// <summary>Texto que muestra el número de horda actual en la UI del jugador.</summary>
    private TextMeshProUGUI hordeCounter;
    /// <summary>Componente de localización que muestra el tiempo restante de la horda.</summary>
    private LocalizeStringEvent counter;
    /// <summary>GameObject padre que agrupa a todos los enemigos spawneados en la horda actual.</summary>
    private GameObject enemiesParent;
    /// <summary>Indica si el jugador ya ha muerto, para evitar procesar el evento de muerte más de una vez.</summary>
    private bool death = false;

    /// <summary>
    /// Obtiene las referencias a los servicios y se suscribe al <see cref="DieEvent"/>
    /// al inicializarse el componente.
    /// </summary>
    private void Awake()
    {
        _profileService = AppContainer.Get<IProfileService>();
        _scoreService = AppContainer.Get<IScoreService>();
        _eventService = AppContainer.Get<IEventService>();
        _eventService.Subscribe<DieEvent>(OnPlayerDeath);   
    }
    /// <summary>
    /// Actualiza el temporizador y comprueba el estado de los enemigos cada frame.
    /// No realiza ninguna acción si no hay una horda activa.
    /// </summary>
    void Update()
    {
        if (!isInHorde) return;
        //checkEnemies();
        timer -= Time.deltaTime;
        updateCounter();
        checkEnemies();
    }
    /// <summary>
    /// Cancela la suscripción al <see cref="DieEvent"/> al destruirse el componente.
    /// </summary>
    private void OnDestroy()
    {
        _eventService.Unsubscribe<DieEvent>(OnPlayerDeath);
    }
    /// <summary>
    /// Comprueba si todos los enemigos han sido eliminados o si el temporizador ha llegado a cero,
    /// avanzando a la siguiente horda en ese caso.
    /// </summary>
    private void checkEnemies()
    {
        if (enemiesParent.transform.childCount <= 0 || timer <= 0)
        {
            NextHorde();
        }
    }

    /// <summary>
    /// Inicia la primera horda, instancia el panel informativo en el canvas del jugador,
    /// crea el contenedor de enemigos y lanza el primer spawn.
    /// </summary>
    public void beginHorde()
    {
        timer = maxCount;
        isInHorde = true;
        Canvas canvas = player.GetComponentInChildren<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("El player no tiene Canvas");
            return;
        }

        GameObject counterObj = Instantiate(HordeInfoPrefab, canvas.transform);
        hordeCounter = counterObj.GetComponentsInChildren<TextMeshProUGUI>()[0];
        Transform txtTime = counterObj.transform.Find("CounterNext/txt_Time");
        counter = txtTime.GetComponent<LocalizeStringEvent>();
        hordeCounter.text = horde.ToString();
        enemiesParent = new GameObject("Enemies");
        spawnEnemies();
        updateCounter();

        Debug.Log("inicia la ronda: " + horde);

    }
    /// <summary>
    /// Actualiza el argumento de localización del contador de tiempo con el valor
    /// redondeado hacia arriba del temporizador actual y refresca el texto en pantalla.
    /// </summary>
    private void updateCounter()
    {
        if (counter == null)
            return;
        counter.StringReference.Arguments = new object[]
{
    Mathf.CeilToInt(timer)
};

        counter.RefreshString();
        //counter.StringReference.Arguments = new object[] {Mathf.CeilToInt(timer) };
    }
    /// <summary>
    /// Instancia los enemigos de la horda actual en posiciones válidas del mapa,
    /// eligiendo aleatoriamente entre los prefabs disponibles en <see cref="enemy"/>.
    /// </summary>
    private void spawnEnemies()
    {
        for (int i = 0; i < enemiesPerHorde; i++)
        {
            Vector3 spawnPos;
            if(TryGetPosition(out spawnPos))
            GameObject.Instantiate(enemy[UnityEngine.Random.Range(0, enemy.Length)], spawnPos, Quaternion.identity, enemiesParent.transform);
        }
    }
    /// <summary>
    /// Intenta encontrar una posición de spawn válida dentro de los límites del mapa,
    /// descartando posiciones demasiado cercanas al jugador o bloqueadas por obstáculos.
    /// </summary>
    /// <param name="spawnPos">Posición válida encontrada, o <see cref="Vector3.zero"/> si no se encontró ninguna.</param>
    /// <returns><c>true</c> si se encontró una posición válida; <c>false</c> si se agotaron los intentos.</returns>
    private bool TryGetPosition(out Vector3 spawnPos)
    {
        int maxAtempts = 20;
        for (int i = 0; i < maxAtempts; i++) {
            float randomX = Random.Range(mapMinBounds.x, mapMaxBounds.x);
            float randomZ = Random.Range(mapMinBounds.y, mapMaxBounds.y);
            Vector3 randomPos = new Vector3(Mapcenter.transform.position.x + randomX, Mapcenter.transform.position.y, Mapcenter.transform.position.z + randomZ);
            Debug.Log("Intento " + i + ": Posición aleatoria generada: " + randomPos);
            //checkea la distancia con el jugador
            if (Vector3.Distance(player.transform.position, randomPos) < minSpawnDistance)
              continue;
            //compureba si choca con algun obstaculo
            bool blocked = Physics.CheckSphere(randomPos, 1f, obstacleMask);
            Debug.Log("Intento " + i + ": ¿Posición bloqueada por obstáculos? " + blocked);
            if (blocked) continue;
            spawnPos= randomPos;
            return true;
        }
        spawnPos = Vector3.zero;
        return false;
    }
    /// <summary>
    /// Avanza a la siguiente horda incrementando el contador, escalando el número de enemigos
    /// y los puntos por horda en un 50%, sumando los puntos al perfil activo y spawneando
    /// la nueva oleada de enemigos.
    /// </summary>
    private void NextHorde()
    {
        timer = maxCount;
        horde++;
        enemiesPerHorde = Mathf.CeilToInt(enemiesPerHorde*1.5f);
        //_scoreService.addPoints("Pepe", pointsPerHorde);
         _scoreService.addPoints(_profileService.getSelectedProfile().guid, pointsPerHorde);
        pointsPerHorde = Mathf.CeilToInt(pointsPerHorde * 1.5f);
        spawnEnemies();
        hordeCounter.text = horde.ToString();
        Debug.Log("Siguiente horda: " + horde);
    }
    /// <summary>
    /// Callback invocado al recibir el <see cref="DieEvent"/>.
    /// Pausa el juego, muestra el cursor, cambia el mapa de controles a UI,
    /// registra la puntuación final y muestra el panel de muerte con la horda
    /// alcanzada y la puntuación obtenida. Solo se ejecuta una vez gracias al flag <see cref="death"/>.
    /// </summary>
    /// <param name="base">Evento base recibido, correspondiente a un <see cref="DieEvent"/>.</param>
    private void OnPlayerDeath(GameEventBase @base)
    {
        if (!death) { 
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        PlayerInputManager.SwitchControlMap(PlayerInputManager.ControlMap.UI);
        _scoreService.AddScore();
        GameObject canvas= GameObject.Instantiate(DeathPanel);
        LocalizeStringEvent txtwave = canvas.transform.GetComponentsInChildren<LocalizeStringEvent>()[0];
        txtwave.StringReference.Arguments = new object[] { horde };
            txtwave.RefreshString();
        LocalizeStringEvent txtscore = canvas.transform.GetComponentsInChildren<LocalizeStringEvent>()[1];
            txtscore.StringReference.Arguments = new object[] { _scoreService.GetPoints(_profileService.getSelectedProfile().guid) };
            txtscore.RefreshString();
            Destroy(FindAnyObjectByType<PlayerController>().GetComponent<PlayerController>());
        DeathPanel.SetActive(true);
            death = true;
        } 
    }


}
