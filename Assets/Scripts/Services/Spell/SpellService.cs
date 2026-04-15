using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellService : ISpellService
{
    private readonly GameObject _rayObjectPrefab;
    private List<GameObject> _rayList = new List<GameObject>();

    public SpellService()
    {
        _rayObjectPrefab = new GameObject("SpellService");
        Object.DontDestroyOnLoad(_rayObjectPrefab);
    }
    public void CreateRay(GameObject Ray)
    {
        if (Ray == null) return;
        GameObject existingRay = _rayList.FirstOrDefault<GameObject>();

        if (existingRay != null)
        {
            //existingRay.Stop();
            //existingRay.Play();
            return;
        }
        var audioSource = GetOrCreateRay();
        //audioSource.clip = clip;
        //audioSource.loop = loop;
        //audioSource.Play();
    }
    private GameObject GetOrCreateRay()
    {
        GameObject Ray = _rayList.FirstOrDefault<GameObject>();

        if (Ray == null)
        {
            Ray = _rayObjectPrefab;
            _rayList.Add(Ray);
        }

        return Ray;
    }
    public void DestroyRay()
    {
        foreach (var ray in _rayList)
        {
            Object.Destroy(ray);
        }
        _rayList.Clear();
    }
}
