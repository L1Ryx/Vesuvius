using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class AmbiencePlayer : MonoBehaviour
{
    public SceneAudioData sceneAudioData;
    public CinemachineImpulseSource impulseSource;

    // Singleton instance
    public static AmbiencePlayer Instance { get; private set; }

    public float transitionDuration = 2f;  // Duration of the crossfade
    private bool isSecondDimension = false; // Track current dimension for debugging
    private float currentDimensionBlend = 0f; // Track the RTPC value locally

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

    private void Update()
    {
        // Debug: Press '0' to toggle dimensions and adjust RTPC values
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ToggleDimension();
        }
    }

    private void PlayAmbience()
    {
        // Start playing both ambiences
        AkSoundEngine.PostEvent("Play_CaveAmbienceBlend", gameObject);
        AkSoundEngine.PostEvent("Play_ForestAmbienceBlend", gameObject);

        // Set the initial RTPC value to match the starting dimension
        AkSoundEngine.SetRTPCValue("Dimension_Blend", currentDimensionBlend);
    }

    private void ToggleDimension()
    {
        // Determine the target value for the RTPC
        float targetValue = isSecondDimension ? 0f : 100f;

        // Start the crossfade
        StartCoroutine(CrossfadeDimension(targetValue));

        // Toggle the dimension flag
        isSecondDimension = !isSecondDimension;

        Debug.Log(isSecondDimension
            ? "Switched to Forest dimension."
            : "Switched to Icy Cavern dimension.");

        // Trigger camera shake during dimension switch
        TriggerCameraShake();
    }

    private System.Collections.IEnumerator CrossfadeDimension(float targetValue)
    {
        float startingValue = currentDimensionBlend;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            currentDimensionBlend = Mathf.Lerp(startingValue, targetValue, elapsed / transitionDuration);

            // Update the RTPC value in Wwise
            AkSoundEngine.SetRTPCValue("Dimension_Blend", currentDimensionBlend);

            yield return null;
        }

        // Ensure the final RTPC value is set
        currentDimensionBlend = targetValue;
        AkSoundEngine.SetRTPCValue("Dimension_Blend", currentDimensionBlend);
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
}
