using UnityEngine;

/// <summary>
/// Handles jump input and execution. Subscribes to OnBeforeMove to apply jump before movement.
/// </summary>
public class PlayerJump : MonoBehaviour, IPlayerComponent
{
    [Header("Jumping")]
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float jumpPressBufferTime = 0.05f;

    private bool tryingToJump;
    private float lastJumpPressTime;

    public void Process(PlayerScript player)
    {
        // Jump is handled via OnBeforeMove subscription, not in Process
    }

    /// <summary>
    /// Called by Input System when jump is pressed.
    /// </summary>
    public void OnJump()
    {
        tryingToJump = true;
        lastJumpPressTime = Time.time;
    }

    public void ApplyJump(PlayerScript player)
    {
        bool wasTryingToJump = Time.time - lastJumpPressTime < jumpPressBufferTime;
        bool isOrWasTryingToJump = tryingToJump || wasTryingToJump;

        if (isOrWasTryingToJump && player.Controller.isGrounded)
        {
            var vel = player.Velocity;
            vel.y += jumpSpeed;
            player.Velocity = vel;
        }

        tryingToJump = false;
    }
}
