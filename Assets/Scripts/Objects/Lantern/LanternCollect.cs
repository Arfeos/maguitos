using UnityEngine;

public class LanternCollect : MonoBehaviour, ICollectable
{
    public void Collect()
    {
        var Player = FindAnyObjectByType<PlayerController>();
        if (Player != null)
        {
            gameObject.transform.parent = Player.transform;
        }
    }
}
