using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Representa una entrada individual en la tabla de puntuaciones,
/// almacenando el nombre del jugador, su puntuación y si completó la partida en modo pacifista.
/// Serializable para su persistencia en JSON.
/// </summary>
[System.Serializable]
public class ScoreEntry
{
    /// <summary>
    /// Nombre del perfil del jugador asociado a esta entrada.
    /// </summary>
    public string playerName;

    /// <summary>
    /// Puntuación obtenida por el jugador en la partida.
    /// </summary>
    public int score;

    /// <summary>
    /// Indica si el jugador completó la partida en modo pacifista.
    /// </summary>
    public bool pacifist;

    // <summary>
    /// Crea una nueva entrada de puntuación con los datos de la partida finalizada.
    /// </summary>
    /// <param name="name">Nombre del perfil del jugador.</param>
    /// <param name="score">Puntuación obtenida en la partida.</param>
    /// <param name="pacifist">Si el jugador completó la partida en modo pacifista.</param>
    public ScoreEntry(string name, int score, bool pacifist)
    {
        playerName = name;
        this.score = score;
        this.pacifist = pacifist;
    }
}
