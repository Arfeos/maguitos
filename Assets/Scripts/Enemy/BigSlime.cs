using System;
using UnityEngine;

public class BigSlime : BasicSlimeController
{
    [SerializeField] private GameObject limoChild;
    [SerializeField] private int numberOfChilds;

    protected override void OnDeath()
    {
        base.OnDeath();
        spawnChilds(numberOfChilds);
    }

    private void spawnChilds(int numberOfChilds)
    {
        if (numberOfChilds > 0)
        {
            //Vector3 transformSpawn = new Vector3 (transform.position)
            //Instantiate(limoChild,)
            numberOfChilds--;
        }
    }
}
