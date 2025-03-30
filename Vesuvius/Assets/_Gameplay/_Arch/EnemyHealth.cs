using System.Collections;
using _ScriptableObjects.PlayerInfo;
using UnityEngine;
using UnityEngine.Events;

namespace _Gameplay._Arch
{
    public class EnemyHealth : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool damageable = true;
        [SerializeField] private int healthAmount = 100;
        [SerializeField] private float iFramesTime = 0.2f;
        public bool giveUpwardsForce = false;
        public bool canBeKnockedBack = true;
        public bool isLiveEnemy = true;

        [Header("Flash Settings")]
        [SerializeField] private bool shouldFlashOnDamage = true; // Toggle for flashing
        [SerializeField] private Color flashColor = Color.red; // Color to flash
        [SerializeField] private float flashDuration = 0.1f; // Time per flash
        [SerializeField] private int flashCount = 2; // Number of flashes

        [Header("Death Settings")]
        [SerializeField] private Color deathColor = Color.gray; // Color to turn upon death
        [SerializeField] private float upwardForce = 5f; // Force to project upwards
        [SerializeField] private float fadeDuration = 10f; // Time before fading out
        [SerializeField] private ParticleSystem deathBurst; // Particle system for death burst

        [Header("References")]
        public SpriteRenderer sr; // Reference to the enemy's SpriteRenderer
        private Rigidbody2D rb; // Reference to the enemy's Rigidbody2D
        private bool isDead = false; // Flag to indicate if the enemy is dead
        [SerializeField] private PlayerInfo playerInfo;
        [Header("Totem Power Settings")]
        [SerializeField] private int minTotemPower = 10; // Minimum totem power to grant
        [SerializeField] private int maxTotemPower = 20; // Maximum totem power to grant
        [Header("Time Slowdown Settings")]
        [SerializeField] private float slowdownTime = 0.2f; // Duration of the slowdown
        [SerializeField] private float slowdownScale = 0.05f; // Time scale during the slowdown
        [Header("Events")]
        [SerializeField] private UnityEvent RebalanceTutorialNeeded;


        [Header("Private Fields")]
        private bool hit;
        private int currentHealth;
        private Color originalColor; // Store the original color of the sprite


        void Start()
        {
            currentHealth = healthAmount;

            // Ensure SpriteRenderer and Rigidbody2D are assigned
            if (sr == null)
            {
                sr = GetComponent<SpriteRenderer>();
            }
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
                if (rb == null) {
                    rb = GetComponentInParent<Rigidbody2D>();
                }

                if (rb == null) {
                    Debug.LogError("rb is null in EnemyHealth");
                }
            }

            if (sr != null)
            {
                originalColor = sr.color; // Save the original color
            }
        }

        public void Death(Vector2 knockbackForce)
        {
            if (!isLiveEnemy) return;

            isDead = true;
            isLiveEnemy = false; // Mark as no longer a live enemy
            damageable = false; // Make the enemy unhittable

            gameObject.layer = LayerMask.NameToLayer("DeadEnemy");

            // Change sprite appearance
            if (sr != null)
            {
                sr.color = deathColor;
            }
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z); // Flip upside down

            // Apply upward force combined with knockback
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; // Reset velocity before applying forces
                rb.AddForce(knockbackForce + Vector2.up * upwardForce, ForceMode2D.Impulse); // Combine knockback and upward forces
            }
            else
            {
                Debug.LogError("Rigidbody2D not found on EnemyHealth or its parent.");
            }

            // Play death burst particle system
            if (deathBurst != null)
            {
                deathBurst.Play();
            }

            GrantTotemPower();

            // Trigger the time slowdown
            StartCoroutine(TimeSlowdownCoroutine());

            // Start fade-out coroutine
            StartCoroutine(FadeOutAndDestroy());

            if (playerInfo.GetCurrentHealth() < playerInfo.GetMaximumHealth()) {
                RebalanceTutorialNeeded.Invoke();
            }
        }



        public bool GetIsDead() {
            return isDead;
        }
        public void Damage(int amount, Vector2 knockbackForce)
        {
            if (!damageable || hit) return; // Prevent taking damage if not damageable or in iFrames

            hit = true;
            currentHealth -= amount;

            if (shouldFlashOnDamage && sr != null)
            {
                StartCoroutine(FlashSprite());
            }

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Death(knockbackForce); // Pass knockback force to Death
            }
            else
            {
                StartCoroutine(TurnOffHit());
            }
        }


        private IEnumerator TurnOffHit()
        {
            yield return new WaitForSeconds(iFramesTime);
            hit = false;
        }

        private IEnumerator FlashSprite()
        {
            if (sr == null) yield break;

            for (int i = 0; i < flashCount; i++)
            {
                sr.color = flashColor; // Change to flash color
                yield return new WaitForSeconds(flashDuration); // Wait for half the flash duration

                sr.color = originalColor; // Revert to original color
                yield return new WaitForSeconds(flashDuration); // Wait for the other half
            }
        }

        private void GrantTotemPower()
        {
            if (playerInfo != null)
            {
                int addedTotemPower = UnityEngine.Random.Range(minTotemPower, maxTotemPower + 1);
                playerInfo.AddTotemPower(addedTotemPower);
            }
            else
            {
                Debug.LogError("PlayerInfo is not assigned. Cannot add totem power.");
            }
        }

        private IEnumerator TimeSlowdownCoroutine()
        {
            Time.timeScale = slowdownScale; // Apply the slowdown scale
            // Time.fixedDeltaTime = 0.02f * Time.timeScale; // Adjust fixed delta time for physics
            yield return new WaitForSecondsRealtime(slowdownTime); // Wait in real-time
            Time.timeScale = 1f; // Reset time scale
            // Time.fixedDeltaTime = 0.02f; // Reset fixed delta time
        }


        private IEnumerator FadeOutAndDestroy()
        {
            if (sr == null) yield break;

            float elapsedTime = 0f;
            Color startColor = sr.color;
            Color targetColor = startColor;
            targetColor.a = 0f; // Target fully transparent

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                sr.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration);
                yield return null;
            }

            Destroy(gameObject); // Destroy the enemy object after fading out
        }

        public int GetCurrentHealth()
        {
            return currentHealth;
        }

    }
}
