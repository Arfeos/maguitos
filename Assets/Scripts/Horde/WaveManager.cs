using System;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject counterPrefab;
    [SerializeField] GameObject player;
    [SerializeField] GameObject Mapcenter;

    [Header("Wave Settings")]
    [SerializeField] private int maxCount = 30;
    [SerializeField] private int enemiesPerHorde = 2;
    private float timer;
    private int horde = 1;
    private bool isInHorde = false;
    private TextMeshProUGUI counter;
    private GameObject enemiesParent;

    // Update is called once per frame
    void Update()
    {
        if (!isInHorde) return;
        //checkEnemies();
        timer -= Time.deltaTime;
        updateCounter();
        checkEnemies();
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

        GameObject counterObj = Instantiate(counterPrefab, canvas.transform);

        counter = counterObj.GetComponent<TextMeshProUGUI>();
        enemiesParent = new GameObject("Enemies");
        spawnEnemies();
        updateCounter();

        Debug.Log("inicia la ronda: " + horde);

    }

    private void updateCounter()
    {
        if (counter == null)
            return;

        counter.text = "Tiempo: " + Mathf.CeilToInt(timer);
    }

    private void spawnEnemies()
    {
        for (int i = 0; i < enemiesPerHorde; i++)
        {
            Vector3 randosPos = player.transform.position + new Vector3(UnityEngine.Random.Range(-25, 25), 0, UnityEngine.Random.Range(-25, 25));
            GameObject.Instantiate(enemy, randosPos, Quaternion.identity, enemiesParent.transform);
        }
    }
    private void NextHorde()
    {
        timer = maxCount;
        horde++;
        enemiesPerHorde += 2;
        spawnEnemies();
        Debug.Log("Siguiente horda: " + horde);
    }

}
