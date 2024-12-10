using UnityEngine;
using UnityEngine.SceneManagement;

public class AmbiencePlayer : MonoBehaviour
{
    // Reference to your SceneAudioData ScriptableObject
    public SceneAudioData sceneAudioData;

    // Singleton instance
    public static AmbiencePlayer Instance { get; private set; }

    private void Awake()
    {
        // Implement Singleton pattern to ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes

            // Initialize the scene audio dictionary
            sceneAudioData.InitializeDictionary();

            // Start playing the ambience
            PlayAmbience();
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    private void PlayAmbience()
    {
        // Start playing your ambience events
        // Replace "Play_CaveAmbience" and "Play_RandomCaveSFX" with your Wwise event names
        AkSoundEngine.PostEvent("Play_CaveAmbienceBlend", gameObject);
        AkSoundEngine.PostEvent("Play_RandomCaveSounds", gameObject);

        UpdateAudioData();
    }

    // Function to update audio data (RTPCs) for a new scene
    public void UpdateAudioData()
    {
        // Check if the scene audio data contains the current scene
        if (sceneAudioData.sceneAudioDictionary.TryGetValue(SceneManager.GetActiveScene().name, out AudioInfo audioInfo))
        {
            // Update the RTPCs in Wwise
            AkSoundEngine.SetRTPCValue("Windiness", audioInfo.Windiness);
            AkSoundEngine.SetRTPCValue("Openness", audioInfo.Openness);
            AkSoundEngine.SetRTPCValue("Creakiness", audioInfo.Creakiness);
        }
        else
        {
            Debug.LogWarning($"No audio data found for scene: {SceneManager.GetActiveScene().name}");
        }
    }
}
