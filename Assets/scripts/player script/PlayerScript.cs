using UnityEngine;

/// <summary>
/// Coordinator for player behavior. Delegates to sub-components that each handle
/// a specific behavior (movement, jump, sprint, climbing, gravity, head bob).
/// </summary>
public class PlayerScript : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 velocity;
    private float movementSpeedMultiplier = 1f;

    private PlayerMovement playerMovement;
    private PlayerJump playerJump;
    private PlayerSprint playerSprint;
    private PlayerClimbing playerClimbing;
    private PlayerGravity playerGravity;
    private PlayerHeadBob playerHeadBob;

    public CharacterController Controller => controller;
    public Vector3 Velocity { get => velocity; set => velocity = value; }
    public float MovementSpeedMultiplier { get => movementSpeedMultiplier; set => movementSpeedMultiplier = value; }
    public bool Climbing => playerClimbing != null ? playerClimbing.Climbing : false;
    public float ClimbSpeed => playerClimbing != null ? playerClimbing.ClimbSpeed : 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
        playerJump = GetComponent<PlayerJump>();
        playerSprint = GetComponent<PlayerSprint>();
        playerClimbing = GetComponent<PlayerClimbing>();
        playerGravity = GetComponent<PlayerGravity>();
        playerHeadBob = GetComponent<PlayerHeadBob>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (playerClimbing != null)
            playerClimbing.ResetClimbTimer();
    }

    private void OnEnable()
    {
        if (playerMovement != null)
            playerMovement.OnBeforeMove += OnBeforeMove;
    }

    private void OnDisable()
    {
        if (playerMovement != null)
            playerMovement.OnBeforeMove -= OnBeforeMove;
    }

    private void OnBeforeMove()
    {
        playerJump?.ApplyJump(this);
        playerSprint?.ApplySprint(this);
    }

    public void ResetClimbTimer()
    {
        playerClimbing?.ResetClimbTimer();
    }

    private void Update()
    {
        playerMovement?.Process(this);
        playerGravity?.Process(this);
        playerSprint?.Process(this);
        playerClimbing?.Process(this);
        playerGravity?.Process(this);
        playerHeadBob?.Process(this);
    }
}
