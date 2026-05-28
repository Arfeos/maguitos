using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Servicio encargado de gestionar la puntuación de los jugadores durante la partida,
/// así como el guardado y carga del historial de puntuaciones en un archivo JSON.
/// </summary>
public class ScoreService : IScoreService
{
    // <summary>
    /// Diccionario que almacena la puntuación actual de cada jugador durante la partida,
    /// usando el nombre del jugador como clave y su puntuación como valor.
    /// </summary>
    public Dictionary<string, int> PlayerList = new Dictionary<string, int>();

    /// <summary>
    /// Tabla de puntuaciones persistente que se serializa y deserializa desde el archivo JSON.
    /// </summary>
    public ScoreTable scoreTable;



    /// <summary>
    /// Agrega los puntos indicados al jugador designado.
    /// Si el jugador no existe en la lista, lo registra con la puntuación indicada.
    /// </summary>
    /// <param name="player">Nombre del jugador al que se le suman los puntos.</param>
    /// <param name="points">Cantidad de puntos a añadir.</param>
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
            Debug.Log("Player: " + player + " has now " + PlayerList[player] + " points.");
        }
    }

    /// <summary>
    /// Devuelve la puntuación actual del jugador indicado.
    /// Si el jugador no existe en la lista, lo registra con 0 puntos y devuelve 0.
    /// </summary>
    /// <param name="player">Nombre del jugador cuya puntuación se quiere consultar.</param>
    /// <returns>Puntuación actual del jugador.</returns>
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
    /// Resta los puntos indicados al jugador designado.
    /// Si el jugador no existe en la lista, lo registra con puntuación negativa.
    /// </summary>
    /// <param name="player">Nombre del jugador al que se le restan los puntos.</param>
    /// <param name="points">Cantidad de puntos a restar.</param>
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
    /// Reinicia la lista de puntuaciones de la partida actual,
    /// eliminando todos los jugadores y sus puntuaciones registradas.
    /// </summary>
    public void resetScore()
    {
        PlayerList = new Dictionary<string, int>();
    }


    /// <summary>
    /// Registra la puntuación del perfil activo en la tabla de puntuaciones globales,
    /// la ordena de mayor a menor y guarda el resultado en el archivo JSON.
    /// </summary>
    public void AddScore()
    {
        LoadScores();
        string profile = AppContainer.Get<IProfileService>().getSelectedProfile().name;
        scoreTable.scores.Add(new ScoreEntry(profile, GetPoints(AppContainer.Get<IProfileService>().getSelectedProfile().guid), AppContainer.Get<ICharacterService>().getPacifist()));

        scoreTable.scores = scoreTable.scores.OrderByDescending(s => s.score).ToList();

        SaveScores();
    }

    /// <summary>
    /// Serializa la tabla de puntuaciones actual y la guarda en el archivo
    /// <c>Scores.json</c> dentro de <see cref="Application.persistentDataPath"/>.
    /// </summary>
    private void SaveScores()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "Scores.json");
        Debug.Log(Path.Combine(Application.persistentDataPath, "Scores.json"));
        string json = JsonUtility.ToJson(scoreTable,true);
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// Carga la tabla de puntuaciones desde el archivo <c>Scores.json</c>.
    /// Si el archivo no existe, inicializa una tabla vacía.
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

    /// <summary>
    /// Devuelve la tabla de puntuaciones, cargándola desde disco si aún no ha sido inicializada.
    /// </summary>
    /// <returns>La <see cref="ScoreTable"/> con el historial de puntuaciones.</returns>
    public ScoreTable getScoreTable()
    {
        if (scoreTable == null) LoadScores();
        return scoreTable;
    }
}
