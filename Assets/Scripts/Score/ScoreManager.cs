using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Clase que se encarga de registrar las puntuaciones en un archibo json
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public ScoreTable scoreTable;

    private string filePath;

    private IScoreService _scoreService;
    private IProfileService _profileService;
    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "Scores.json");
        Debug.Log("Archivo JSON en: " + filePath);
        _scoreService = AppContainer.Get<IScoreService>();
        _profileService = AppContainer.Get<IProfileService>();
        LoadScores();
    }

    /// <summary>
    /// Agrega la puntuacion a la lista de puntuaciones y la ordena, después guarda la lista en el json.
    /// </summary>
    public void AddScore(bool pacifist)
    {
        string profile = _profileService.getSelectedProfile().name;
        scoreTable.scores.Add(new ScoreEntry(profile, _scoreService.GetPoints(profile), pacifist));

        scoreTable.scores = scoreTable.scores.OrderByDescending(s => s.score).ToList();

        SaveScores();
    }

    /// <summary>
    /// Guarda la lista en el json
    /// </summary>
    public void SaveScores()
    {
        string json = JsonUtility.ToJson(scoreTable,true);
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// carga la lista
    /// </summary>
    public void LoadScores()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            scoreTable = JsonUtility.FromJson<ScoreTable>(json);
        }else
        {
            scoreTable = new ScoreTable();
        }
    }
}
