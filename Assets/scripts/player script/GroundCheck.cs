using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public bool grounded;

    void OnCollisionStay(Collision collision)
    {
        // Check if the object the player is touching has the "Ground" tag
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.otherCollider.CompareTag("Ground"))
            {
                grounded = true;
                return;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // If the player exits contact with a ground object, set grounded to false
        if (collision.collider.CompareTag("Ground"))
        {
            grounded = false;
        }
    }
}
