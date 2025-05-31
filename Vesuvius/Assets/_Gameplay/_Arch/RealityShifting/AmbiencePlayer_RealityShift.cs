using _ScriptableObjects.SceneAudio;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AmbiencePlayer_RealityShift : MonoBehaviour, IRealityShiftable
{
    public SceneAudioData sceneAudioData;
    public CinemachineImpulseSource impulseSource;

    // Singleton instance
    public static AmbiencePlayer_RealityShift Instance { get; private set; }
    private float currentRealityBlend = 0f; // Track the RTPC value locally

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

    private void Start()
    {
        // Initialize audio data for the current scene
        UpdateAudioData();
    }

    private void PlayAmbience()
    {
        // Start playing both ambiences
        AkSoundEngine.PostEvent("Play_CaveAmbienceBlend", gameObject);
        AkSoundEngine.PostEvent("Play_ForestAmbienceBlend", gameObject);

        // Set the initial RTPC value to match the starting dimension
        AkSoundEngine.SetRTPCValue("Dimension_Blend", currentRealityBlend);
    }


    private System.Collections.IEnumerator CrossfadeDimension(float targetValue, float crossfadeDuration)
    {
        float startingValue = currentRealityBlend;
        float elapsed = 0f;

        while (elapsed < crossfadeDuration)
        {
            elapsed += Time.deltaTime;
            currentRealityBlend = Mathf.Lerp(startingValue, targetValue, elapsed / crossfadeDuration);

            // Update the RTPC value in Wwise
            AkSoundEngine.SetRTPCValue("Dimension_Blend", currentRealityBlend);

            yield return null;
        }

        // Ensure the final RTPC value is set
        currentRealityBlend = targetValue;
        AkSoundEngine.SetRTPCValue("Dimension_Blend", currentRealityBlend);
    }

    private void TriggerCameraShake()
    {
        // Generate an impulse
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }
        else
        {
            Debug.LogWarning("No Cinemachine Impulse Source assigned!");
        }
    }

    public void UpdateAudioData()
    {
        // Check if the scene audio data contains the current scene
        if (sceneAudioData.sceneAudioDictionary.TryGetValue(SceneManager.GetActiveScene().name, out AudioInfo audioInfo))
        {
            // Update the RTPCs in Wwise
            AkSoundEngine.SetRTPCValue("Windiness", audioInfo.Windiness);
            AkSoundEngine.SetRTPCValue("Openness", audioInfo.Openness);
            AkSoundEngine.SetRTPCValue("Creakiness", audioInfo.Creakiness);

            Debug.Log("Updated audio data for scene: " + SceneManager.GetActiveScene().name);
        }
        else
        {
            Debug.LogWarning($"No audio data found for scene: {SceneManager.GetActiveScene().name}");
        }
    }

    private void OnApplicationQuit()
    {
        // Restore RTPC values or clean up if needed
        RestoreOriginalValues();
    }

    private void RestoreOriginalValues()
    {
        // Example: Reset RTPC values for cleanup on application quit
        AkSoundEngine.SetRTPCValue("Windiness", 0);
        AkSoundEngine.SetRTPCValue("Openness", 0);
        AkSoundEngine.SetRTPCValue("Creakiness", 0);

        Debug.Log("Restored original RTPC values on quit.");
    }

    public void RealityShiftCrossFade(bool isAltReality, float crossfadeDuration)
    {
        // Determine the target value for the RTPC
        float targetValue = isAltReality ? 100f:0f;

        // Start the crossfade
        StartCoroutine(CrossfadeDimension(targetValue, crossfadeDuration));

        // Trigger camera shake during dimension switch
        TriggerCameraShake();
    }

    public void RealityShiftInstantly(bool isAltReality)
    {
        // Ensure the final RTPC value is set
        currentRealityBlend = isAltReality ? 100f : 0f;
        AkSoundEngine.SetRTPCValue("Dimension_Blend", currentRealityBlend);
    }
}

