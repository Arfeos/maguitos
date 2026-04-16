using UnityEngine;

public interface ISpellService
{
    GameObject ShootRay(GameObject Ray);
    void DestroyRayObjects();
    public void ReturnRay(GameObject ray);
}
