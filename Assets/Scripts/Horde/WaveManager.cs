
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;


public class WaveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject[] enemy;
    [SerializeField] GameObject HordeInfoPrefab;
    [SerializeField] GameObject player;
    [SerializeField] GameObject Mapcenter;
    [SerializeField] GameObject DeathPanel;
    [SerializeField] private Vector2 mapMinBounds;
    [SerializeField] private Vector2 mapMaxBounds;
    
    [SerializeField] private LayerMask obstacleMask;
    IEventService _eventService;
    IScoreService _scoreService;
    IProfileService _profileService;
    [Header("Wave Settings")]
    [SerializeField] private float minSpawnDistance = 2f;
    [SerializeField] private int maxCount = 30;
    [SerializeField] private int enemiesPerHorde = 2;
    [SerializeField] private int pointsPerHorde=1000;
    private float timer;
    private int horde = 1;
    private bool isInHorde = false;
    private TextMeshProUGUI hordeCounter;
    private LocalizeStringEvent counter;
    private GameObject enemiesParent;
    private bool death = false;

    // Update is called once per frame
    private void Awake()
    {
        _profileService = AppContainer.Get<IProfileService>();
        _scoreService = AppContainer.Get<IScoreService>();
        _eventService = AppContainer.Get<IEventService>();
        _eventService.Subscribe<DieEvent>(OnPlayerDeath);   
    }
    void Update()
    {
        if (!isInHorde) return;
        //checkEnemies();
        timer -= Time.deltaTime;
        updateCounter();
        checkEnemies();
    }

    private void OnDestroy()
    {
        _eventService.Unsubscribe<DieEvent>(OnPlayerDeath);
    }

    private void checkEnemies()
    {
        if (enemiesParent.transform.childCount <= 0 || timer <= 0)
        {
            NextHorde();
        }
    }


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

    private void spawnEnemies()
    {
        for (int i = 0; i < enemiesPerHorde; i++)
        {
            Vector3 spawnPos;
            if(TryGetPosition(out spawnPos))
            GameObject.Instantiate(enemy[UnityEngine.Random.Range(0, enemy.Length)], spawnPos, Quaternion.identity, enemiesParent.transform);
        }
    }

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
        LocalizeStringEvent txtscore = canvas.transform.GetComponentsInChildren<LocalizeStringEvent>()[1];
        txtscore.StringReference.Arguments = new object[] { _scoreService.GetPoints(_profileService.getSelectedProfile().guid)};
        Destroy(FindAnyObjectByType<PlayerController>().GetComponent<PlayerController>());
        DeathPanel.SetActive(true);
            death = true;
        } 
    }


}
