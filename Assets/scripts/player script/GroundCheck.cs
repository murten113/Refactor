using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public bool grounded;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;

    void Update()
    {
        // Cast a ray downward to check if the player is on the ground
        grounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, Color.red);

    }
}
