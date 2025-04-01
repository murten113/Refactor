using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
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



    //get all the needed components when the script first starts
   private void Awake()
    {
        Controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
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
