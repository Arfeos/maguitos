using UnityEngine;

public interface ISpellService
{
    GameObject ShootRay(Vector3 start, Vector3 end);
    void DestroyRayObjects();
    public void ReturnRay(GameObject ray);
}
