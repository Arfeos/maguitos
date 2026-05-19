using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

/// <summary>
/// Clase que se encarga de mostrar la tabla de puntuacion en la escena
/// </summary>
public class DisplayScoreTable : MonoBehaviour
{
    
    public ScoreManager scoreManager;           
    public Transform container;                
    public GameObject rowPrefab;

    public int pageSize = 5;

    private List<GameObject> _rows = new List<GameObject>();
    public int currentPage = 0;

    
    
    [SerializeField] bool _RefreshAtStart = false;

    private void Start()
    {
        if (_RefreshAtStart) RefreshTable();
    }
    /// <summary>
    /// Actualiza la tabla
    /// </summary>
    public void RefreshTable()
    {
        //Limpiar filas existentes
        foreach (var row in _rows)
        {
            Destroy(row);
        }
        _rows.Clear();

        var sortedScores = scoreManager.scoreTable.scores
           .OrderByDescending(s => s.score);

        var pageScores = sortedScores
          .Skip(currentPage * pageSize)
          .Take(pageSize);

        //Crear filas nuevas
        foreach (var entri in pageScores)
        {
            GameObject newRow = Instantiate(rowPrefab, container);
            _rows.Add(newRow);

            TextMeshProUGUI[] texts = newRow.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 3)
            {
                texts[0].text = entri.playerName;
                texts[1].text = entri.score.ToString();
                texts[2].text = entri.pacifist.ToString();
            }
        }
        int maxPage = Mathf.CeilToInt((float)sortedScores.Count() / pageSize);
        
    }

    /// <summary>
    /// Pasa a la siguiente página
    /// </summary>
    public void NextPage()
    {
       
        int maxPage = Mathf.CeilToInt((float)scoreManager.scoreTable.scores.Count / pageSize) - 1;
        if (currentPage < maxPage)
        { 
            
            currentPage++;
            RefreshTable();
        }
    }
    /// <summary>
    /// Vuelve a la página anterior
    /// </summary>

    public void PreviousPage()
    {

        if (currentPage > 0)
        {
            
            currentPage--;
            RefreshTable();
        }
    }
}