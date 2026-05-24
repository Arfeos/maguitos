using UnityEngine;

/// <summary>
/// Define el contrato para objetos que pueden recibir daño.
/// </summary>
internal interface IHittable
{
    /// <summary>
    /// Aplica una cantidad de daño al objeto.
    /// </summary>
    /// <param name="damage">Cantidad de daño a recibir.</param>
    public void Hit(float damage);
}