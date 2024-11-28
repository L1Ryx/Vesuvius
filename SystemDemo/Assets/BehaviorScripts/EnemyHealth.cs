using System.Collections;
using UnityEngine;

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
        }

        if (sr != null)
        {
            originalColor = sr.color; // Save the original color
        }
    }

    private void Death()
    {
        if (!isLiveEnemy) return;

        isDead = true;
        isLiveEnemy = false; // Mark as no longer a live enemy
        damageable = false; // Make the enemy unhittable

        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");

        // Change sprite appearance
        sr.color = deathColor;
        transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z); // Flip upside down

        // Apply upward force
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Reset velocity
            rb.AddForce(Vector2.up * upwardForce, ForceMode2D.Impulse);
        }

        // Play death burst particle system
        if (deathBurst != null)
        {
            deathBurst.Play();
        }

        // Start fade-out coroutine
        StartCoroutine(FadeOutAndDestroy());
    }

    public bool GetIsDead() {
        return isDead;
    }
    public void Damage(int amount)
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
            Death();
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
}
