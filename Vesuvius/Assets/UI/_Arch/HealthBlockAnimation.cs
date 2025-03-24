using UnityEngine;

public class HealthBlockAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float floatAmplitude = 3f; // Vertical movement range
    [SerializeField] private float floatSpeed = 2f; // Speed of vertical movement
    [SerializeField] private float scaleAmplitude = 0.05f; // Scale pulsation range
    [SerializeField] private float scaleSpeed = 3f; // Speed of scale pulsation
    [SerializeField] private bool enableRotation = false; // Option to enable/disable rotation
    [SerializeField] private float rotationSpeed = 30f; // Speed of rotation in degrees per second
    [SerializeField] private float adjustedFloatMultiplier = 3f;

    [Header("Player Info Reference")]
    [SerializeField] private PlayerInfo playerInfo; // Reference to PlayerInfo for health values

    private Vector3 initialPosition;
    private Vector3 initialScale;

    private void Start()
    {
        // Cache the initial position and scale
        initialPosition = transform.localPosition;
        initialScale = transform.localScale;
    }

    private void Update()
    {
        // Check if health is at or below 20%
        bool isLowHealth = playerInfo.GetCurrentHealth() <= playerInfo.GetMaximumHealth() * 0.2f;

        // Adjust floating speed based on health status
        float adjustedFloatSpeed = isLowHealth ? floatSpeed * adjustedFloatMultiplier : floatSpeed;

        // Vertical floating animation
        float yOffset = Mathf.Sin(Time.time * adjustedFloatSpeed) * floatAmplitude;
        transform.localPosition = initialPosition + new Vector3(0, yOffset, 0);

        // Pulsating scale animation
        float scaleOffset = Mathf.Sin(Time.time * scaleSpeed) * scaleAmplitude;
        transform.localScale = initialScale + new Vector3(scaleOffset, scaleOffset, 0);

        // Optional rotation animation
        if (enableRotation)
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }
}
