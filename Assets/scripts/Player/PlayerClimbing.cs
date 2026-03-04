using UnityEngine;

/// <summary>
/// Handles wall detection and climbing. Manages climb state and Rigidbody-based climbing movement.
/// </summary>
public class PlayerClimbing : MonoBehaviour, IPlayerComponent
{
    [Header("Climbing")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private float maxClimbTime = 2f;
    [SerializeField] private float detectionLength = 0.5f;
    [SerializeField] private float sphereCastRadius = 0.25f;
    [SerializeField] private float maxWallLookingAngle = 50f;

    private float climbTimer;
    private float wallLookingAngle;
    private RaycastHit frontWallHit;
    private bool wallFront;

    public bool Climbing { get; private set; }

    public void Process(PlayerScript player)
    {
        WallCheck();
        StateMachine();
        ClimbEnable();
    }

    private void ResetTimer()
    {
        climbTimer = maxClimbTime;
    }

    private void ClimbEnable()
    {
        if (Climbing)
            ClimbingMovement();
    }

    private void StateMachine()
    {
        if (wallFront && Input.GetKey(KeyCode.W) && wallLookingAngle < maxWallLookingAngle)
        {
            if (!Climbing && climbTimer > 0)
                StartClimbing();

            if (climbTimer > 0)
                climbTimer -= Time.deltaTime;

            if (climbTimer <= 0)
                StopClimbing();
        }
        else if (Climbing)
        {
            StopClimbing();
        }
    }

    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall);

        if (wallFront)
            wallLookingAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);
    }

    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
    }

    private void StartClimbing()
    {
        Climbing = true;
        rb.useGravity = false;
    }

    private void StopClimbing()
    {
        Climbing = false;
        rb.useGravity = true;
    }

    public void ResetClimbTimer()
    {
        climbTimer = maxClimbTime;
    }

    public float ClimbSpeed => climbSpeed;
}
