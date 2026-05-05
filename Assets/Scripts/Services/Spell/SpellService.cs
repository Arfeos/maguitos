using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellService : ISpellService
{
    private readonly GameObject _SpellServiceFather;
    private List<GameObject> _rayList = new List<GameObject>();
    private List<GameObject> _ballList = new List<GameObject>();
    private readonly GameObject _rayPrefab;
    private readonly GameObject _BallPrefab;
    public SpellService(GameObject rayPrefab , GameObject ballPrefab)
    {
        _SpellServiceFather = new GameObject("SpellService");
        Object.DontDestroyOnLoad(_SpellServiceFather);
        _rayPrefab = rayPrefab;
        _BallPrefab = ballPrefab;
    }

    //public GameObject ShootRay(GameObject Ball)
    //{
    //    if (Ball == null) return null;
    //    //no hace falta comprobar si el rayo ya existe, porque cada rayo es único y se inactiva al finalizar su animación
    //    var RayObject = GetOrCreateRay();
    //    RayObject = Ball;
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

    public GameObject ShootRay(Vector3 start, Vector3 end, List<Material> material)
    {
        var ray = GetOrCreateRay();

        var line = ray.GetComponent<LineRenderer>();

        line.SetPosition(0, start);
        line.SetPosition(1, end);
        
        line.SetMaterials(material);
        ray.SetActive(true);

        return ray;
    }
    public GameObject ShootBall(Vector3 start, Vector3 direction, float velocity, List<Material> material)
    {
        var ball = GetOrCreateBall();
        Rigidbody rb = ball.GetComponent<Rigidbody>();

        Debug.Log($"Ball: {ball.name} | Active: {ball.activeSelf} | RB null: {rb == null}");

        ball.transform.SetPositionAndRotation(start, Quaternion.identity);
        ball.SetActive(true);
        //Los objetos inactivos no tienen fisicas, quien lo diria verdad, el sergio de la ultima hora desde luego no
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
            rb.WakeUp();
            rb.AddForce(direction.normalized * velocity, ForceMode.Impulse);

        }

        ball.GetComponent<MeshRenderer>().materials = material.ToArray();
        
        return ball;
    }
    private GameObject GetOrCreateRay()
    {
        GameObject Ray = _rayList.FirstOrDefault(r => !r.activeInHierarchy);

        if (Ray == null)
        {
            Ray = Object.Instantiate(_rayPrefab, _SpellServiceFather.transform);
            _rayList.Add(Ray);
        }

        return Ray;
    }

    private GameObject GetOrCreateBall()
    {
        GameObject Ball = _ballList.FirstOrDefault(r => !r.activeInHierarchy);

        if (Ball == null)
        {
            Ball = Object.Instantiate(_BallPrefab, _SpellServiceFather.transform);
            _ballList.Add(Ball);
        }

        return Ball;
    }
    public void ReturnRay(GameObject ray)
    {
        ray.SetActive(false);
    }
    public void ReturnBall(GameObject ball)
    {
        ball.SetActive(false);
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
    public void DestroyBallObjects()
    {
        foreach (var ball in _ballList)
        {
            Object.Destroy(ball);
        }
        _ballList.Clear();
    }
}
