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

    private bool climbing;

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
        if(wallFront && Input.GetKey(KeyCode.W) && wallLookingAngle < maxWallLookingAngle)
        {
            if (!climbing && climbTimer > 0) StartClimbing();

            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer < 0) StopClimbing();
        } 

        else
        {
            if (climbing) StopClimbing();
        }
    }

    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontwallHit, detectionLength, whatIsWall);
        wallLookingAngle = Vector3.Angle(orientation.forward, -frontwallHit.normal);

        if (GC.grounded)
        {
            climbTimer = maxClimbTime;
        }
    }

    private void StartClimbing()
    {
        climbing = true;
    }

    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
    }

    private void StopClimbing()
    {
        climbing = false;
    }
}
