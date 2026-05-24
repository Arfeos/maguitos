using System;
/// <summary>
/// Clase parcial encargada de definir la estructura base de un hechizo. Contiene elementos comunes utilizados por distintas implementaciones relacionadas con el sistema de magia
/// </summary>
public partial class SpellBase
{

    /// <summary>
    /// Enumeración utilizada para definir los distintos tipos de lanzamiento que puede tener un hechizo
    /// </summary>
    public enum CastType
    {
        auto,
        semi,
        charged
    }
}
