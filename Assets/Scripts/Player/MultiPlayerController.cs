using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
public class MultiPlayerController : NetworkBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float Velocity = 10f;

    [Header("Mouse")]
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform BodyTransform;

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

    public NetworkVariable<float> pitch = new(
    0f,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner);

    public NetworkVariable<float> yaw = new(0f,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        characterController = GetComponent<CharacterController>();
        Debug.Log($"[Player] OnNetworkSpawn - IsOwner: {IsOwner} - Position: {transform.position}");

        if (!IsOwner)
        {
            playerCamera.enabled = false;
            characterController.enabled = false;
            var audioListener = playerCamera.GetComponent<AudioListener>();
            if (audioListener != null) audioListener.enabled = false;
            return;
        }

        PlayerInputManager.SwitchControlMap(PlayerInputManager.ControlMap.Player);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void Update()
    {
        if (IsOwner)
        {
            LookMouse();
            Move();
            HandleCrouch();
        }
        else
        {
            ApplyRemoteRotation();
        }
    }

    private void LookMouse()
    {
        Vector2 mouseInput = PlayerInputManager.Actions.Player.Look.ReadValue<Vector2>();

        xRotation -= mouseInput.y * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.Rotate(Vector3.up * mouseInput.x * mouseSensitivity);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        pitch.Value = xRotation;
        yaw.Value = transform.eulerAngles.y;
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

        Vector3 camPos = playerCamera.transform.localPosition;
        float targetY = isCrouching ? (crouchHeight/2) - 0.2f : (standHeight/2) - 0.2f;

        camPos.y = Mathf.Lerp(camPos.y, targetY, Time.deltaTime * crouchSpeed);
        playerCamera.transform.localPosition = camPos;
    }
    private void ApplyRemoteRotation()
    {
        // Aplica yaw (horizontal) al cuerpo
        transform.rotation = Quaternion.Euler(pitch.Value, yaw.Value, 0f);
        // No aplicamos pitch aquí porque la cámara remota está desactivada
        // Si tuvieras un hueso de cabeza o cuello podrías rotarlo aquí
    }
}