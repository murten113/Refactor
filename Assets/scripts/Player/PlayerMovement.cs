using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles camera look and basic movement. Depends on movementSpeedMultiplier from PlayerSprint.
/// </summary>
public class PlayerMovement : MonoBehaviour, IPlayerComponent
{
    [Header("Basic Movement")]
    [SerializeField] private float mouseSens = 3f;
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private Transform cameraTransform;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private Vector2 look;

    public event System.Action OnBeforeMove;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];

        if (cameraTransform == null)
        {
            var cam = GetComponentInChildren<Camera>();
            if (cam != null)
                cameraTransform = cam.transform;
        }
    }

    public void Process(PlayerScript player)
    {
        UpdateLook();
        UpdateMovement(player);
    }

    public void UpdateLook()
    {
        if (cameraTransform == null) return;

        var lookInput = lookAction.ReadValue<Vector2>();
        look.x += lookInput.x * mouseSens;
        look.y += lookInput.y * mouseSens;
        look.y = Mathf.Clamp(look.y, -89f, 89f);

        cameraTransform.localRotation = Quaternion.Euler(-look.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, look.x, 0);
    }

    private Vector3 GetMovementInput(PlayerScript player)
    {
        var moveInput = moveAction.ReadValue<Vector2>();
        var input = new Vector3();
        input += transform.forward * moveInput.y;
        input += transform.right * moveInput.x;
        input = Vector3.ClampMagnitude(input, 1f);
        input *= moveSpeed * player.MovementSpeedMultiplier;
        return input;
    }

    public void UpdateMovement(PlayerScript player)
    {
        player.MovementSpeedMultiplier = 1f;
        OnBeforeMove?.Invoke();

        var input = GetMovementInput(player);
        var factor = acceleration * Time.deltaTime;
        player.Velocity = new Vector3(
            Mathf.Lerp(player.Velocity.x, input.x, factor),
            player.Velocity.y,
            Mathf.Lerp(player.Velocity.z, input.z, factor)
        );

        player.Controller.Move(player.Velocity * Time.deltaTime);
    }

    public float MoveSpeed => moveSpeed;
    public Transform CameraTransform => cameraTransform;
}
