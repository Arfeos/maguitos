using UnityEngine;
/// <summary>
/// Esta clase sirve para controlar los botones de las puertas
/// </summary>
public class Opendoor : MonoBehaviour, IHittable
{
    /// <summary>
    /// El valor del estado de la puerta se almacena en el boton y al ser golpeada cambia de estado
    /// </summary>
    [SerializeField] private GameObject PuertaAAbrir;
    private Animator _animatorPuerta;
    private bool _estatusPuerta = false;
    
    
    public void Hit()
    {
        _estatusPuerta = !_estatusPuerta;
        _animatorPuerta.SetBool("Open", _estatusPuerta);
    }

    private void Awake()
    {
        _animatorPuerta = PuertaAAbrir.GetComponent<Animator>();
    }
}
