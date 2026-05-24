using System;

/// <summary>
/// Define el contrato para objetos que pueden ser recogidos por el jugador.
/// </summary>
internal interface ICollectable
{
    /// <summary>
    /// Ejecuta la lógica de recogida del objeto (equiparlo, aplicar efecto, destruirlo, etc.).
    /// </summary>
    void Collect();
}