using _ScriptableObjects;
using Public.Tarodev_2D_Controller.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Gameplay._Arch
{
    public class EmptyCradle : MonoBehaviour
    {
        [Header("Settings")]
        public GameObject menuPrefab; // Prefab for the temporary menu
        public UnsavedPlayerInfo unsavedPlayerInfo;

        [Header("Checkpoint Info")]
        public string checkpointScene; // Scene to set as checkpoint
        public Vector2 checkpointLocation; // Exact location to set as checkpoint

        private GameObject player;
        private PlayerController playerController; // Dynamically fetched PlayerController
        private GameObject instantiatedMenu;

        private void Start()
        {
            player = FindFirstObjectByType<PlayerSpawner>()?.GetRuntimePlayer();
            if (player == null)
            {
                Debug.LogError("Player not found! Ensure PlayerSpawner and runtime player setup is correct.");
                return;
            }

            // Dynamically fetch PlayerController from the player
            playerController = player.GetComponent<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("PlayerController component not found on the player.");
            }

        }

        public void OpenMenu()
        {
            if (playerController == null)
            {
                Debug.LogWarning("Cannot open menu. PlayerController not assigned.");
                return;
            }

            GameObject menuInstance = Instantiate(menuPrefab, this.transform);
            CradleMenu menuScript = menuInstance.GetComponent<CradleMenu>();

            // Set the menu's PlayerInfo reference
            //menuScript.playerInfo.SetCheckpoint(checkpointScene, checkpointLocation);

            menuScript.MenuClosed += CloseMenu; // Subscribe to the MenuClosed event

            unsavedPlayerInfo.isInMenuMode = true;

            // Freeze player movement
            playerController.SetFreezeMode(true);

            Debug.Log("Menu opened and player frozen.");
        }

        private void CloseMenu()
        {
            unsavedPlayerInfo.isInMenuMode = false;

            if (playerController == null)
            {
                Debug.LogWarning("Cannot close menu. PlayerController not assigned.");
                return;
            }

            // Unfreeze player movement
            playerController.SetFreezeMode(false);

            Debug.Log("Menu closed and player unfrozen.");
        }

        public string GetCheckpointScene()
        {
            return checkpointScene;
        }

        public Vector2 GetCheckpointLocation()
        {
            return checkpointLocation;
        }

    }
}
