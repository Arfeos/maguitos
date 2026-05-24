using UnityEngine;
/// <summary>
/// Componente de Unity encargado de eliminar objetos que entran en una zona determinada mediante colisiones tipo Trigger
/// </summary>
public class DestroyZone : MonoBehaviour
{
    /// <summary>
    /// Se ejecuta automáticamente cuando otro objeto con un Collider entra en la zona Trigger. Destruye el objeto detectado utilizando Destroy() sobre su gameObject, eliminándolo de la escena
    /// </summary>
    /// <param name="other">Referencia al Collider del objeto que ha entrado en la zona Trigger. Permite acceder a su gameObject y a sus componentes asociados</param>
    private void OnTriggerEnter(Collider other)
    {
        Object.Destroy(other.gameObject);
    }
}
