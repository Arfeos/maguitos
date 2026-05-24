using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// MonoBehaviour encargado de gestionar el registro de puntuaciones en un archivo JSON.
/// Actúa como puente entre el <see cref="IScoreService"/>, el <see cref="IProfileService"/>
/// y la persistencia de datos en disco.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    /// <summary>
    /// Tabla de puntuaciones que se serializa y deserializa desde el archivo JSON.
    /// </summary>
    public ScoreTable scoreTable;

    /// <summary>
    /// Ruta completa al archivo <c>Scores.json</c> dentro de <see cref="Application.persistentDataPath"/>.
    /// </summary>
    private string filePath;

    /// <summary>
    /// Referencia al servicio de puntuación, usado para obtener los puntos del perfil activo.
    /// </summary>
    private IScoreService _scoreService;

    /// <summary>
    /// Referencia al servicio de perfiles, usado para obtener el perfil seleccionado actualmente.
    /// </summary>
    private IProfileService _profileService;

    /// <summary>
    /// Inicializa las referencias a servicios, construye la ruta del archivo JSON
    /// y carga las puntuaciones almacenadas al arrancar el componente.
    /// </summary>
    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "Scores.json");
        Debug.Log("Archivo JSON en: " + filePath);
        _scoreService = AppContainer.Get<IScoreService>();
        _profileService = AppContainer.Get<IProfileService>();
        LoadScores();
    }

    /// <summary>
    /// Registra la puntuación del perfil activo en la tabla de puntuaciones,
    /// la ordena de mayor a menor y persiste el resultado en el archivo JSON.
    /// </summary>
    /// <param name="pacifist">Indica si el jugador completó la partida en modo pacifista.</param>
    public void AddScore(bool pacifist)
    {
        string profile = _profileService.getSelectedProfile().name;
        scoreTable.scores.Add(new ScoreEntry(profile, _scoreService.GetPoints(profile), pacifist));

        scoreTable.scores = scoreTable.scores.OrderByDescending(s => s.score).ToList();

        SaveScores();
    }

    /// <summary>
    /// Serializa la tabla de puntuaciones actual y la escribe en el archivo
    /// <c>Scores.json</c> en <see cref="Application.persistentDataPath"/>.
    /// </summary>
    public void SaveScores()
    {
        string json = JsonUtility.ToJson(scoreTable,true);
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// Carga la tabla de puntuaciones desde el archivo <c>Scores.json</c>.
    /// Si el archivo no existe, inicializa una tabla vacía.
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
