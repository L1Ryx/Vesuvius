using System.Collections;
using _ScriptableObjects.SceneAudio;
using Public.Tarodev_2D_Controller.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Gameplay._Arch
{
    public class PlayerAudio : MonoBehaviour
    {
        [Header("References")]
        public SceneAudioData sceneAudioData; // Reference to the ScriptableObject holding scene audio data
        [SerializeField] private PlayerController playerController; // Reference to the PlayerController

        [Header("Footstep Settings")]
        [SerializeField] private float footstepInterval = 0.5f; // Settable interval for footsteps (in seconds)
        [SerializeField] private float graceTime = 0.2f; // Grace period (in seconds) for walking detection
        [SerializeField] private float landingGracePeriod = 0.0f; // Grace period to ignore landing sounds at the start

        private string currentSceneName;
        private Coroutine footstepCoroutine;
        private Coroutine graceCoroutine;
        private bool isGracePeriodActive = false;
        private bool landingGraceExpired = false; // Flag for whether the landing grace period has ended

        private void Start()
        {
            // Get the current active scene name
            currentSceneName = SceneManager.GetActiveScene().name;

            // Start a landing grace period coroutine
            StartCoroutine(StartLandingGracePeriod());
        }

        private void Update()
        {
            HandleWalkingDetection();
            HandleLandingDetection();
        }

        private void HandleWalkingDetection()
        {
            // Check if the player is walking and grounded, or if grace period is active
            if ((playerController.IsWalking() || isGracePeriodActive) && footstepCoroutine == null)
            {
                // Start the footstep loop if not already playing
                footstepCoroutine = StartCoroutine(FootstepLoop());
            }
            else if (!playerController.IsWalking() && footstepCoroutine != null && !isGracePeriodActive)
            {
                // Start grace period before stopping the footstep loop
                if (graceCoroutine == null)
                    graceCoroutine = StartCoroutine(StartGracePeriod());
            }
        }

        public void PlayJump()
        {
            // Get Openness value for the current scene
            float opennessValue = GetOpennessForCurrentScene();

            // Set the Openness RTPC in Wwise
            AkSoundEngine.SetRTPCValue("Openness", opennessValue);

            // Play the jump event
            AkSoundEngine.PostEvent("Play_Jump", gameObject);
        }

        public void PlayPlayerDamaged()
        {
            AkSoundEngine.PostEvent("Play_PlayerDamaged", gameObject);
        }

        public void PlayPlayerHealing()
        {
            AkSoundEngine.PostEvent("Play_PlayerHealing", gameObject);
        }

        public void PlayPlayerHealed()
        {
            AkSoundEngine.PostEvent("Play_PlayerHealed", gameObject);
        }

        private void HandleLandingDetection()
        {
            // Ignore landing detection if within the grace period
            if (!landingGraceExpired) return;

            // Check if the player has landed
            if (playerController.HasLanded())
            {
                PlayLanding();
            }
        }

        private IEnumerator StartLandingGracePeriod()
        {
            // Wait for the landing grace period to expire
            yield return new WaitForSeconds(landingGracePeriod);
            landingGraceExpired = true; // Landing sounds are now allowed
        }

        private IEnumerator StartGracePeriod()
        {
            isGracePeriodActive = true;

            // Wait for the grace period
            yield return new WaitForSeconds(graceTime);

            // If still not walking after grace time, stop the footstep loop
            if (!playerController.IsWalking())
            {
                StopCoroutine(footstepCoroutine);
                footstepCoroutine = null;
            }

            isGracePeriodActive = false;
            graceCoroutine = null;
        }

        private IEnumerator FootstepLoop()
        {
            while (playerController.IsWalking() || isGracePeriodActive)
            {
                // Play the footstep sound
                PlayFootstep();

                // Wait for the next interval
                yield return new WaitForSeconds(footstepInterval);
            }
        }

        public void PlayFootstep()
        {
            // Get Openness value for the current scene
            float opennessValue = GetOpennessForCurrentScene();

            // Set the Openness RTPC in Wwise
            AkSoundEngine.SetRTPCValue("Openness", opennessValue);

            // Play the footstep event
            AkSoundEngine.PostEvent("Play_Footstep", gameObject);
        }

        public void PlaySwing()
        {
            // Get Openness value for the current scene
            float opennessValue = GetOpennessForCurrentScene();

            // Set the Openness RTPC in Wwise
            AkSoundEngine.SetRTPCValue("Openness", opennessValue);

            // Play the footstep event
            AkSoundEngine.PostEvent("Play_Swing", gameObject);
        }

        public void PlayLanding()
        {
            // Get Openness value for the current scene
            float opennessValue = GetOpennessForCurrentScene();

            // Set the Openness RTPC in Wwise
            AkSoundEngine.SetRTPCValue("Openness", opennessValue);

            // Play the landing event
            AkSoundEngine.PostEvent("Play_Landing", gameObject);
        }

        private float GetOpennessForCurrentScene()
        {
            // If no SceneAudioData is assigned, return a default value
            if (sceneAudioData == null)
            {
                Debug.LogWarning("SceneAudioData is not assigned. Using default Openness value of 50.");
                return 50f; // Default Openness value
            }

            // Initialize the dictionary if necessary
            sceneAudioData.InitializeDictionary();

            // Try to get the Openness value for the current scene
            if (sceneAudioData.sceneAudioDictionary.TryGetValue(currentSceneName, out AudioInfo audioInfo))
            {
                return audioInfo.Openness;
            }
            else
            {
                Debug.LogWarning($"No audio data found for scene: {currentSceneName}. Using default Openness value of 50.");
                return 50f; // Default Openness value
            }
        }

    
    }
}
