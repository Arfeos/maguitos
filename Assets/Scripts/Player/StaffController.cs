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
        Ray ray = new Ray(spellSpawn.position, Vector3.forward);
        SpellBase ActualSpell = spell.GetComponent<SpellBase>();
        RaycastHit hit;
        
        if(Physics.Raycast(spellSpawn.position, this.transform.TransformDirection(Vector3.forward), out hit, ActualSpell.LifeTime, layersToHit))
        {
            Debug.DrawRay(spellSpawn.position, this.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);

            Debug.Log("ObjetoGolpeado");
        }
        if (!ActualSpell.ProducesLine) ActualSpell.createLine(spellSpawn.position, ray, hit);
    }
}
