using UnityEngine;
using UnityEngine.SceneManagement;

public class WatcherAudio : MonoBehaviour
{
    [Header("References")]
    public SceneAudioData sceneAudioData; // Reference to the ScriptableObject holding scene audio data
       
    private string currentSceneName;

    private void Start()
    {
        // Get the current active scene name
        currentSceneName = SceneManager.GetActiveScene().name;
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

    public void PlayWatcherAlert() {
        float opennessValue = GetOpennessForCurrentScene();

        // Set the Openness RTPC in Wwise
        AkSoundEngine.SetRTPCValue("Openness", opennessValue);

        AkSoundEngine.PostEvent("Play_WatcherAlert", gameObject);
    }

    public void PlayWatcherAttack() {
        float opennessValue = GetOpennessForCurrentScene();

        // Set the Openness RTPC in Wwise
        AkSoundEngine.SetRTPCValue("Openness", opennessValue);

        AkSoundEngine.PostEvent("Play_WatcherAttack", gameObject);
    }

    public void PlayWatcherDeath() {
        float opennessValue = GetOpennessForCurrentScene();

        // Set the Openness RTPC in Wwise
        AkSoundEngine.SetRTPCValue("Openness", opennessValue);

        AkSoundEngine.PostEvent("Play_WatcherDeath", gameObject);
    }

    public void PlayFootstep()
    {
        // Get Openness value for the current scene
        float opennessValue = GetOpennessForCurrentScene();

        // Set the Openness RTPC in Wwise
        AkSoundEngine.SetRTPCValue("Openness", opennessValue);

        // Play the footstep event
        AkSoundEngine.PostEvent("Play_WatcherFootstep", gameObject);
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
