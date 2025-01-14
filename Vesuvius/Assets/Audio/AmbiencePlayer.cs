using UnityEngine;
using UnityEngine.SceneManagement;

public class AmbiencePlayer : MonoBehaviour
{
    // Reference to your SceneAudioData ScriptableObject
    public SceneAudioData sceneAudioData;

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
}
