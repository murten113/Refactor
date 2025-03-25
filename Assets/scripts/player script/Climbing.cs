using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("References")]
    private Transform orientation;
    private Rigidbody rb;
    private LayerMask whatIsWall;
    private GroundCheck GC;

    [Header("Climbing")]
    public float climbSpeed;
    private float maxClimbTime;
    private float climbTimer;

    public bool climbing;

    [Header("Detection")]
    private float detectionLength;
    private float sphereCastRadius;
    private float maxWallLookingAngle;
    private float wallLookingAngle;

    private RaycastHit frontwallHit;
    private bool wallFront;


    private void Update()
    {
        WallCheck();
        StateMachine();

        if(climbing) ClimbingMovement();
    }

    //start climbing if the W key is pressed and the wall is meeting the walcheck conditions
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


    //check if the player is looking at a wall at the right angle and distance for climbing 
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


    //apply up movement to the player as climbing
    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
        Debug.Log("Climbing... Velocity: " + rb.velocity);
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

}
