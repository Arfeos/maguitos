using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contenedor serializable que agrupa la lista de entradas de puntuaciones.
/// Se usa para serializar y deserializar el historial completo de puntuaciones en JSON.
/// </summary>
[System.Serializable]
public class ScoreTable
{
    /// <summary>
    /// Lista de entradas de puntuación ordenadas de mayor a menor.
    /// Cada entrada representa el resultado de una partida completada.
    /// </summary>
    public List<ScoreEntry> scores = new List<ScoreEntry>();

}
