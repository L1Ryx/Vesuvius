using _ScriptableObjects;
using Public.Tarodev_2D_Controller.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Gameplay._Arch
{
    public class EmptyCradle : MonoBehaviour
    {
        [Header("UI References")]
        public Canvas cradleCanvas;
        public TMP_Text promptText;
        public ControlPrompts controlPrompts;

        [Header("Settings")]
        public float activationRadius = 2f; // Distance for interaction
        public GameObject menuPrefab; // Prefab for the temporary menu

        [Header("Checkpoint Info")]
        public string checkpointScene; // Scene to set as checkpoint
        public Vector2 checkpointLocation; // Exact location to set as checkpoint

        private GameObject player;
        private PlayerController playerController; // Dynamically fetched PlayerController
        private PlayerControls playerControls;
        private bool isPlayerNear = false; // Tracks if the player is near
        private bool menuActive = false; // Tracks if the menu is active
        private GameObject instantiatedMenu;

        private Color targetColor;
        public float lerpSpeed = 2f;

        private void Awake()
        {
            playerControls = new PlayerControls();
        }

        private void OnEnable()
        {
            playerControls.Player.Interact.performed += OnInteractPerformed;
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Player.Interact.performed -= OnInteractPerformed;
            playerControls.Disable();
        }

        public void OnControlsChanged()
        {
            UpdateUIHints(); // Force re-generation of our cached text strings to pick up new bindings.
        }

        private void UpdateUIHints()
        {
            promptText.text = controlPrompts.interactPrompt;
        }

        private void Start()
        {
            player = FindObjectOfType<PlayerSpawner>()?.GetRuntimePlayer();
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

            InitializeUI();
        }

        private void InitializeUI()
        {
            cradleCanvas.gameObject.SetActive(false);
            SetTextAlpha(0);
        }

        private void Update()
        {
            if (menuActive)
            {
                HandlePromptLerpOut(); // Smoothly lerp the prompt out when the menu is active
                return;
            }

            HandleProximityLogic();
        }

        private void HandleProximityLogic()
        {
            if (player == null) return;

            float distance = Vector3.Distance(player.transform.position, transform.position);
            isPlayerNear = distance <= activationRadius;

            if (isPlayerNear)
            {
                ActivatePrompt();
            }
            else
            {
                DeactivatePrompt();
            }

            LerpTextAlpha();
        }

        private void ActivatePrompt()
        {
            if (!cradleCanvas.gameObject.activeSelf)
            {
                cradleCanvas.gameObject.SetActive(true);
            }
            SetTargetAlpha(1);
        }

        private void DeactivatePrompt()
        {
            SetTargetAlpha(0);
            if (GetTextAlpha() <= 0.01f && cradleCanvas.gameObject.activeSelf)
            {
                cradleCanvas.gameObject.SetActive(false);
            }
        }

        private void HandlePromptLerpOut()
        {
            SetTargetAlpha(0); // Set the target alpha to 0 for a smooth fade-out
            LerpTextAlpha();

            if (GetTextAlpha() <= 0.01f && cradleCanvas.gameObject.activeSelf)
            {
                cradleCanvas.gameObject.SetActive(false); // Deactivate the canvas after fade-out
            }
        }

        private void LerpTextAlpha()
        {
            var currentColor = promptText.color;
            promptText.color = Color.Lerp(
                currentColor,
                new Color(currentColor.r, currentColor.g, currentColor.b, targetColor.a),
                Time.deltaTime * lerpSpeed
            );
        }

        private void SetTargetAlpha(float alpha)
        {
            targetColor = new Color(promptText.color.r, promptText.color.g, promptText.color.b, alpha);
        }

        private float GetTextAlpha()
        {
            return promptText.color.a;
        }

        private void SetTextAlpha(float alpha)
        {
            promptText.color = new Color(promptText.color.r, promptText.color.g, promptText.color.b, alpha);
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            if (menuActive)
            {
                return;
            }
            else if (isPlayerNear)
            {
                OpenMenu();
            }
        }

        private void OpenMenu()
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

            menuActive = true;

            // Freeze player movement
            playerController.SetFreezeMode(true);

            Debug.Log("Menu opened and player frozen.");
        }

        private void CloseMenu()
        {
            menuActive = false;

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
