using Public.Tarodev_2D_Controller.Scripts;
using ScriptableObjects.PlayerInfo;
using TarodevController;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Gameplay._Arch
{
    public class PlayerMelee : MonoBehaviour
    {
        [Header("References")]
        public GameObject swingPrefab;
        public GameObject playerParent;
        public Transform playerSprite;
        public PlayerController playerController;
        public PlayerInfo playerInfo;

        [Header("Swing Offsets")]
        public Vector2 rightSwingOffset = new Vector2(1f, 0f);
        public Vector2 leftSwingOffset = new Vector2(-1f, 0f);
        public Vector2 upSwingOffset = new Vector2(0f, 1f);
        public Vector2 downSwingOffset = new Vector2(0f, -1f);

        [Header("Swing Settings")]
        public float cooldownTime = 0.5f;
        public float swingDuration = 0.2f;
        public float defaultForce = 300f;
        public float upwardsForce = 600f;
        public float movementTime = 0.1f;
        public float sequenceTimeout = 1.0f; // Time (in seconds) before sequence resets

        [Header("Sheath Settings")]
        public Transform sheath;
        public Transform leftSheathPoint;
        public Transform rightSheathPoint;
        public float sheathLerpSpeed = 5f;
        [Header("Coyote Time Settings")]
        public float coyoteTimeDuration = 0.3f; // Time before the end of a swing to register a queued input
        private bool isSwingQueued = false; // Tracks if a swing input is queued
        private Vector2 queuedInputDirection; // Stores the direction of the queued swing
        [Header("Events")]
        public UnityEvent playerSwung;


        public bool IsSwinging { get; private set; }

        private PlayerControls swingControls;
        private PlayerInputActions moveControls;
        private float lastSwingTime;
        private Vector2 inputDirection;
        private Rigidbody2D playerRb;
        private SpriteRenderer spriteRenderer;
        private bool isAltSwing; // Tracks if the next swing should be alt slash

        private int playerLayer;
        private int enemyLayer;

        private void Awake()
        {
            swingControls = new PlayerControls();
            moveControls = new PlayerInputActions();
            playerRb = playerParent.GetComponent<Rigidbody2D>();
            spriteRenderer = playerSprite.GetComponent<SpriteRenderer>();

            playerLayer = LayerMask.NameToLayer("Player");
            enemyLayer = LayerMask.NameToLayer("Enemy");
        }

        private void OnEnable()
        {
            swingControls.Player.Swing.performed += OnSwing;
            swingControls.Player.Enable();

            moveControls.Player.Move.performed += OnMove;
            moveControls.Player.Move.canceled += ctx => inputDirection = Vector2.zero;
            moveControls.Enable();
        }

        private void OnDisable()
        {
            swingControls.Player.Swing.performed -= OnSwing;
            swingControls.Player.Disable();

            moveControls.Player.Move.performed -= OnMove;
            moveControls.Disable();
        }

        private void Update()
        {
            UpdateSheathPosition();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            inputDirection = context.ReadValue<Vector2>();
        }

        public Vector2 GetInputDirection() => inputDirection;

        private void OnSwing(InputAction.CallbackContext context)
        {
            // Check if swinging is currently allowed
            if (Time.time - lastSwingTime < cooldownTime || IsSwinging)
            {
                // If the player presses swing during the coyote time, queue the swing
                if (IsSwinging && swingDuration - (Time.time - lastSwingTime) <= coyoteTimeDuration)
                {
                    isSwingQueued = true;
                    queuedInputDirection = inputDirection; // Store the current input direction
                }
                return;
            }

            PerformSwing();
        }

        private void PerformSwing()
        {
            if (swingPrefab != null && playerController != null)
            {
                Vector3 swingPosition;
                Quaternion swingRotation;

                // Determine swing direction based on input
                if (inputDirection.y > 0)
                {
                    swingPosition = playerSprite.position + (Vector3)upSwingOffset;
                    swingRotation = Quaternion.Euler(0, 0, 90);
                }
                else if (inputDirection.y < 0)
                {
                    swingPosition = playerSprite.position + (Vector3)downSwingOffset;
                    swingRotation = Quaternion.Euler(0, 0, -90);
                }
                else if (playerController.IsFacingRight())
                {
                    swingPosition = playerSprite.position + (Vector3)rightSwingOffset;
                    swingRotation = Quaternion.identity;
                }
                else if (playerController.IsFacingLeft())
                {
                    swingPosition = playerSprite.position + (Vector3)leftSwingOffset;
                    swingRotation = Quaternion.Euler(0, 180f, 0);
                }
                else
                {
                    Debug.LogWarning("Player facing direction is ambiguous!");
                    return;
                }

                playerSwung.Invoke();

                GameObject swing = Instantiate(swingPrefab, swingPosition, swingRotation, transform);
                swing.GetComponentInChildren<Swing>().Init(playerController, this, playerParent, isAltSwing);

                UpdateSwingSequence();

                SetSheathActive(false);

                IsSwinging = true;
                Invoke(nameof(EndSwing), swingDuration);

                lastSwingTime = Time.time;
            }
            else
            {
                Debug.LogWarning("Swing prefab or PlayerController reference is missing!");
            }
        }



        private void UpdateSwingSequence()
        {
            // Check if the sequence has timed out
            if (Time.time - lastSwingTime > sequenceTimeout)
            {
                isAltSwing = false; // Reset to normal swing
            }
            else
            {
                isAltSwing = !isAltSwing; // Alternate swing
            }
        }

        private void EndSwing()
        {
            IsSwinging = false;
            SetSheathActive(true);

            // Check if a swing was queued during the coyote time
            if (isSwingQueued)
            {
                isSwingQueued = false; // Reset the queued swing flag
                inputDirection = queuedInputDirection; // Use the queued input direction
                PerformSwing(); // Perform the queued swing
            }
        }


        private void SetSheathActive(bool active)
        {
            if (sheath != null)
            {
                sheath.gameObject.SetActive(active);
            }
        }

        private void UpdateSheathPosition()
        {
            if (sheath == null || leftSheathPoint == null || rightSheathPoint == null || !sheath.gameObject.activeSelf)
            {
                return;
            }

            Vector3 targetPosition = playerController.IsFacingRight()
                ? leftSheathPoint.position
                : rightSheathPoint.position;

            sheath.position = Vector3.Lerp(sheath.position, targetPosition, sheathLerpSpeed * Time.deltaTime);
        }
    }
}