using UnityEngine;


/// <summary>
/// Componente que actualiza el material del objeto en cada frame
/// para reflejar visualmente el hechizo actualmente seleccionado por el jugador.
/// </summary>
public class changeStaffMaterial : MonoBehaviour
{
    /// <summary>
    /// Referencia al servicio de personaje, usado para obtener el hechizo activo.
    /// </summary>
    private ICharacterService _characterService;

    /// <summary>
    /// Renderer del objeto al que se le aplicará el material del hechizo activo.
    /// </summary>
    private Renderer _materialRenderer;

    /// <summary>
    /// Obtiene las referencias necesarias al inicio del ciclo de vida del componente.
    /// </summary>
    private void Start()
    {
        _characterService = AppContainer.Get<ICharacterService>();
        _materialRenderer = GetComponent<Renderer>();
    }

    /// <summary>
    /// Actualiza el material del <see cref="Renderer"/> cada frame según el hechizo
    /// actualmente seleccionado, usando el primer material de rayo definido en el hechizo.
    /// No realiza ningún cambio si el renderer o el hechizo activo son nulos.
    /// </summary>
    void Update()
    {
        SpellBase ActualSpell = _characterService.getSpell(_characterService.getIndex())?.GetComponent<SpellBase>();
        if (_materialRenderer != null && ActualSpell != null)
        {
            
       
                _materialRenderer.material = ActualSpell.spell.RayMaterial[0];
            
        }
        
    }
}
