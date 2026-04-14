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
    public override void LanzarHechizo()
    {
        base.LanzarHechizo();
        ProducesLine = true;
        Debug.Log("Lanza de Luz: " + ProducesLine);
    }
}
