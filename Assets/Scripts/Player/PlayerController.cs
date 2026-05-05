using Unity.VisualScripting;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float Velocity = 10f;

    [Header("Mouse")]
    [SerializeField] private float mouseSensitivity = 0.5f;
    [SerializeField] private Transform cameraTransform;

    [Header("Crouch")]
    [SerializeField] private float standHeight = 1f;
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float crouchSpeed = 8f;

    private CharacterController characterController;
    private float gravity = -9.8f;
    private float velocityY;
    private float previusVelocity;
    private bool isRunning;
    private bool isCrouching;

    private float xRotation = 0f;


    private IEventService _eventService;

    private void Awake()
    {
        PlayerInputManager.SwitchControlMap(PlayerInputManager.ControlMap.Player);
        characterController = GetComponent<CharacterController>();
        _eventService = AppContainer.Get<IEventService>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        LookMouse();   
        Move();        
        HandleCrouch();
        handleReload();
        handleChangeWeapon();
    }

    private void LookMouse()
    {
        Vector2 mouseInput = PlayerInputManager.Actions.Player.Look.ReadValue<Vector2>();

        float mouseX = mouseInput.x * mouseSensitivity;
        float mouseY = mouseInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);    
        transform.Rotate(Vector3.up * mouseX);
    }

    private void Move()
    {
        var inputPlayer = PlayerInputManager.Actions.Player.Move.ReadValue<Vector2>();

        if (characterController.isGrounded && velocityY < 0)
        {
            velocityY = -2f;
        }

        velocityY += gravity * Time.deltaTime;

        Vector3 move = Vector3.zero;

        if (inputPlayer.magnitude > 0.1f)
        {
       

            move = transform.forward * inputPlayer.y + transform.right * inputPlayer.x;
        }

        move.y = velocityY;

        if (PlayerInputManager.Actions.Player.Sprint.IsPressed())
        {
            if (!isRunning)
            {
                previusVelocity = Velocity;
                Velocity *= 2;
                isRunning = true;
            }
        }
        else
        {
            if (isRunning)
            {
                Velocity = previusVelocity;
                isRunning = false;
            }
        }

        characterController.Move(move * Velocity * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        if (PlayerInputManager.Actions.Player.Crouch.WasPressedThisFrame())
        {
            isCrouching = !isCrouching;
        }

        float targetHeight = isCrouching ? crouchHeight : standHeight;

        characterController.height = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * crouchSpeed);

        Vector3 camPos = cameraTransform.localPosition;
        float targetY = isCrouching ? (crouchHeight/2) - 0.2f : (standHeight/2) - 0.2f;

        camPos.y = Mathf.Lerp(camPos.y, targetY, Time.deltaTime * crouchSpeed);
        cameraTransform.localPosition = camPos;
    }


    public void handleReload()
    {
        if (PlayerInputManager.Actions.Player.Reload.WasPressedThisFrame())
        {
            ReloadEvent reloadEvent = new ReloadEvent();
            _eventService.Publish(reloadEvent);
        }
    }

    public void handleChangeWeapon()
    {
        if (PlayerInputManager.Actions.Player.Next.WasPressedThisFrame())
        {
            SpellChangeEvent reloadEvent = new SpellChangeEvent();
            reloadEvent.cambio = 1;
            _eventService.Publish(reloadEvent);
        }
        if (PlayerInputManager.Actions.Player.Previous.WasPressedThisFrame())
        {
            SpellChangeEvent reloadEvent = new SpellChangeEvent();
            reloadEvent.cambio = -1;
            _eventService.Publish(reloadEvent);
        }
    }
}