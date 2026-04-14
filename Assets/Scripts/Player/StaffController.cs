using System;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Configuracion de hechizo")]
    [SerializeField] private GameObject spell;
    [SerializeField] private Transform spellSpawn;

    [Header("Configuracion de Objetos")]
    [SerializeField] private LayerMask layersToHit;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float isAtacking = PlayerInputManager.Actions.Player.Attack.ReadValue<float>();
        if (isAtacking > 0.1)
        {
            Debug.Log("Atacando");
            LanzarHechizo();
        }
    }

    private void LanzarHechizo()
    {
        SpellBase ActualSpell = spell.GetComponent<SpellBase>();
        ActualSpell.LanzarHechizo(spellSpawn, spell, layersToHit);
    }
}
