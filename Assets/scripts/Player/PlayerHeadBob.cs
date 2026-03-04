using UnityEngine;

/// <summary>
/// Applies head bobbing effect to camera when moving on ground.
/// </summary>
public class PlayerHeadBob : MonoBehaviour, IPlayerComponent
{
    [Header("Head Bobbing")]
    [SerializeField] private float bobFrequency = 5f;
    [SerializeField] private float bobAmplitude = 0.05f;

    private float bobTimer;
    private Vector3 originalCameraPosition;
    private Transform cameraTransform;
    private float moveSpeed;

    public void Process(PlayerScript player)
    {
        if (cameraTransform == null)
        {
            var movement = GetComponent<PlayerMovement>();
            if (movement != null)
            {
                cameraTransform = movement.CameraTransform;
                moveSpeed = movement.MoveSpeed;
                originalCameraPosition = cameraTransform.localPosition;
            }
        }

        if (cameraTransform != null)
            ApplyHeadBobbing(player);
    }

    private void ApplyHeadBobbing(PlayerScript player)
    {
        if (player.Controller.velocity.magnitude > 0.1f && player.Controller.isGrounded)
        {
            bobTimer += Time.deltaTime * (player.Velocity.magnitude / moveSpeed) * bobFrequency;
            float bobOffset = Mathf.Sin(bobTimer) * bobAmplitude;
            cameraTransform.localPosition = originalCameraPosition + new Vector3(0, bobOffset, 0);
        }
        else
        {
            bobTimer = 0;
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, originalCameraPosition, Time.deltaTime * 5f);
        }
    }
}
