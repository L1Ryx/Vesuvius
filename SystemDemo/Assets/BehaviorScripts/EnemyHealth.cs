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

    [Header("Flash Settings")]
    [SerializeField] private bool shouldFlashOnDamage = true; // Toggle for flashing
    [SerializeField] private Color flashColor = Color.red; // Color to flash
    [SerializeField] private float flashDuration = 0.1f; // Time per flash
    [SerializeField] private int flashCount = 2; // Number of flashes

    [Header("References")]
    public SpriteRenderer sr; // Reference to the enemy's SpriteRenderer

    [Header("Private Fields")]
    private bool hit;
    private int currentHealth;
    private Color originalColor; // Store the original color of the sprite

    void Start()
    {
        currentHealth = healthAmount;

        // Ensure SpriteRenderer is assigned
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }

        if (sr != null)
        {
            originalColor = sr.color; // Save the original color
        }
    }

    private void Death()
    {
        // Placeholder for better death logic
        gameObject.SetActive(false);
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
}
