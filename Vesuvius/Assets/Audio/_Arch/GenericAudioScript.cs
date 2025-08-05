using System.Collections;
using _ScriptableObjects.SceneAudio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenericAudioScript : MonoBehaviour
{
    [Header("References")]
    public SceneAudioData sceneAudioData; // Reference to the ScriptableObject holding scene audio data

    private string currentSceneName;
    [Header("Settings")]
    [SerializeField] private float eventDelay = 0.5f; // Delay before posting Wwise events


    private void Awake()
    {
        // Get the current active scene name
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    public void PlayWwiseEvent(string eventName)
    {
        if (!string.IsNullOrEmpty(eventName))
        {
            StartCoroutine(DelayedPlayEvent(eventName));
        }
        else
        {
            Debug.LogWarning("Attempted to play a null or empty Wwise event.");
        }
    }


    private IEnumerator DelayedPlayEvent(string eventName)
    {
        yield return new WaitForSeconds(eventDelay);
        AkSoundEngine.PostEvent(eventName, gameObject);
    }



    protected float GetOpennessForCurrentScene()
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
