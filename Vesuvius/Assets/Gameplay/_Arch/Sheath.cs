using UnityEngine;

public class Sheath : MonoBehaviour
{
    [Header("References")]
    public Transform sheathSprite; // Reference to the sheath sprite Transform

    [Header("Bobbing Settings")]
    public float bobbingAmplitude = 0.1f; // The vertical distance the sheath bobs
    public float bobbingSpeed = 2f; // The speed of the bobbing motion

    [Header("Scaling Settings")]
    public float scalingAmplitude = 0.05f; // The amount by which the size changes
    public float scalingSpeed = 1.5f; // The speed of the size oscillation

    private Vector3 initialPosition; // The starting position of the sheath sprite
    private Vector3 initialScale; // The starting scale of the sheath sprite

    private void Start()
    {
        if (sheathSprite == null)
        {
            Debug.LogWarning("Sheath sprite is not assigned! Disabling script.");
            enabled = false;
            return;
        }

        // Cache the initial position and scale of the sheath sprite
        initialPosition = sheathSprite.localPosition;
        initialScale = sheathSprite.localScale;
    }

    private void Update()
    {
        if (sheathSprite != null)
        {
            ApplyBobbing();
            ApplyScaling();
        }
    }

    private void ApplyBobbing()
    {
        // Calculate the new vertical position using a sine wave for smooth oscillation
        float bobbingOffset = Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmplitude;
        sheathSprite.localPosition = initialPosition + new Vector3(0, bobbingOffset, 0);
    }

    private void ApplyScaling()
    {
        // Calculate the new scale using a sine wave for smooth size oscillation
        float scalingOffset = Mathf.Sin(Time.time * scalingSpeed) * scalingAmplitude;
        sheathSprite.localScale = initialScale + new Vector3(scalingOffset, scalingOffset, 0);
    }
}
