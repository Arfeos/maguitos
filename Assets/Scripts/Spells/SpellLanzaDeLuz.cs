using UnityEngine;

public class SpellLanzaDeLuz : SpellBase
{
    private  void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void LanzarHechizo(Transform spellSpawn, GameObject spell, LayerMask layersToHit)
    {
        base.LanzarHechizo(spellSpawn,spell,layersToHit);
        ProducesLine = true;
        Debug.Log("Lanza de Luz: " + ProducesLine);
    }
}
