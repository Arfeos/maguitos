using UnityEngine;

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

    [Header("Center")]
    [SerializeField] private float crouchCenter = 0.5f;
    [SerializeField] private float standCenter = 1f;

    [Header("Jump")]
    [SerializeField]   private float jumpHeight = 1f;
    private bool isJumping;

    private CharacterController characterController;
    private float gravity = -9.8f;
    private float velocityY;
    private float previusVelocity;
    private bool isRunning;
    private bool isCrouching;

    private Vector2 dirAnimation;

    private float xRotation = 0f;


    private IEventService _eventService;

    private Animator _animator;

    private void Awake()
    {
        PlayerInputManager.SwitchControlMap(PlayerInputManager.ControlMap.Player);
        characterController = GetComponent<CharacterController>();
        _eventService = AppContainer.Get<IEventService>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _animator = GetComponent<Animator>(); 
    }

    private void Update()
    {
        LookMouse();   
        Move();        
        HandleCrouch();
        handleReload();
        handleChangeWeapon();
        SetAnimation();
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
        dirAnimation = inputPlayer;

        if (characterController.isGrounded && velocityY < 0)
        {
            velocityY = -2f;
        }
        if (PlayerInputManager.Actions.Player.Jump.WasPressedThisFrame() && characterController.isGrounded)
        {
            velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else if (!characterController.isGrounded)
        {
            velocityY += gravity * Time.deltaTime;
        }

        var move = Vector3.zero;

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
        float targetCenter = isCrouching ? crouchCenter : standCenter;

        characterController.height = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * crouchSpeed);
        characterController.center = new Vector3(0, Mathf.Lerp(characterController.center.y, targetCenter, Time.deltaTime * crouchSpeed), 0);


        Vector3 camPos = cameraTransform.localPosition;
        float targetY = isCrouching ? (crouchHeight)+0.3f : (standHeight) - 0.3f;
        

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

    private void SetAnimation()
    {if (!characterController.isGrounded)
        {
            if(!isJumping)
                _animator.SetBool("onAir", true);
            isJumping = true;
        }
        else
        {
            isJumping = false;
                _animator.SetBool("onAir", false);
        }
        
        if (dirAnimation == Vector2.zero)
        {
            _animator.SetBool("isIdle", true);
        }
        else
        {
            _animator.SetBool("isIdle", false);
            _animator.SetFloat("VelocityX", dirAnimation.x);
            _animator.SetFloat("VelocityY", dirAnimation.y);
        }
        _animator.SetBool("isCrouching", isCrouching);
        _animator.SetBool("isRunning", isRunning);
        if (velocityY > 0.5)
        {
            _animator.SetBool("onAir", true);
        }

    }
}