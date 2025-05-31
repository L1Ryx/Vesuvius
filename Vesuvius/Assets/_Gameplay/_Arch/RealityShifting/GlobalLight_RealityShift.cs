using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class GlobalLight_RealityShift : MonoBehaviour, IRealityShiftable
{
    [Header("First Dimension Settings")]
    public Color firstDimensionColor = Color.white;  // Light color for the first dimension
    public float firstDimensionIntensity = 1f;      // Light intensity for the first dimension

    [Header("Second Dimension Settings")]
    public Color secondDimensionColor = Color.blue; // Light color for the second dimension
    public float secondDimensionIntensity = 0.5f;   // Light intensity for the second dimension

    private Light2D globalLight;
    private Coroutine currentTransition;           // Track the active lerp coroutine

    private void Awake()
    {
        globalLight = GetComponent<Light2D>();
    }

    private IEnumerator LerpLightSettings(Color targetColor, float targetIntensity, float crossfadeDuration)
    {
        // Store the initial settings
        Color initialColor = globalLight.color;
        float initialIntensity = globalLight.intensity;

        float elapsedTime = 0f;

        // Lerp over the specified duration
        while (elapsedTime < crossfadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / crossfadeDuration;

            // Lerp the color and intensity
            globalLight.color = Color.Lerp(initialColor, targetColor, t);
            globalLight.intensity = Mathf.Lerp(initialIntensity, targetIntensity, t);

            yield return null;
        }

        // Ensure the target settings are applied
        SetLightSettings(targetColor, targetIntensity);
    }

    private void SetLightSettings(Color color, float intensity)
    {
        globalLight.color = color;
        globalLight.intensity = intensity;
    }

    public void RealityShiftCrossFade(bool isAltReality, float crossfadeDuration)
    {
        Color targetColor = isAltReality ? secondDimensionColor: firstDimensionColor;
        float targetIntensity = isAltReality ? secondDimensionIntensity : firstDimensionIntensity;

        if (currentTransition != null)
        {
            StopCoroutine(currentTransition);
        }
        currentTransition = StartCoroutine(LerpLightSettings(targetColor, targetIntensity, crossfadeDuration));
    }

    public void RealityShiftInstantly(bool isAltReality)
    {
        // Set initial light settings based on dimension at load
        Color targetColor = isAltReality ? secondDimensionColor: firstDimensionColor;
        float targetIntensity = isAltReality ? secondDimensionIntensity : firstDimensionIntensity;
        SetLightSettings(targetColor, targetIntensity);
    }
}
