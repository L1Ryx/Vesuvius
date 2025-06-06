using UnityEngine;

public class FloatyObject : MonoBehaviour
{
    [Header("Bobbing Settings")]
    [Tooltip("How far up and down the object moves.")]
    public float amplitude = 0.25f;

    [Tooltip("How fast the object bobs.")]
    public float frequency = 1f;

    [Header("Scaling Settings")]
    [Tooltip("Enable to make the object pulse in size.")]
    public bool enableScalePulse = true;

    [Tooltip("Maximum scale change from original size.")]
    public float scaleAmplitude = 0.05f;

    [Tooltip("Speed of pulsing.")]
    public float scaleFrequency = 1.5f;

    private Vector3 startPosition;
    private Vector3 originalScale;

    void Start()
    {
        startPosition = transform.position;
        originalScale = transform.localScale;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = startPosition + new Vector3(0f, yOffset, 0f);
        if (enableScalePulse)
        {
            float scaleOffset = Mathf.Sin(Time.time * scaleFrequency) * scaleAmplitude;
            transform.localScale = originalScale * (1 + scaleOffset);
        }
    }
}