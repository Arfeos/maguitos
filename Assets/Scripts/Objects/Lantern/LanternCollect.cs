using UnityEngine;
using UnityEngine.UIElements;

public class LanternCollect : MonoBehaviour, ICollectable
{
    public void Collect()
    {
        var Player = FindAnyObjectByType<PlayerController>();
        if (Player != null)
        {
            Transform lanternSocket = Player.GetComponentInChildren<lanternsocket>().gameObject.transform;
            if (lanternSocket != null)
            {
                
                gameObject.transform.parent = lanternSocket.transform;
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.localRotation = Quaternion.Euler(70.31f, -11.6f, 0);
            }
            
        }
    }
}
