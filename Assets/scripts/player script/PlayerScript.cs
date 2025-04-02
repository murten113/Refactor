using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [Header("Basic Movement")]
    [SerializeField] public float mouseSens = 3f;
    [SerializeField] public float moveSpeed = 6f;
    [SerializeField] public float mass = 1f;
    [SerializeField] public float acceleration = 20f;
    [SerializeField] private Transform cameraTransform;

    public event Action OnBeforeMove;

    internal float movementSpeedMultiplier;

    public Climbing Climbing;

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
    }


    //keep all the looking, movement and gravity scripts in check every frame
    private void Update()
    {
        UpdateLook();
        UpdateMovement();
        UpdateGravity();

        HandleStamina();
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


    //get the needed input for the movement
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

    //when the player is climbing turn the player script gravity off so it doesnt interfere, and when the player is grounded reset the climb timer and if he stopped climbing reapply gravity
    public void UpdateGravity()
    {
        var gravity = Physics.gravity * mass * Time.deltaTime;

        if (Climbing.climbing)
        {
            velocity.y = Climbing.climbSpeed;
            Debug.Log("Climbing - No gravity applied.");
        }
        else if (Controller.isGrounded)
        {
            velocity.y = -1f;
            Climbing.climbTimer = Climbing.maxClimbTime;
            Debug.Log("Grounded - Gravity reset.");
        }
        else
        {
            velocity.y += gravity.y;
            Debug.Log("Falling - Gravity applied: " + gravity.y);
        }
    }

}
