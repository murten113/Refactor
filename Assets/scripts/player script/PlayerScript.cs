using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] float mouseSens = 3f;
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float mass = 1f;
    [SerializeField] float acceleration = 20f;
    [SerializeField] Transform cameraTransform;

    public event Action OnBeforeMove;

    internal float movementSpeedMultiplier;

    public Climbing Climbing;

    PlayerInput playerInput;
    InputAction moveAction;
    InputAction lookAction;
    InputAction sprintAction;


    internal CharacterController Controller;
    internal Vector3 velocity;
    Vector2 look;


    void Awake()
    {
        Controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        sprintAction = playerInput.actions["Sprint"];
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        UpdateLook();
        UpdateMovement();
        UpdateGravity();
    }

    void UpdateLook()
    {
        var lookInput = lookAction.ReadValue<Vector2>();
        look.x += lookInput.x * mouseSens;
        look.y += lookInput.y * mouseSens;

        look.y = Mathf.Clamp(look.y, -89f, 89f);

        cameraTransform.localRotation = Quaternion.Euler(-look.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, look.x, 0);

    }

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

    void UpdateMovement()
    {
        movementSpeedMultiplier = 1f;
        OnBeforeMove?.Invoke();


       var input = GetMovementInput();

        var factor = acceleration * Time.deltaTime;
        velocity.x = Mathf.Lerp(velocity.x, input.x, factor);
        velocity.z = Mathf.Lerp(velocity.z, input.z, factor);



        Controller.Move(velocity * Time.deltaTime);
    }

    void UpdateGravity()
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
            Debug.Log("Grounded - Gravity reset.");
        }
        else
        {
            velocity.y += gravity.y;
            Debug.Log("Falling - Gravity applied: " + gravity.y);
        }
    }

}
