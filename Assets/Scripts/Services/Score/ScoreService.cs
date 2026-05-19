using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ScoreService : IScoreService
{
    public Dictionary<string, int> PlayerList = new Dictionary<string, int>();
    
    public ScoreTable scoreTable;

    /// <summary>
    /// Agrega los puntos indicados al jugador designado
    /// </summary>
    public void addPoints(string player, int points)
    {
        if (PlayerList.TryGetValue(player, out int playerPoints))
        {
            playerPoints += points;
            PlayerList[player] = playerPoints;
            Debug.Log("Player: " + player + " has now " + playerPoints + " points.");
        }
        else
        {
            PlayerList.Add(player, points);
            Debug.Log("Player: " + player + " has now " + playerPoints + " points.");
        }
    }

    /// <summary>
    /// Devuelve los puntos del jugador indicado
    /// </summary>
    public int GetPoints(string player)
    {
        if (PlayerList.TryGetValue(player, out int playerPoints)) return playerPoints;
        else
        {
            PlayerList.Add(player, 0);
            return 0;
        }
            
    }

    /// <summary>
    /// Quita los puntos indicados al jugador designado
    /// </summary>
    public void removePoints(string player, int points)
    {
        if (PlayerList.TryGetValue(player, out int playerPoints))
        {
            playerPoints -= points;
            PlayerList[player] = playerPoints;
        }
        else
        {
            PlayerList.Add(player, -points);
        }
    }

    /// <summary>
    /// Coge la lista de jugadores y la borra.
    /// </summary>
    public void resetScore()
    {
        PlayerList = new Dictionary<string, int>();
    }


   /// <summary>
    /// Agrega la puntuacion a la lista de puntuaciones y la ordena, después guarda la lista en el json.
    /// </summary>
    public void AddScore(bool pacifist)
    {
        LoadScores();
        string profile = AppContainer.Get<IProfileService>().getSelectedProfile().name;
        scoreTable.scores.Add(new ScoreEntry(profile, GetPoints(profile), pacifist));

        scoreTable.scores = scoreTable.scores.OrderByDescending(s => s.score).ToList();

        SaveScores();
    }

    /// <summary>
    /// Guarda la lista en el json
    /// </summary>
    private void SaveScores()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "Scores.json");
    string json = JsonUtility.ToJson(scoreTable,true);
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// carga la lista
    /// </summary>
    private void LoadScores()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "Scores.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            scoreTable = JsonUtility.FromJson<ScoreTable>(json);
        }else
        {
            scoreTable = new ScoreTable();
        }
    }

    public ScoreTable getScoreTable()
    {
        return scoreTable;
    }
}
