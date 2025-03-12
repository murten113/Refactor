using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; // For UI elements

[RequireComponent(typeof(PlayerScript))]
public class PlayerSprinting : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float maxStamina = 5f;
    [SerializeField] private float sprintDrainRate = 1f;
    [SerializeField] private float staminaRegenRate = 1f;
    [SerializeField] private Slider staminaBar; // Assign in Inspector

    private float currentStamina;
    private bool isSprinting;

    private PlayerScript player;
    private PlayerInput playerInput;
    private InputAction sprintAction;

    private void Awake()
    {
        player = GetComponent<PlayerScript>();
        playerInput = GetComponent<PlayerInput>();
        sprintAction = playerInput.actions["Sprint"];

        currentStamina = maxStamina;
        if (staminaBar != null)
        {
            staminaBar.maxValue = maxStamina;
            staminaBar.value = maxStamina;
        }
    }

    private void Update()
    {
        HandleStamina();
    }

    private void HandleStamina()
    {
        if (isSprinting)
        {
            currentStamina -= sprintDrainRate * Time.deltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isSprinting = false; // Stop sprinting when out of stamina
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

    public void OnBeforeMove()
    {
        float sprintInput = sprintAction.ReadValue<float>();

        if (sprintInput > 0 && currentStamina > 0)
        {
            isSprinting = true;
            float forwardMovementFactor = Mathf.Clamp01(Vector3.Dot(player.transform.forward, player.velocity.normalized));
            float multiplier = Mathf.Lerp(1f, speedMultiplier, forwardMovementFactor);
            player.movementSpeedMultiplier *= multiplier;
        }
        else
        {
            isSprinting = false;
        }
    }

    private void OnEnable() => player.OnBeforeMove += OnBeforeMove;
    private void OnDisable() => player.OnBeforeMove -= OnBeforeMove;
}
