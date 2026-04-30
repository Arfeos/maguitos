using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public interface ISpellService
{
    GameObject ShootRay(Vector3 start, Vector3 end);
    GameObject ShootRay(Vector3 start, Vector3 end, List<Material> listamateriales);
    void DestroyRayObjects();
    public void ReturnRay(GameObject ray);
}
