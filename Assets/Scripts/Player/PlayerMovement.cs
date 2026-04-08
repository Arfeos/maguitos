using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;
    void Start()
    {
        if (IsOwner)
        {
            InputManager.SwitchMap(InputManager.Actions.Player);
            GetComponent<Renderer>().material.color = Color.red;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;

        if (!InputManager.Actions.Player.Move.IsPressed()) return;
        Vector2 Movement = InputManager.Actions.Player.Move.ReadValue<Vector2>();
        Debug.Log(Movement);
        Vector3 direction = new Vector3(Movement.x, 0f, Movement.y).normalized;
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
