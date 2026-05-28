using System.Collections.Generic;
using UnityEngine;

public interface IScoreService
{
    /// <summary>
    /// Devuelve la tabla de puntuaciones, cargándola desde disco si aún no ha sido inicializada.
    /// </summary>
    /// <returns>La <see cref="ScoreTable"/> con el historial de puntuaciones.</returns>
    public ScoreTable getScoreTable();
    /// <summary>
    /// Devuelve la puntuación actual del jugador indicado.
    /// Si el jugador no existe en la lista, lo registra con 0 puntos y devuelve 0.
    /// </summary>
    /// <param name="player">Nombre del jugador cuya puntuación se quiere consultar.</param>
    /// <returns>Puntuación actual del jugador.</returns>
    public int GetPoints(string player);
    /// <summary>
    /// Resta los puntos indicados al jugador designado.
    /// Si el jugador no existe en la lista, lo registra con puntuación negativa.
    /// </summary>
    /// <param name="player">Nombre del jugador al que se le restan los puntos.</param>
    /// <param name="points">Cantidad de puntos a restar.</param>
    public void removePoints(string player, int points);

    /// <summary>
    /// Agrega los puntos indicados al jugador designado.
    /// Si el jugador no existe en la lista, lo registra con la puntuación indicada.
    /// </summary>
    /// <param name="player">Nombre del jugador al que se le suman los puntos.</param>
    /// <param name="points">Cantidad de puntos a añadir.</param>
    public void addPoints(string player, int points);

    /// <summary>
    /// Reinicia la lista de puntuaciones de la partida actual,
    /// eliminando todos los jugadores y sus puntuaciones registradas.
    /// </summary>
    public void resetScore();


    /// <summary>
    /// Registra la puntuación del perfil activo en la tabla de puntuaciones globales,
    /// la ordena de mayor a menor y guarda el resultado en el archivo JSON.
    /// </summary>
    public void AddScore();
}
