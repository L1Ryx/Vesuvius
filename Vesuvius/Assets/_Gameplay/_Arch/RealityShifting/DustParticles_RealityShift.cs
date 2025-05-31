using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DustParticles_RealityShift : MonoBehaviour, IRealityShiftable
{
    [Header("First Dimension Colors")]
    public Color firstDimensionColorMin = Color.white; // Minimum color for first dimension
    public Color firstDimensionColorMax = Color.gray; // Maximum color for first dimension

    [Header("Second Dimension Colors")]
    public Color secondDimensionColorMin = Color.blue; // Minimum color for second dimension
    public Color secondDimensionColorMax = Color.cyan; // Maximum color for second dimension

    private ParticleSystem particleSystem;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void SetParticleColors(Color colorMin, Color colorMax)
    {
        // Access the ParticleSystem's color over lifetime module
        var mainModule = particleSystem.main;
        var colorOverLifetime = particleSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;

        // Define the gradient for the color range
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(colorMin, 0f),
                new GradientColorKey(colorMax, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f), // Full alpha at start
                new GradientAlphaKey(1f, 1f)  // Full alpha at end
            }
        );

        // Apply the gradient to the color over lifetime module
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        // Restart the particle system to reflect the changes immediately
        particleSystem.Stop();
        particleSystem.Play();
    }

    public void RealityShiftCrossFade(bool isAltReality, float crossfadeDuration)
    {
        //currently no crossfade for the particle colors so we just call shift instantly.
        RealityShiftInstantly(isAltReality);
    }

    public void RealityShiftInstantly(bool isAltReality)
    {
        // Set the initial colors based on the first dimension
        Color colorMax = isAltReality ? secondDimensionColorMax:firstDimensionColorMax;
        Color colorMin = isAltReality ? secondDimensionColorMin:firstDimensionColorMin;

        SetParticleColors(colorMin,colorMax);
    }
}
