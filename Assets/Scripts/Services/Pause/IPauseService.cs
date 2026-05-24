using UnityEngine;

/// <summary>
/// Implementación de <see cref="IPauseService"/> que gestiona los paneles de pausa y ajustes.
/// Busca o instancia el panel de pausa en el canvas 2D de la escena activa,
/// controla el <see cref="Time.timeScale"/> y cambia el mapa de control del jugador al pausar o reanudar.
/// </summary>
public interface IPauseService
{

    /// <summary>
    /// Alterna el estado del panel de pausa. Si no existe una instancia en la escena activa,
    /// la crea en el primer canvas 2D encontrado. Al pausar detiene el tiempo, libera el cursor
    /// y cambia al mapa de control UI; al reanudar restaura el tiempo, bloquea el cursor
    /// y vuelve al mapa de control del jugador.
    /// </summary>
    public void TogglePause();

    /// <summary>
    /// Alterna la visibilidad del panel de ajustes. Si no existe instancia, la crea y oculta el panel
    /// de pausa. Si ya existe, alterna entre mostrar ajustes (ocultando pausa) y mostrar pausa
    /// (ocultando ajustes).
    /// </summary>
    public void ToggleSettings();
}
