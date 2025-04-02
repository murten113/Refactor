using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public LayerMask whatIsWall;

    [Header("Climbing")]
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

    //set the climb timer to the set max climb time at the beginning
    private void Start()
    {
        ResetTimer();
    }
    private void Update()
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
        if (climbing)
            ClimbingMovement();
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
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontwallHit, detectionLength, whatIsWall);

        if (wallFront)
        {
            wallLookingAngle = Vector3.Angle(orientation.forward, -frontwallHit.normal);
            Debug.Log("Wall detected! Angle: " + wallLookingAngle);
        }
        else
        {
            Debug.Log("No wall detected.");
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
