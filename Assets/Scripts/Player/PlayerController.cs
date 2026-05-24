using System;
using UnityEngine;
/// <summary>
/// Componente de Unity encargado de gestionar el control completo del jugador. Controla el movimiento, rotación de cámara, salto, agacharse, animaciones, sonidos, pausa, cambio de hechizos y daño recibido. Se comunica con múltiples servicios como <see cref="ICharacterService"/>, <see cref="IEventService"/>, <see cref="IAudioService"/>, <see cref="IPauseService"/> y <see cref="IProfileService"/>. 
/// También utiliza <see cref="PlayerInputManager"/> para gestionar las entradas del jugador
/// </summary>
public class PlayerController : MonoBehaviour, IHittable
{
    [Header("Movimiento")]
    [SerializeField] private float Velocity = 10f;
    [SerializeField] private AudioClip WalkSound;

    [Header(" ")]
    [SerializeField] private float Sensitivity = 0.5f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private int xDirection = 1;
    [SerializeField] private int yDirection = 1;

    [Header("Crouch")]
    [SerializeField] private float standHeight = 1f;
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float crouchSpeed = 8f;

    [Header("Center")]
    [SerializeField] private float crouchCenter = 0.5f;
    [SerializeField] private float standCenter = 1f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 1f;
    private bool isJumping;

    private CharacterController characterController;
    private float gravity = -9.8f;
    private float velocityY;
    private float previusVelocity;
    private bool isRunning;
    private bool isCrouching;

    private Vector2 dirAnimation;

    private float yRotation = 0f;

    private IPauseService _pauseService;
    private IEventService _eventService;
    private ICharacterService _characterService;

    private IProfileService _profileService;
    private IAudioService _audioService;
    private Animator _animator;


    /// <summary>
    /// Método ejecutado durante la inicialización del objeto. Configura el mapa de controles mediante <see cref="PlayerInputManager"/>, obtiene referencias a servicios mediante <see cref="AppContainer"/>, registra eventos de <see cref="PreferenceChangeEvent"/> y configura la cámara y el cursor
    /// </summary>
    private void Awake()
    {
        PlayerInputManager.SwitchControlMap(PlayerInputManager.ControlMap.Player);
        characterController = GetComponent<CharacterController>();
        _eventService = AppContainer.Get<IEventService>();
        _characterService = AppContainer.Get<ICharacterService>();
        _audioService = AppContainer.Get<IAudioService>();
        _profileService = AppContainer.Get<IProfileService>();
        _pauseService = AppContainer.Get<IPauseService>();
        _eventService.Subscribe<PreferenceChangeEvent>(updatePreferences);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _animator = GetComponent<Animator>();
        updatePreferences();
    }
    /// <summary>
    /// Método ejecutado al destruir el objeto. Elimina la suscripción al evento <see cref="PreferenceChangeEvent"/> desde <see cref="IEventService"/>
    /// </summary>
    private void OnDestroy()
    {
        _eventService.Unsubscribe<PreferenceChangeEvent>(updatePreferences);
    }
    /// <summary>
    /// Método ejecutado automáticamente en cada frame. Gestiona las funciones principales del jugador como movimiento, cámara, agacharse, recarga, cambio de hechizos, pausa y animaciones
    /// </summary>
    private void Update()
    {
        Look ();
        Move();
        HandleCrouch();
        handleReload();
        handleChangeWeapon();
        handlePause();
        SetAnimation();
    }


    /// <summary>
    /// Gestiona la rotación de la cámara y del personaje utilizando las entradas proporcionadas por <see cref="PlayerInputManager"/>
    /// </summary>
    private void Look ()
    {
        Vector2  Input = PlayerInputManager.Actions.Player.Look.ReadValue<Vector2>();

        float  X =  Input.x * Sensitivity * xDirection;
        float  Y =  Input.y * Sensitivity * yDirection;

        yRotation -=  Y;
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(yRotation , 0f, 0f);
        transform.Rotate(Vector3.up *  X);
    }
    /// <summary>
    /// Gestiona el movimiento del jugador, incluyendo desplazamiento, gravedad, salto, sprint y reproducción de sonidos mediante <see cref="IAudioService"/>
    /// </summary>
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
            isCrouching = false;
            HandleCrouch();
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
    /// <summary>
    /// Gestiona el estado agachado del jugador modificando progresivamente la altura del <see cref="CharacterController"/> y la posición de la cámara
    /// </summary>
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
        float targetY = isCrouching ? (crouchHeight) + 0.3f : (standHeight) - 0.3f;


        camPos.y = Mathf.Lerp(camPos.y, targetY, Time.deltaTime * crouchSpeed);
        cameraTransform.localPosition = camPos;

    }
    /// <summary>
    /// Detecta la entrada de recarga mediante <see cref="PlayerInputManager"/> y publica un evento <see cref="ReloadEvent"/> mediante <see cref="IEventService"/>
    /// </summary>
    public void handleReload()
    {
        if (PlayerInputManager.Actions.Player.Reload.WasPressedThisFrame())
        {
            ReloadEvent reloadEvent = new ReloadEvent();
            _eventService.Publish(reloadEvent);
        }
    }
    /// <summary>
    /// Detecta el cambio de hechizo mediante <see cref="PlayerInputManager"/> y publica eventos <see cref="SpellChangeEvent"/> mediante <see cref="IEventService"/>
    /// </summary>
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
    /// <summary>
    /// Detecta la acción de pausa mediante <see cref="PlayerInputManager"/> y alterna el estado del juego utilizando <see cref="IPauseService"/>
    /// </summary>
    private void handlePause()
    {
        if (PlayerInputManager.Actions.Player.pause.WasPressedThisFrame()) {
            _pauseService.TogglePause();
        }
    }
    /// <summary>
    /// Actualiza los parámetros del <see cref="Animator"/> para sincronizar las animaciones con el estado actual del jugador, como movimiento, salto, carrera o agacharse
    /// </summary>
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




        _animator.SetFloat("VelocityX", dirAnimation.x);
        _animator.SetFloat("VelocityY", dirAnimation.y);

        _animator.SetBool("isCrouching", isCrouching);
        _animator.SetBool("isRunning", isRunning);

    }
    /// <summary>
    /// Implementación de la interfaz IHittable. Aplica daño al personaje utilizando <see cref="ICharacterService"/>
    /// </summary>
    /// <param name="damage"></param>
    public void Hit(float damage)
    {
        _characterService.TakeDamage((int)damage);
    }
    /// <summary>
    /// Actualiza las preferencias del jugador utilizando los datos almacenados en <see cref="UserProfile"/> obtenidos mediante <see cref="IProfileService"/>. Modifica sensibilidad y configuración de ejes
    /// </summary>
    /// <param name="game">Evento recibido desde <see cref="IEventService"/> para actualizar preferencias. Valor por defecto null</param>
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


}