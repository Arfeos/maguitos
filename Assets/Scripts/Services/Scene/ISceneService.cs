using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Servicio encargado de gestionar la carga y navegación entre escenas del juego.
/// Soporta transiciones con efecto de fade y navegación hacia la escena anterior.
/// </summary>
public interface ISceneService
{
    /// <summary>
    /// Carga la escena indicada, guardando la escena actual en el historial para poder volver a ella.
    /// </summary>
    /// <param name="scene">Nombre de la escena destino definido en el enum <see cref="SceneNames"/>.</param>
    void LoadScene(SceneNames scene);

    /// <summary>
    /// Navega a la última escena visitada, extrayéndola del historial.
    /// Si no hay escenas en el historial, no realiza ninguna acción.
    /// </summary>
    void GoBack();
}
