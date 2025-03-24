using ScriptableObjects.SceneAudio;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay._Arch
{
    public class SwingAudio : MonoBehaviour
    {
        [Header("References")]
        public SceneAudioData sceneAudioData; // Reference to the ScriptableObject holding scene audio data
       
        private string currentSceneName;

        private void Start()
        {
            // Get the current active scene name
            currentSceneName = SceneManager.GetActiveScene().name;
        }

        public void PlayPlayerAttackHit()
        {
            // Get Openness value for the current scene
            float opennessValue = GetOpennessForCurrentScene();

            // Set the Openness RTPC in Wwise
            AkSoundEngine.SetRTPCValue("Openness", opennessValue);

            // Play the footstep event
            AkSoundEngine.PostEvent("Play_PlayerAttackHit", gameObject);
        }

        public void PlayPlayerAttackKill()
        {
            // Get Openness value for the current scene
            float opennessValue = GetOpennessForCurrentScene();

            // Set the Openness RTPC in Wwise
            AkSoundEngine.SetRTPCValue("Openness", opennessValue);

            // Play the footstep event
            AkSoundEngine.PostEvent("Play_PlayerAttackKill", gameObject);
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
