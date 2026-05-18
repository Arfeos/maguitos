using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEditor.MPE;
using UnityEditor.PackageManager;
using UnityEngine;
public class MultiPlayerController : NetworkBehaviour, IHittable
{
    public NetworkVariable<float> health = new(100f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    [Header("Movimiento")]
    [SerializeField] private float Velocity = 10f;
    [SerializeField] private AudioClip WalkSound;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 1f;
    private bool isJumping;

    [Header(" ")]
    [SerializeField] private float Sensitivity = 0.5f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private int xDirection = 1;
    [SerializeField] private int yDirection = 1;

    [Header("Mouse")]
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform BodyTransform;

    [Header("Crouch")]
    [SerializeField] private float standHeight = 1f;
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float crouchSpeed = 8f;

    [Header("Center")]
    [SerializeField] private float crouchCenter = 0.5f;
    [SerializeField] private float standCenter = 1f;

    private CharacterController characterController;
    private float gravity = -9.8f;
    private float velocityY;
    private float previusVelocity;
    private bool isRunning;
    private bool isCrouching;

    public NetworkVariable<Vector2> NetVelocity = new();
    public NetworkVariable<bool> IsRunning = new();
    public NetworkVariable<bool> IsCrouching = new();
    public NetworkVariable<bool> OnAir = new();


    private float xRotation = 0f;
    private Vector2 dirAnimation;

    public NetworkVariable<float> pitch = new(
    0f,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner);

    public NetworkVariable<float> yaw = new(0f,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner);

    private IPauseService _pauseService;
    private IEventService _eventService;
    private ICharacterService _characterService;

    private IProfileService _profileService;
    private IAudioService _audioService;
    private Animator _animator;
    private NetworkAnimator _networkAnimator;
    [SerializeField] private GameObject bodyModel;



    public override void OnNetworkSpawn()
    {
        characterController = GetComponent<CharacterController>();
        Debug.Log($"[Player] OnNetworkSpawn - IsOwner: {IsOwner} - Position: {transform.position}");

        PlayerInputManager.SwitchControlMap(PlayerInputManager.ControlMap.Player);
        characterController = GetComponent<CharacterController>();
        _eventService = AppContainer.Get<IEventService>();
        _characterService = AppContainer.Get<ICharacterService>();
        _audioService = AppContainer.Get<IAudioService>();
        _profileService = AppContainer.Get<IProfileService>();
        _pauseService = AppContainer.Get<IPauseService>();
        _eventService.Subscribe<PreferenceChangeEvent>(updatePreferences);
        _animator = GetComponent<Animator>();
        _networkAnimator = GetComponent<NetworkAnimator>();
        if (IsOwner)
        {
            foreach (var renderer in bodyModel.GetComponentsInChildren<Renderer>())
                renderer.enabled = false;
            updatePreferences();
        }
        

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
            Look();
            Move();
            HandleCrouch();
            handleReload();
            handleChangeWeapon();
            handlePause();
            SendAnimationDataToServer();
            SetAnimation();
        }
        else
        {
            ApplyRemoteRotation();
        }
    }

    private void Look()
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
        dirAnimation = inputPlayer;
        bool isWalking = inputPlayer.magnitude > 0.1f && characterController.isGrounded;

        if (isWalking)
        {


            _audioService.PlayLoopSound(WalkSound, isRunning ? 1.5f : 1f);
        }

        else
        {
            _audioService.StopSound(WalkSound);
        }
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

        Vector3 camPos = playerCamera.transform.localPosition;
        float targetY = isCrouching ? (crouchHeight/2) - 0.2f : (standHeight/2) - 0.2f;

        camPos.y = Mathf.Lerp(camPos.y, targetY, Time.deltaTime * crouchSpeed);
        playerCamera.transform.localPosition = camPos;
    }

    public void handleReload()
    {
        if (PlayerInputManager.Actions.Player.Reload.WasPressedThisFrame())
        {
            ReloadEvent reloadEvent = new ReloadEvent();
            _eventService.Publish(reloadEvent);
        }
        if (PlayerInputManager.Actions.Player.Previous.WasPressedThisFrame())
        {
            SpellChangeEvent reloadEvent = new SpellChangeEvent();
            reloadEvent.cambio = -1;
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

    private void SendAnimationDataToServer()
    {
        NetVelocity.Value = dirAnimation;
        IsRunning.Value = isRunning;
        IsCrouching.Value = isCrouching;
        OnAir.Value = !characterController.isGrounded;
    }

    private void SetAnimation()
    {
        if (!characterController.isGrounded)
        {
            if (!isJumping)
                _animator.SetBool("onAir", true);
            isJumping = true;
        }
        else
        {
            isJumping = false;
            _animator.SetBool("onAir", false);
        }




        _animator.SetFloat("VelocityX", NetVelocity.Value.x);
        _animator.SetFloat("VelocityY", NetVelocity.Value.y);

        _animator.SetBool("isRunning", IsRunning.Value);
        _animator.SetBool("isCrouching", IsCrouching.Value);
        _animator.SetBool("onAir", OnAir.Value);

    }

    private void ApplyRemoteRotation()
    {
        transform.rotation = Quaternion.Euler(0f, yaw.Value, 0f);
    }

    private void updatePreferences(GameEventBase game = null)
    {
        if (_profileService == null) return;
        UserProfile profile = _profileService.getSelectedProfile();
        if (profile != null)
        {
            Sensitivity = profile.settings.sensibility;
            xDirection = profile.settings.axisXDirection;
            yDirection = profile.settings.axisYDirection;
        }
    }

    private void handlePause()
    {
        if (PlayerInputManager.Actions.Player.pause.WasPressedThisFrame() && !NetworkManager.Singleton.IsListening)
        {
            _pauseService.TogglePause();
        }
    }

    public void Hit(float damage)
    {
        if (!IsServer) return;
        _characterService.TakeDamage((int)damage);
    }
}