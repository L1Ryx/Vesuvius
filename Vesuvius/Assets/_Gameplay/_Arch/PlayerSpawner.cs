using Events._Arch;
using Public.Tarodev_2D_Controller.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Gameplay._Arch
{
    public class PlayerSpawner : MonoBehaviour
    {
        public SpawnData spawnData;
        public GameObject playerPrefab;
        public GameObject playerDummy;

        [Header("Runtime Data")]
        [SerializeField] private GameObject runtimePlayer;

        private bool spawnFacingLeft = false; // Determines if the player should spawn facing left

        private void Awake()
        {
            spawnFacingLeft = spawnData.isFacingLeft;
            runtimePlayer = InstantiatePlayer(spawnData.spawnLocation);
        }

        public GameObject GetRuntimePlayer()
        {
            return runtimePlayer;
        }

        [ContextMenu("Update Spawn Location")]
        public void UpdateSpawnLocationFromDummy()
        {
            UpdatePlayerLocationInSpawnData();
        }

        public Vector2 UpdatePlayerLocationInSpawnData()
        {
            if (playerDummy != null)
            {
                spawnData.spawnLocation = playerDummy.transform.position;
                return spawnData.spawnLocation;
            }
            else
            {
                Debug.LogError("PlayerDummy is not assigned.");
                return spawnData.spawnLocation; // Return the current spawn location in case of error
            }
        }

        private GameObject InstantiatePlayer(Vector2 spawnLocation)
        {
            if (GameObject.FindGameObjectWithTag("Player") == null)
            {
                GameObject player = Instantiate(playerPrefab, spawnLocation, Quaternion.identity);
                player.tag = "Player";

                // Set player facing direction based on the spawnFacingLeft field
                FlipPlayerIfNecessary(player);

                print("New player");
                player.GetComponent<PlayerController>().VerticalTransition(spawnFacingLeft);

                return player;
            }
            else
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.transform.position = spawnLocation;

                print("existing player");
                // Set player facing direction if necessary
                FlipPlayerIfNecessary(player);

                if (spawnData.needsUpwardForce)
                {
                    player.GetComponent<PlayerController>().VerticalTransition(spawnFacingLeft);
                    spawnData.needsUpwardForce = false;
                }

                return player;
            }
        }

        private void FlipPlayerIfNecessary(GameObject player)
        {
            // Access the PlayerController component
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("PlayerController not found on the player!");
                return;
            }

            // Get the correct SpriteRenderer via PlayerController
            SpriteRenderer spriteRenderer = playerController.getPlayerSpriteRenderer();
            if (spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer not found via PlayerController!");
                return;
            }

            Debug.Log("Flip player left: " + spawnFacingLeft);
        
            // Flip the sprite based on the spawnFacingLeft field
            spriteRenderer.flipX = spawnFacingLeft;
        }


        public void SaveScene()
        {
            ES3.Save("PlayerScene", SceneManager.GetActiveScene().name);
        }

        public void LoadScene()
        {
            LoadPlayerScene();
            // LoadPlayerTransform();
        }

        public void ClearPlayerSceneSave()
        {
            if (ES3.KeyExists("PlayerScene"))
            {
                ES3.DeleteKey("PlayerScene");
            }
        }

        private void LoadPlayerScene()
        {
            if (ES3.KeyExists("PlayerScene") && ES3.Load<string>("PlayerScene") == SceneManager.GetActiveScene().name)
            {
                UpdateSpawnLocation();
            }
            else if (ES3.KeyExists("PlayerScene") && ES3.Load<string>("PlayerScene") != SceneManager.GetActiveScene().name)
            {
                UpdateSpawnLocation();
                SceneManager.LoadScene((string)ES3.Load("PlayerScene"));
            }
            else
            {
                Debug.LogError("No PlayerScene save to load!");
            }
        }

        private void UpdateSpawnLocation()
        {
            spawnData.spawnLocation = ES3.Load<Transform>("PlayerTransform").position;
        }

        private void LoadPlayerTransform()
        {
            if (ES3.KeyExists("PlayerTransform"))
            {
                InstantiatePlayer(ES3.Load<Transform>("PlayerTransform").position);
            }
            else
            {
                Debug.LogError("No PlayerTransform save to load!");
            }
        }
    }
}
