using System;
using System.Collections;
using _ScriptableObjects.PlayerInfo;
using Events._Arch;
using Public.Tarodev_2D_Controller.Scripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Gameplay._Arch
{
    public class PlayerDamage : MonoBehaviour
    {
        [Header("References")]
        public GameObject playerParent;
        public PlayerController playerController;
        public PlayerInfo playerInfo;
        public Transform playerSprite;
        public PlayerHealing playerHealing;

        [Header("Damage Settings")]
        public float playerIFrames = 2f;
        public float playerKnockbackVelocity = 750f;
        public float lowAlpha = 0.3f;
        public float flashFrequency = 0.2f;
        public float timeStopDelay = 0.06f;
        public float timeStopMaxDuration = 0.2f;
        public float timeStopDuration = 0.2f;
        public float timeStopScaleValue = 0.05f;
        public float swingKnockbackCooldown = 0.2f; 

        [Header("Visuals")]
        public Color originalColor;
        public Color damageColor;
        public bool useTimeStop = false;

        [Header("Particle Effects")]
        [SerializeField] private ParticleSystem damageBurst;
        public bool useParticles = true; // Set this in the Inspector to toggle particle effects
        [Header("Events")]
        public UnityEvent playerDamaged;

        private bool isInvincible = false;
        private bool isTimeStopped = false;
        private Rigidbody2D playerRb;
        private SpriteRenderer spriteRenderer;

        private bool isFlashing = false;

        [Header("Layer Settings")]
        [Tooltip("Assign the Player layer in the Inspector.")]
        public LayerMask playerLayerMask;

        [Tooltip("Assign the Enemy layer in the Inspector.")]
        public LayerMask enemyLayerMask;
        private int playerLayer;
        private int enemyLayer;

        private Vector2 pendingKnockback = Vector2.zero;

        [Header("Knockback")]
        public float maxKnockbackVelocity = 10f;
        public float finalMaxKnockbackVelocity = 20f;
        public float velocityClampDuration = 0.15f; // Duration to clamp velocity after damage
        public float maxAllowedVelocity = 20f; // The max velocity we allow after taking damage

        private float velocityClampEndTime = 0f; // Time when clamping should stop
        [Header("Death Overlay")]
        public GameObject deathOverlayPanel; // Assign the black overlay image in the Inspector
        public float deathDelay = 4f; // Time to wait before reloading the checkpoint
        public SpawnData spawnData; // Reference to the SpawnData ScriptableObject
        [Header("Debug Info")]
        public Vector2 currentVelocity; // Tracks the player's velocity
        public float maxVelocityMagnitude = 0f; // Tracks the highest velocity recorded


        private void Awake()
        {
            playerRb = playerParent.GetComponent<Rigidbody2D>();
            spriteRenderer = playerSprite.GetComponent<SpriteRenderer>();
            playerHealing = playerParent.GetComponent<PlayerHealing>();

            playerLayer = LayerMaskToLayer(playerLayerMask);
            enemyLayer = LayerMaskToLayer(enemyLayerMask);
        }

        private int LayerMaskToLayer(LayerMask layerMask)
        {
            int layer = (int)Mathf.Log(layerMask.value, 2);
            if (layer < 0 || layer > 31)
                throw new ArgumentOutOfRangeException(nameof(layer), "LayerMask must represent a valid layer index (0–31).");
            return layer;
        }

        void Start()
        {
            originalColor = spriteRenderer.color;
        }

        private void Update()
        {
            if (isTimeStopped)
            {
                timeStopMaxDuration -= Time.unscaledDeltaTime;
                if (timeStopMaxDuration <= 0)
                { 
                    ResumeTimeEffect();
                }
            }

            if (playerRb != null)
            {
                // Store the player's current velocity
                currentVelocity = playerRb.linearVelocity;

                // If within velocity clamp duration, limit velocity magnitude
                if (Time.time < velocityClampEndTime)
                {
                    float currentMagnitude = playerRb.linearVelocity.magnitude;
                    if (currentMagnitude > maxAllowedVelocity)
                    {
                        // Scale down velocity to not exceed maxAllowedVelocity
                        playerRb.linearVelocity = playerRb.linearVelocity.normalized * maxAllowedVelocity;
                    }
                }

                // Update the max velocity magnitude if the current velocity exceeds it
                float velocityMagnitude = playerRb.linearVelocity.magnitude;
                if (velocityMagnitude > maxVelocityMagnitude)
                {
                    maxVelocityMagnitude = velocityMagnitude;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {

            if (collision.gameObject == null) return;
            if (collision.gameObject.CompareTag("Enemy"))
            {
                print("colliding");
                EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>()
                                          ?? collision.gameObject.GetComponentInChildren<EnemyHealth>();

                if (enemyHealth != null)
                {
                    HandleCollisionWithEnemy(enemyHealth, collision);
                }
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.gameObject.CompareTag("Hazard"))
            {
                print("take damage");
                HandleCollisionWithHazard(collision);
            }
        }

        // Call this when the player takes damage
        private void HandleCollisionWithEnemy(EnemyHealth enemy, Collision2D collision)
        {
            if (playerInfo.GetCurrentHealth() <= 0) return;
            if (enemy.GetIsDead()) return;
            if (!isInvincible)
            {
                playerHealing?.CancelHeal();
                isInvincible = true;
                playerDamaged.Invoke();

                playerRb.linearVelocity = Vector2.zero;

                if (useTimeStop) StartCoroutine(StartStopTimeEffectCoroutine());

                Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

                if (useParticles && damageBurst != null && !damageBurst.isPlaying) damageBurst.Play();
                if (!isFlashing) StartCoroutine(FlashPlayer());
                StartCoroutine(ResetInvincibilityCoroutine());

                // **Enable velocity clamping for a short period**
                velocityClampEndTime = Time.time + velocityClampDuration;

                Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
                pendingKnockback = knockbackDirection * Mathf.Min(playerKnockbackVelocity, finalMaxKnockbackVelocity);

                ApplyKnockback();
            }
        }

        private void HandleCollisionWithHazard( Collider2D collision)
        {
            if (playerInfo.GetCurrentHealth() <= 0) return;
            if (!isInvincible)
            {
                playerHealing?.CancelHeal();
                isInvincible = true;
                playerDamaged.Invoke();

                playerRb.linearVelocity = Vector2.zero;

                if (useTimeStop) StartCoroutine(StartStopTimeEffectCoroutine());

                Physics2D.IgnoreLayerCollision(playerLayer, 14, true);

                if (useParticles && damageBurst != null && !damageBurst.isPlaying) damageBurst.Play();
                if (!isFlashing) StartCoroutine(FlashPlayer());
                StartCoroutine(ResetInvincibilityCoroutine());

                // **Enable velocity clamping for a short period**
                velocityClampEndTime = Time.time + velocityClampDuration;

                Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
                pendingKnockback = knockbackDirection * Mathf.Min(playerKnockbackVelocity, finalMaxKnockbackVelocity);

                ApplyKnockback();
            }
        }


        private void ApplyKnockback()
        {
            // Hard reset player velocity before applying knockback
            playerRb.linearVelocity = Vector2.zero;  

            // Clamp knockback force to prevent extreme movement
            Vector2 clampedKnockback = Vector2.ClampMagnitude(pendingKnockback, finalMaxKnockbackVelocity);
        
            // Apply knockback force
            playerRb.AddForce(clampedKnockback, ForceMode2D.Impulse);

            // Clear pending knockback to prevent additional stacking
            pendingKnockback = Vector2.zero;
        }



        private IEnumerator StartStopTimeEffectCoroutine()
        {
            yield return new WaitForSecondsRealtime(timeStopDelay);
            StartStopTimeEffect();
        }

        private IEnumerator ResetInvincibilityCoroutine()
        {
            yield return new WaitForSecondsRealtime(playerIFrames);
            Physics2D.IgnoreLayerCollision(playerLayer, 14, false);
            ResetInvincibility();
        }

        private void TurnToTimeStopColor()
        {
            spriteRenderer.color = damageColor;
        }

        private void RevertToOriginalColor()
        {
            spriteRenderer.color = originalColor;
        }

        private IEnumerator FlashPlayer()
        {
            if (isFlashing) yield break;
            isFlashing = true;

            float elapsed = 0;
            Color flashColor = originalColor;
            flashColor.a = lowAlpha;

            try
            {
                while (elapsed < playerIFrames - timeStopDelay)
                {
                    spriteRenderer.color = flashColor;
                    yield return new WaitForSecondsRealtime(flashFrequency / 2);
                    spriteRenderer.color = originalColor;
                    yield return new WaitForSecondsRealtime(flashFrequency / 2);
                    elapsed += flashFrequency;
                }
            }
            finally
            {
                spriteRenderer.color = originalColor;
                isFlashing = false;
            }
        }

        private void StartStopTimeEffect()
        {
            isTimeStopped = true;
            timeStopMaxDuration = timeStopDuration;
            Time.timeScale = timeStopScaleValue;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        private void ResetInvincibility()
        {
            isInvincible = false;
        }

        private void ResumeTimeEffect()
        {
            isTimeStopped = false;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;

            RevertToOriginalColor();
            if (playerInfo.DecrementHealth()) {
                StartCoroutine(HandlePlayerDeath());
                return; // Prevent further processing
            }


            if (useParticles && damageBurst != null)
            {
                damageBurst.Play();
            }

            StartCoroutine(FlashPlayer());
        }

        private IEnumerator HandlePlayerDeath()
        {
            // Activate the death overlay
            Image overlayImage = deathOverlayPanel.GetComponent<Image>();
            if (overlayImage != null)
            {
                Color overlayColor = overlayImage.color;
                overlayColor.a = 1f;
                overlayImage.color = overlayColor;
                deathOverlayPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("Death overlay panel is missing an Image component!");
            }

            // Wait for the death delay
            yield return new WaitForSeconds(deathDelay);

            // Set spawn location in SpawnData
            if (playerInfo != null && spawnData != null)
            {
                spawnData.spawnLocation = playerInfo.GetCheckpointLocation();
                Debug.Log($"SpawnData updated: Location {spawnData.spawnLocation}");
            }
            else
            {
                Debug.LogError("PlayerInfo or SpawnData is not assigned!");
            }

            // Refill health and totem power
            if (playerInfo != null)
            {
                playerInfo.RefillHealthAndTotemPower();
            }

            // Load the checkpoint scene
            if (playerInfo != null)
            {
                string checkpointScene = playerInfo.GetCheckpointScene();
                if (!string.IsNullOrEmpty(checkpointScene))
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(checkpointScene);
                }
                else
                {
                    Debug.LogError("Checkpoint scene is not set in PlayerInfo!");
                }
            }
        }


    

    }
}
