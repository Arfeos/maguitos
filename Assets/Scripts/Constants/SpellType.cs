/// <summary>
/// Clase parcial encargada de definir la estructura base de un hechizo. Este fragmento contiene una enumeración utilizada para clasificar los distintos tipos de hechizos disponibles dentro del sistema
/// </summary>
public partial class SpellBase
{
#nullable enable
    /// <summary>
    /// Enumeración utilizada para identificar el comportamiento o categoría funcional de un objeto <see cref="SpellBase"/>
    /// </summary>
    public enum SpellType { 
        ray,
        ball,
        buff,
        structure
    }
}
