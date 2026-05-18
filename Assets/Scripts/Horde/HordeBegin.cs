using UnityEngine;

public class HordeBegin : MonoBehaviour
{
    [SerializeField] GameObject spawn;
    [SerializeField] WaveManager WaveManger;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            if (spawn == null) return;
            CharacterController cc = other.GetComponent<CharacterController>();

            if (cc != null)
                cc.enabled = false;

            other.transform.position = spawn.transform.position;

            if (cc != null)
                cc.enabled = true;
            WaveManger.beginHorde();
        }

    }
}
