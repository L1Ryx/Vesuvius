using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace _Gameplay._Arch
{
    [RequireComponent(typeof(Light2D))]
    public class GlobalLightSwitcher : MonoBehaviour
    {
        [Header("First Dimension Settings")]
        public Color firstDimensionColor = Color.white;  // Light color for the first dimension
        public float firstDimensionIntensity = 1f;      // Light intensity for the first dimension

        [Header("Second Dimension Settings")]
        public Color secondDimensionColor = Color.blue; // Light color for the second dimension
        public float secondDimensionIntensity = 0.5f;   // Light intensity for the second dimension

        [Header("Transition Settings")]
        public float transitionDuration = 2f;          // Duration of the lerp during the switch

        private Light2D globalLight;
        private bool isSecondDimension = false;        // Track the current dimension
        private Coroutine currentTransition;           // Track the active lerp coroutine

        private void Start()
        {
            globalLight = GetComponent<Light2D>();

            // Set initial light settings based on the first dimension
            SetLightSettings(firstDimensionColor, firstDimensionIntensity);
        }

        private void Update()
        {
            // Debug: Press '0' to switch dimensions
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SwitchDimension();
            }
        }

        public void SwitchDimension()
        {
            // Determine target settings based on the current dimension
            Color targetColor = isSecondDimension ? firstDimensionColor : secondDimensionColor;
            float targetIntensity = isSecondDimension ? firstDimensionIntensity : secondDimensionIntensity;

            // Stop any active transition and start a new one
            if (currentTransition != null)
            {
                StopCoroutine(currentTransition);
            }
            currentTransition = StartCoroutine(LerpLightSettings(targetColor, targetIntensity));

            // Toggle the dimension flag
            isSecondDimension = !isSecondDimension;
        }

        private IEnumerator LerpLightSettings(Color targetColor, float targetIntensity)
        {
            // Store the initial settings
            Color initialColor = globalLight.color;
            float initialIntensity = globalLight.intensity;

            float elapsedTime = 0f;

            // Lerp over the specified duration
            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / transitionDuration;

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
    }
}
