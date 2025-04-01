using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public bool grounded;
    private float groundCheckDistance = 0.1f;
    private LayerMask groundLayer;


    //draw a line to the feet of the player and check if the layer that the line touches is the ground layer, if so grounded becomes true
    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, Color.red);
    }
    
}
