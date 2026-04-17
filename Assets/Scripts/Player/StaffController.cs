using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static SpellBase;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Configuracion de hechizo")]
    [SerializeField] private SpellBase[] spellList;
    [SerializeField] private SpellBase Actualspell;
    [SerializeField] private Transform spellSpawn;

    [Header("Configuracion de Objetos")]
    [SerializeField] private LayerMask layersToHit;
    [Header("prueba sonido")]
    [SerializeField] private AudioClip _audioClip;
    private IAudioService _audioService;
    void Start()
    {
        
        PlayerInputManager.Actions.Player.Reload.started += OnReloadStarted;
        _audioService = AppContainer.Get<IAudioService>();
    }

    private void OnReloadStarted(InputAction.CallbackContext context)
    {
        SpellBase ActualSpell = Actualspell.GetComponent<SpellBase>();
        ActualSpell.Invoke( "Reload", ActualSpell.ReloadTime);
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: Esto es terrible, hacer un evento
        SpellBase ActualSpell = Actualspell.GetComponent<SpellBase>();

        switch (ActualSpell.castType)
        {
            case CastType.auto:
                if (PlayerInputManager.Actions.Player.Attack.IsPressed()) LanzarHechizo(ActualSpell);
                break;
            case CastType.semi:
                if (PlayerInputManager.Actions.Player.Attack.WasPressedThisFrame()) LanzarHechizo(ActualSpell);
                break;
        }
    }



    private void LanzarHechizo(SpellBase ActualSpell)
    {
        if (_audioService != null) {
            _audioService.PlaySound(_audioClip, false);
        }
        
        ActualSpell.LanzarHechizo(spellSpawn, ActualSpell, layersToHit);
    }
}
