using _ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Gameplay._Arch
{
    public class GateSwitch : MonoBehaviour
    {
        [Header("UI References")]
        public Canvas gateSwitchCanvas;
        public TMP_Text interactPromptText;
        public ControlPrompts controlPrompts;

        [Header("Gate Reference")]
        public Gate gate;

        [Header("Sprites")]
        public SpriteRenderer switchSpriteRenderer;
        public Sprite lockedSprite;
        public Sprite unlockedSprite;

        [Header("Settings")]
        public float activationRadius = 5f;
        public float lerpSpeed = 2f;
        public float pulseSpeed = 1.2f;
        public float pulseIntensity = 0.06f;

        private Color targetColor;
        protected bool isPlayerNear = false;
        protected bool gateUnlocked = false;
        private GameObject player;

        private PlayerControls playerControls;
        private SwitchAudio switchAudio;

        private void Awake()
        {
            playerControls = new PlayerControls();
            switchAudio = GetComponent<SwitchAudio>();
        }

        private void OnEnable()
        {
            playerControls.Player.Interact.performed += OnInteract;
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Player.Interact.performed -= OnInteract;
            playerControls.Disable();
        }

        //possible race condition currently, need to fix.
        public void OnControlsChanged()
        {
            print("Controls Changed");
            UpdateUIHints(regenerate: true); // Force re-generation of our cached text strings to pick up new bindings.
        }

        private void UpdateUIHints(bool regenerate = false)
        {
            interactPromptText.text = controlPrompts.interactPrompt;
        }

        private void Start()
        {
            player = FindObjectOfType<PlayerSpawner>()?.GetRuntimePlayer();
            if (player == null)
            {
                Debug.LogError("Player not found! Ensure PlayerSpawner and runtime player setup is correct.");
            }

            InitializeUI();
            UpdateSwitchSprite(); // Set the correct sprite at the start
        }

        protected virtual void InitializeUI()
        {
            gateSwitchCanvas.gameObject.SetActive(false);
            SetTextAlpha(0);
        }

        private void Update()
        {
            if (gateUnlocked || player == null || gate == null)
            {
                // Skip logic if the gate is unlocked or required references are missing
                return;
            }
            HandleSwitchProximityLogic();
        }

        protected virtual void HandleSwitchProximityLogic()
        {
            if (!gate.gateData.GetGateLockedState(gate.gateID))
            {
                // Gate is already unlocked, ensure prompt stays deactivated
                gateUnlocked = true;
                DeactivateSwitch();
                UpdateSwitchSprite(); // Update sprite to unlocked state
                return;
            }

            float distance = Vector3.Distance(player.transform.position, transform.position);
            isPlayerNear = distance <= activationRadius;

            if (isPlayerNear)
            {
                ActivateSwitch();
            }
            else
            {
                DeactivateSwitch();
            }

            LerpTextAlpha();
        }

        private void ActivateSwitch()
        {
            if (!gateSwitchCanvas.gameObject.activeSelf)
            {
                gateSwitchCanvas.gameObject.SetActive(true);
            }
            SetTargetAlpha(1);
        }

        private void DeactivateSwitch()
        {
        
            SetTargetAlpha(0);
            if (GetTextAlpha() <= 0.01f && gateSwitchCanvas.gameObject.activeSelf)
            {
                gateSwitchCanvas.gameObject.SetActive(false);
                Debug.Log("Prompt deactivated.");
            }
        }

        private void LerpTextAlpha()
        {
            var currentColor = interactPromptText.color;
            interactPromptText.color = Color.Lerp(
                currentColor,
                new Color(currentColor.r, currentColor.g, currentColor.b, targetColor.a),
                Time.deltaTime * lerpSpeed
            );
        }

        private void SetTargetAlpha(float alpha)
        {
            targetColor = new Color(interactPromptText.color.r, interactPromptText.color.g, interactPromptText.color.b, alpha);
        }

        private float GetTextAlpha()
        {
            return interactPromptText.color.a;
        }

        private void SetTextAlpha(float alpha)
        {
            interactPromptText.color = new Color(interactPromptText.color.r, interactPromptText.color.g, interactPromptText.color.b, alpha);
        }

        protected virtual void OnInteract(InputAction.CallbackContext context)
        {
            if (isPlayerNear && gate != null && !gateUnlocked)
            {
                if (gate.gateData.GetGateLockedState(gate.gateID))
                {
                    Debug.Log($"Unlocking gate {gate.gateID} via switch.");
                    gate.UnlockGate();

                    // Immediately deactivate the prompt
                    gateSwitchCanvas.gameObject.SetActive(false);
                    SetTextAlpha(0);
                    gateUnlocked = true;

                    UpdateSwitchSprite(); // Update sprite to unlocked state
                    Debug.Log("Gate unlocked and prompt deactivated.");

                    if (switchAudio != null) {
                        switchAudio.PlaySwitchOpen();
                    }
                }
            }
        }

        private void UpdateSwitchSprite()
        {
            if (switchSpriteRenderer == null || lockedSprite == null || unlockedSprite == null)
            {
                Debug.LogWarning("Switch sprite references are not set properly.");
                return;
            }

            bool isLocked = gate.gateData.GetGateLockedState(gate.gateID);
            switchSpriteRenderer.sprite = isLocked ? lockedSprite : unlockedSprite;
            Debug.Log($"Switch sprite updated to: {(isLocked ? "Locked" : "Unlocked")}");
        }
    }
}
