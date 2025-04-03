using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{

    #region get all needed stuff
    [Header("Basic Movement")]
    [SerializeField] private float mouseSens = 3f;
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float mass = 1f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private Transform cameraTransform;

    public event Action OnBeforeMove;

    internal float movementSpeedMultiplier;

    PlayerInput playerInput;
    InputAction moveAction;
    InputAction lookAction;

    internal CharacterController Controller;
    internal Vector3 velocity;
    Vector2 look;

    [Header("Jumping")]
    public float jumpSpeed = 5f;
    public float jumpPressBufferTime = .05f;

    private bool tryingToJump;
    private float lastJumpPressTime;


    [Header("Sprinting")]
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float maxStamina = 5f;
    [SerializeField] private float sprintDrainRate = 1f;
    [SerializeField] private float staminaRegenRate = 1f;
    [SerializeField] private Slider staminaBar;

    private float currentStamina;
    private bool isSprinting;

    private InputAction sprintAction;

    [Header("Climbing")]
    public Transform orientation;
    public Rigidbody rb;
    public LayerMask whatIsWall;


    public float climbSpeed;
    public float maxClimbTime;
    public float climbTimer;

    public bool climbing;

    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookingAngle;
    private float wallLookingAngle;

    public RaycastHit frontwallHit;
    private bool wallFront;

    [Header("Head Bobbing")]
    [SerializeField] private float bobFrequency = 5f;
    [SerializeField] private float bobAmplitude = 0.05f;
    private float bobTimer;
    private Vector3 originalCameraPosition;
    #endregion

    #region Start of script and update
    //get all the needed components when the script first starts
    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        sprintAction = playerInput.actions["Sprint"];
        currentStamina = maxStamina;

        SetStartStamina();
    }

    //lock the mouse so the players mouse doesnt move across the screen or go to a diffirent screen for those with more than one screen
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        ResetTimer();

        OriginalCamPos();
    }


    //keep all the looking, movement and gravity scripts in check every frame
    private void Update()
    {
        UpdateLook();
        UpdateMovement();
        UpdateGravity();

        HandleStamina();

        WallCheck();
        StateMachine();
        ClimbEnable();

        UpdateGravity();

        ApplyHeadBobbing();
    }

    private void OnEnable()
    {
        OnBeforeMove += JumpCont;
        OnBeforeMove += SprintCont;
    }


    private void OnDisable()
    {
        OnBeforeMove -= JumpCont;
        OnBeforeMove -= SprintCont;
    }
    #endregion

    #region Basic movement
    //read the mouse input and add it to the camera so it follows
    public void UpdateLook()
    {
        var lookInput = lookAction.ReadValue<Vector2>();
        look.x += lookInput.x * mouseSens;
        look.y += lookInput.y * mouseSens;

        look.y = Mathf.Clamp(look.y, -89f, 89f);

        cameraTransform.localRotation = Quaternion.Euler(-look.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, look.x, 0);
    }


    //Set the movement input
    Vector3 GetMovementInput()
    {
        var moveInput = moveAction.ReadValue<Vector2>();

        var input = new Vector3();
        input += transform.forward * moveInput.y;
        input += transform.right * moveInput.x;
        input = Vector3.ClampMagnitude(input, 1f);
        input *= moveSpeed * movementSpeedMultiplier;
        return input;
    }


    //when the input says the player needs to move foreward add a vector to the player so it moves in said direction
    public void UpdateMovement()
    {
        movementSpeedMultiplier = 1f;
        OnBeforeMove?.Invoke();


        var input = GetMovementInput();

        var factor = acceleration * Time.deltaTime;
        velocity.x = Mathf.Lerp(velocity.x, input.x, factor);
        velocity.z = Mathf.Lerp(velocity.z, input.z, factor);



        Controller.Move(velocity * Time.deltaTime);
    }
    #endregion

    #region Jumping
    public void OnJump()
    {
        tryingToJump = true;
        lastJumpPressTime = Time.time;
    }

    public void JumpCont()
    {
        bool wasTryingToJump = Time.time - lastJumpPressTime < jumpPressBufferTime;

        bool isOrWasTryingToJump = tryingToJump || wasTryingToJump;

        if (isOrWasTryingToJump && Controller.isGrounded)
            velocity.y += jumpSpeed;

        tryingToJump = false;
    }
    #endregion

    #region Sprinting
    private void SetStartStamina()
    {
        if (staminaBar != null)
        {
            staminaBar.maxValue = maxStamina;
            staminaBar.value = maxStamina;
        }
    }

    //check if the player is sprinting, if so start depleting the stamina bar and quit sprinting if the stamina bar is empty. If the player isnt sprinting let he stamina bar regenerate
    private void HandleStamina()
    {
        if (isSprinting)
        {
            currentStamina -= sprintDrainRate * Time.deltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isSprinting = false;
            }
        }
        else
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }

        if (staminaBar != null)
            staminaBar.value = currentStamina;
    }

    //apply a speed boost to the pleyer if theyre sprinting
    public void SprintCont()
    {
        float sprintInput = sprintAction.ReadValue<float>();

        if (sprintInput > 0 && currentStamina > 0)
        {
            isSprinting = true;
            float forwardMovementFactor = Mathf.Clamp01(Vector3.Dot(transform.forward, velocity.normalized));
            float multiplier = Mathf.Lerp(1f, speedMultiplier, forwardMovementFactor);
            movementSpeedMultiplier *= multiplier;
        }
        else
            isSprinting = false;
    }
    #endregion

    #region Climbing
    private void ResetTimer()
    {
        climbTimer = maxClimbTime;
    }

    private void ClimbEnable()
    {
        if (climbing)
            ClimbingMovement();
    }

    //start climbing if the W key is pressed and the wall is meeting the walcheck conditions
    private void StateMachine()
    {
        if (wallFront && Input.GetKey(KeyCode.W) && wallLookingAngle < maxWallLookingAngle)
        {
            if (!climbing && climbTimer > 0)
                StartClimbing();

            if (climbTimer > 0)
                climbTimer -= Time.deltaTime;

            if (climbTimer <= 0)
                StopClimbing();
        }
        else if(climbing)
                StopClimbing();
    }
    #endregion

    #region Wall detection
    //check if the player is looking at a wall at the right angle and distance for climbing 
    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontwallHit, detectionLength, whatIsWall);

        if (wallFront)
            wallLookingAngle = Vector3.Angle(orientation.forward, -frontwallHit.normal);
    }


    //apply up movement to the player as climbing
    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
    }


    //the part under me is to disable cravity when climbing so the player controller doesnt pull the player down 
    private void StartClimbing()
    {
        climbing = true;
        rb.useGravity = false;
    }

    private void StopClimbing()
    {
        climbing = false;
        rb.useGravity = true;
    }
    #endregion

    #region gravity cont
    //when the player is climbing turn the player script gravity off so it doesnt interfere, and when the player is grounded reset the climb timer and if he stopped climbing reapply gravity
    public void UpdateGravity()
    {
        var gravity = Physics.gravity * mass * Time.deltaTime;

        if (climbing)
            velocity.y = climbSpeed;

        else if (Controller.isGrounded)
        {
            velocity.y = -1f;
            climbTimer = maxClimbTime;
        }
        else
            velocity.y += gravity.y;
    }
    #endregion

    #region Head bob

    //save the original camera position so you can reset it
    private void OriginalCamPos()
    {
        originalCameraPosition = cameraTransform.localPosition;
    }


    //check if the player's velocity is faster than statonary and check if the player is grounded so the head doesnt bob in the air
    //if all those are true then make the camera go up and down corresponding to the speed that the player is moving
    private void ApplyHeadBobbing()
    {
        if (Controller.velocity.magnitude > 0.1f && Controller.isGrounded)
        {
            bobTimer += Time.deltaTime * (velocity.magnitude / moveSpeed) * bobFrequency;
            float bobOffset = Mathf.Sin(bobTimer) * bobAmplitude;
            cameraTransform.localPosition = originalCameraPosition + new Vector3(0, bobOffset, 0);
        }
        else
        {
            bobTimer = 0;
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, originalCameraPosition, Time.deltaTime * 5f);
        }
    }
    #endregion

}
