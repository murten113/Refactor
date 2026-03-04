using UnityEngine;

/// <summary>
/// Handles vertical velocity from gravity. Respects climbing state from PlayerClimbing.
/// </summary>
public class PlayerGravity : MonoBehaviour, IPlayerComponent
{
    [Header("Gravity")]
    [SerializeField] private float mass = 1f;

    public void Process(PlayerScript player)
    {
        UpdateGravity(player);
    }

    private void UpdateGravity(PlayerScript player)
    {
        var gravity = Physics.gravity * mass * Time.deltaTime;
        var vel = player.Velocity;

        if (player.Climbing)
        {
            vel.y = player.ClimbSpeed;
        }
        else if (player.Controller.isGrounded)
        {
            vel.y = -1f;
            player.ResetClimbTimer();
        }
        else
        {
            vel.y += gravity.y;
        }

        player.Velocity = vel;
    }
}
