using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
/// <summary>
/// Componente de Unity encargado de gestionar la lógica relacionada con los espacios o slots disponibles dentro de un libro de hechizos. En este fragmento incluye una enumeración interna utilizada para definir distintos modos de juego disponibles.
/// </summary>
public class SlotsSpellBook : MonoBehaviour
{
    /// <summary>
    /// Enumeración interna utilizada para representar los distintos modos de juego disponibles dentro del sistema de <see cref="SlotsSpellBook"/>
    /// </summary>
    private enum GameMode
    {
        allvsall,
        knowledgerun,
        friendly
    }
    
}
