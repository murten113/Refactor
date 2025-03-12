using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PlayerScript))]

public class PlayerJump : MonoBehaviour
{

    //{

    PlayerScript player;

    public float jumpSpeed = 5f;
    public float jumpPressBufferTime = .05f;

    private bool tryingToJump;
    private float lastJumpPressTime;

    //}this area is to make all the needed bools n floats n stufz

    //{
    private void Awake()
    {
        player = GetComponent<PlayerScript>();
    }

    //} this is to get the playescript so this script can access the info within

    //{
    private void OnEnable()
    {
        player.OnBeforeMove += OnBeforeMove;
    }


    private void OnDisable()
    {
        player.OnBeforeMove -= OnBeforeMove;
    }

    //} 

    //{
    public void OnJump()
    {
        tryingToJump = true;
        lastJumpPressTime = Time.time;
    }


    public void OnBeforeMove()
    {
        bool wasTryingToJump = Time.time - lastJumpPressTime < jumpPressBufferTime;

        bool isOrWasTryingToJummp = tryingToJump || wasTryingToJump;

        if (isOrWasTryingToJummp && player.Controller.isGrounded)
            player.velocity.y += jumpSpeed;

        tryingToJump = false;
    }

    //} in this area the code checks if the player is trying to jump and if they are the code putsa force on the player to move it up
}
