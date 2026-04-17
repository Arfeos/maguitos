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
    public override void LanzarHechizo(Transform spellSpawn, SpellBase spell, LayerMask layersToHit)
    {
        base.LanzarHechizo(spellSpawn,spell,layersToHit);
    }
}
