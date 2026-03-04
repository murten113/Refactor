using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Handles sprinting and stamina. Modifies movementSpeedMultiplier before movement is applied.
/// </summary>
public class PlayerSprint : MonoBehaviour, IPlayerComponent
{
    [Header("Sprinting")]
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float maxStamina = 5f;
    [SerializeField] private float sprintDrainRate = 1f;
    [SerializeField] private float staminaRegenRate = 1f;
    [SerializeField] private Slider staminaBar;

    private float currentStamina;
    private bool isSprinting;
    private InputAction sprintAction;

    private void Awake()
    {
        sprintAction = GetComponent<PlayerInput>().actions["Sprint"];
        currentStamina = maxStamina;
        SetStartStamina();
    }

    public void Process(PlayerScript player)
    {
        HandleStamina();
    }

    private void SetStartStamina()
    {
        if (staminaBar != null)
        {
            staminaBar.maxValue = maxStamina;
            staminaBar.value = maxStamina;
        }
    }

    private void HandleStamina()
    {
        if (isSprinting)
        {
            currentStamina -= sprintDrainRate * Time.deltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isSprinting = false;
            }
        }
        else
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }

        if (staminaBar != null)
            staminaBar.value = currentStamina;
    }

    public void ApplySprint(PlayerScript player)
    {
        float sprintInput = sprintAction.ReadValue<float>();

        if (sprintInput > 0 && currentStamina > 0)
        {
            isSprinting = true;
            float forwardMovementFactor = Mathf.Clamp01(Vector3.Dot(transform.forward, player.Velocity.normalized));
            float multiplier = Mathf.Lerp(1f, speedMultiplier, forwardMovementFactor);
            player.MovementSpeedMultiplier *= multiplier;
        }
        else
        {
            isSprinting = false;
        }
    }
}
