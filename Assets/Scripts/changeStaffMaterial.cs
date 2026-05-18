using UnityEngine;


public class changeStaffMaterial : MonoBehaviour
{
    private ICharacterService _characterService;
    private Renderer _materialRenderer;


    private void Start()
    {
        _characterService = AppContainer.Get<ICharacterService>();
        _materialRenderer = GetComponent<Renderer>();
    }
    void Update()
    {
        SpellBase ActualSpell = _characterService.getSpell(_characterService.getIndex())?.GetComponent<SpellBase>();
        if (_materialRenderer != null && ActualSpell != null)
        {
            
       
                _materialRenderer.material = ActualSpell.spell.RayMaterial[0];
            
        }
        else Debug.Log("HExa has no renderer puta");
    }
}
