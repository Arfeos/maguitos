using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellService : ISpellService
{
    private readonly GameObject _rayObjectPrefab;
    private List<GameObject> _rayList = new List<GameObject>();
    private readonly GameObject _rayPrefab;

    public SpellService(GameObject rayPrefab)
    {
        _rayObjectPrefab = new GameObject("SpellService");
        Object.DontDestroyOnLoad(_rayObjectPrefab);
        _rayPrefab = rayPrefab;
    }
    //public GameObject ShootRay(GameObject Ray)
    //{
    //    if (Ray == null) return null;
    //    //no hace falta comprobar si el rayo ya existe, porque cada rayo es único y se inactiva al finalizar su animación
    //    var RayObject = GetOrCreateRay();
    //    RayObject = Ray;
    //    return RayObject;
    //}
    public GameObject ShootRay(Vector3 start, Vector3 end)
    {
        var ray = GetOrCreateRay();

        var line = ray.GetComponent<LineRenderer>();

        line.SetPosition(0, start);
        line.SetPosition(1, end);

        ray.SetActive(true);

        return ray;
    }
    private GameObject GetOrCreateRay()
    {
        GameObject Ray = _rayList.FirstOrDefault(r => !r.activeInHierarchy);

        if (Ray == null)
        {
            Ray = Object.Instantiate(_rayObjectPrefab);
            _rayList.Add(Ray);
        }

        return Ray;
    }
    public void ReturnRay(GameObject ray)
    {
        ray.SetActive(false);
    }

    //este metodo se llamara para eliminar la pool de rayos al salir de un mapa o al cerrar el juego, para evitar que se acumulen objetos en la escena
    public void DestroyRayObjects()
    {
        foreach (var ray in _rayList)
        {
            Object.Destroy(ray);
        }
        _rayList.Clear();
    }
}
