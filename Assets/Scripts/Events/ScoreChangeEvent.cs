/// <summary>
/// Evento que se lanza cuando la puntuación del jugador cambia.
/// Contiene la cantidad de puntos involucrados en el cambio.
/// </summary>
public class ScoreChangeEvent : GameEventBase
{
    /// <summary>
    /// Cantidad de puntos añadidos o restados en el cambio de puntuación.
    /// </summary>
    public int points;
}
