using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DustParticleSwitcher : MonoBehaviour
{
    [Header("First Dimension Colors")]
    public Color firstDimensionColorMin = Color.white; // Minimum color for first dimension
    public Color firstDimensionColorMax = Color.gray; // Maximum color for first dimension

    [Header("Second Dimension Colors")]
    public Color secondDimensionColorMin = Color.blue; // Minimum color for second dimension
    public Color secondDimensionColorMax = Color.cyan; // Maximum color for second dimension

    private ParticleSystem particleSystem;
    private bool isSecondDimension = false; // Track the current dimension

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();

        // Set the initial colors based on the first dimension
        SetParticleColors(firstDimensionColorMin, firstDimensionColorMax);
    }

    private void Update()
    {
        // Debug: Press '0' to switch between dimensions
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SwitchDimension();
        }
    }

    public void SwitchDimension()
    {
        if (isSecondDimension)
        {
            // Switch back to the first dimension
            SetParticleColors(firstDimensionColorMin, firstDimensionColorMax);
        }
        else
        {
            // Switch to the second dimension
            SetParticleColors(secondDimensionColorMin, secondDimensionColorMax);
        }

        isSecondDimension = !isSecondDimension;
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
}
