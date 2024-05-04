using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour {
    public TransitionData transitionData;
    public SpawnData spawnData;  // Reference to the ScriptableObject holding spawn location

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            spawnData.spawnLocation = transitionData.spawnPosition;  // Update the spawn location
            SceneManager.LoadScene(transitionData.sceneToLoad);
        }
    }
}

