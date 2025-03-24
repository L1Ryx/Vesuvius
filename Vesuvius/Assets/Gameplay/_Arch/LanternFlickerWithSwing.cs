using UnityEngine;
using UnityEngine.Rendering.Universal;

// Import for 2D Lights

namespace Gameplay._Arch
{
    public class LanternFlickerWithSwing : MonoBehaviour
    {
        public Light2D lanternLight; // Reference to the 2D light
        public float minFalloffStrength = 0.6f; // Minimum falloff strength
        public float maxFalloffStrength = 0.8f; // Maximum falloff strength
        public float flickerSpeed = 5f; // Speed of the flicker

        [Header("Swing Parameters")]
        public float swingAngle = 10f; // Maximum swing angle in degrees
        public float swingSpeed = 2f; // Speed of the swing animation

        private float targetFalloffStrength; // Target falloff strength
        private float currentFalloffStrength; // Current falloff strength
        private float randomSwingOffset; // Random offset for desyncing lanterns

        void Start()
        {
            if (lanternLight == null)
            {
                Debug.LogError("Lantern Light is not assigned!");
                return;
            }

            // Initialize falloff strength
            currentFalloffStrength = lanternLight.falloffIntensity;
            SetNewTarget();

            // Generate a random swing offset to desync animations
            randomSwingOffset = Random.Range(0f, Mathf.PI * 2); // Random value between 0 and 2π
        }

        void Update()
        {
            if (lanternLight == null) return;

            // Flicker logic
            HandleFlicker();

            // Swing logic
            HandleSwing();
        }

        void HandleFlicker()
        {
            // Lerp towards the target falloff strength
            currentFalloffStrength = Mathf.Lerp(currentFalloffStrength, targetFalloffStrength, Time.deltaTime * flickerSpeed);

            // Apply the current falloff strength to the light
            lanternLight.falloffIntensity = currentFalloffStrength;

            // If close enough to the target, pick a new target
            if (Mathf.Abs(currentFalloffStrength - targetFalloffStrength) < 0.01f)
            {
                SetNewTarget();
            }
        }

        void HandleSwing()
        {
            // Calculate the new rotation angle using a sine wave with a random offset
            float swing = Mathf.Sin(Time.time * swingSpeed + randomSwingOffset) * swingAngle;

            // Apply the swing to the Z rotation (pivot should be at the top of the lantern)
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, swing);
        }

        void SetNewTarget()
        {
            // Randomize a new target within the specified range
            targetFalloffStrength = Random.Range(minFalloffStrength, maxFalloffStrength);
        }
    }
}
