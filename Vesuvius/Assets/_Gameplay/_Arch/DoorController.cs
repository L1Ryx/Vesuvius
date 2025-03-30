using Events._Arch;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace _Gameplay._Arch
{
    public class DoorController : MonoBehaviour
    {
        [Header("Settings")]
        public TransitionData transitionData;
        public SpawnData spawnData;  // Reference to the ScriptableObject holding spawn location
        // public float fadeOutSpeed = 2f;
        public float delayBeforeTransition = 1f; // Time to wait before switching scenes

        [Header("Events")]
        public UnityEvent roomExited;

        private bool isTransitioning = false; // Prevent multiple triggers

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !isTransitioning)
            {
                isTransitioning = true; // Mark as transitioning to prevent re-trigger
                spawnData.spawnLocation = transitionData.spawnPosition; // Update the spawn location
                roomExited.Invoke(); // Immediately invoke the roomExited event
                StartCoroutine(TransitionSceneAfterDelay()); // Begin the scene transition after a delay
            }
        }

        private System.Collections.IEnumerator TransitionSceneAfterDelay()
        {
            // Wait for the specified delay
            yield return new WaitForSeconds(delayBeforeTransition);

            // Load the new scene
            SceneManager.LoadScene(transitionData.sceneToLoad);
        }
    }
}
