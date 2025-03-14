using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("Refrences")]
    public Transform orientation;
    public Rigidbody rb;
    public LayerMask whatIsWall;
    public GroundCheck GC;

    [Header("Climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    private float climbTimer;

    public bool climbing;

    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookingAngle;
    private float wallLookingAngle;

    private RaycastHit frontwallHit;
    private bool wallFront;

    private void Update()
    {
        WallCheck();
        StateMachine();

        if(climbing) ClimbingMovement();
    }

    private void StateMachine()
    {
        if (wallFront && Input.GetKey(KeyCode.W) && wallLookingAngle < maxWallLookingAngle)
        {
            Debug.Log("Conditions met for climbing!");

            if (!climbing && climbTimer > 0)
            {
                Debug.Log("Starting to climb...");
                StartClimbing();
            }

            if (climbTimer > 0)
            {
                climbTimer -= Time.deltaTime;
                Debug.Log("Climbing timer: " + climbTimer);
            }
            if (climbTimer <= 0)
            {
                Debug.Log("Climb timer exhausted. Stopping climb.");
                StopClimbing();
            }
        }
        else
        {
            if (climbing)
            {
                Debug.Log("Stopping climb.");
                StopClimbing();
            }
        }
    }


    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward,
                                       out frontwallHit, detectionLength, whatIsWall);

        if (wallFront)
        {
            wallLookingAngle = Vector3.Angle(orientation.forward, -frontwallHit.normal);
            Debug.Log("Wall detected! Angle: " + wallLookingAngle);
        }
        else
        {
            Debug.Log("No wall detected.");
        }

        if (GC.grounded)
        {
            climbTimer = maxClimbTime;
            Debug.Log("Player grounded, reset climbTimer.");
        }
    }


 
    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
        Debug.Log("Climbing... Velocity: " + rb.velocity);
    }

    private void StartClimbing()
    {
        climbing = true;
        rb.useGravity = false;  // Disable gravity when climbing
    }

    private void StopClimbing()
    {
        climbing = false;
        rb.useGravity = true;   // Re-enable gravity when stopping
    }

}
