using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public interface ISpellService
{
    GameObject ShootRay(Vector3 start, Vector3 end);
    public GameObject ShootBall(Vector3 start, Vector3 direction, float velocity, List<Material> material);
    GameObject ShootRay(Vector3 start, Vector3 end, List<Material> listamateriales);
    void DestroyRayObjects();
    public void DestroyBallObjects();
    public void ReturnRay(GameObject ray);
    public void ReturnBall(GameObject ball);
}
