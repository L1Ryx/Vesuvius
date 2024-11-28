using System.Collections;
using TarodevController;
using Unity.Profiling;
using UnityEngine;

/*
* Collision fix applied 
* Time slow fix applied
* No Invoke fix applied
* Velocity fix applied
* Additional time fixes applied
*/
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
    [Header("Visuals")]
    public Color originalColor;
    public Color damageColor;
    public bool useTimeStop = false;
    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem damageBurst; // Reference to the Damage Burst particle system


    private bool isInvincible = false;
    private bool isTimeStopped = false;
    private Rigidbody2D playerRb;
    private SpriteRenderer spriteRenderer;

    private int playerLayer;
    private int enemyLayer;
    private bool shouldIgnoreCollision = false;
    private bool shouldResetCollision = false;
    private Vector2 pendingKnockback = Vector2.zero;
    private bool applyKnockback = false;


    // private void Awake()
    // {
    //     playerRb = playerParent.GetComponent<Rigidbody2D>();
    //     spriteRenderer = playerSprite.GetComponent<SpriteRenderer>();
    //     playerHealing = playerParent.GetComponent<PlayerHealing>();

    //     playerLayer = LayerMask.NameToLayer("Player");
    //     enemyLayer = LayerMask.NameToLayer("Enemy");
    // }

    // void Start() {
    //     originalColor = spriteRenderer.color;
    // }

    // private void Update()
    // {
    //     // Handle collision layer changes outside of physics callbacks
    //     if (shouldIgnoreCollision)
    //     {
    //         Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
    //         shouldIgnoreCollision = false;
    //     }

    //     if (shouldResetCollision)
    //     {
    //         Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
    //         shouldResetCollision = false;
    //     }

    // if (isTimeStopped)
    // {
    //     timeStopMaxDuration -= Time.unscaledDeltaTime;
    //     if (timeStopMaxDuration <= 0)
    //     {
    //         timeStopMaxDuration = 0; // Ensure it doesn't go negative
    //         ResumeTimeEffect();
    //     }
    // }

    // }

    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.TryGetComponent(out EnemyHealth enemyHealth))
    //     {
    //         HandleCollisionWithEnemy(enemyHealth, collision);
    //     }
    // }

    // private void HandleCollisionWithEnemy(EnemyHealth enemy, Collision2D collision)
    // {
    //     // Skip processing if the enemy is dead
    //     if (enemy.GetIsDead())
    //     {
    //         // Permanently ignore collision with the dead enemy
    //         Collider2D enemyCollider = collision.collider;
    //         Collider2D playerCollider = GetComponent<Collider2D>();

    //         if (playerCollider != null && enemyCollider != null)
    //         {
    //             Physics2D.IgnoreCollision(playerCollider, enemyCollider, true);
    //         }
    //         return;
    //     }

    //     if (!isInvincible)
    //     {
    //         playerHealing.CancelHeal();
    //         isInvincible = true;
    //         if (useTimeStop) {
    //             TurnToTimeStopColor();
    //         }
            

    //         // Temporarily ignore collision layers for invincibility
    //         shouldIgnoreCollision = true;

    //         if (useTimeStop) {
    //             StartCoroutine(StartStopTimeEffectCoroutine());
    //         } else {
    //             playerInfo.DecrementHealth();

    //             // Trigger the Damage Burst particle effect
    //             if (damageBurst != null)
    //             {
    //                 damageBurst.Play();
    //             }
    //             else
    //             {
    //                 Debug.LogWarning("Damage Burst particle system is not assigned!");
    //             }

    //             StartCoroutine(FlashPlayer());
    //         }
            
    //         StartCoroutine(ResetInvincibilityCoroutine());

    //         Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
    //         pendingKnockback = Vector2.ClampMagnitude(knockbackDirection * playerKnockbackVelocity, 1000f);
    //         applyKnockback = true;
    //     }
    // }



    // private void FixedUpdate()
    // {
    //     if (applyKnockback)
    //     {
    //         playerRb.linearVelocity = pendingKnockback;
    //         applyKnockback = false;
    //     }
    // }


    // private IEnumerator StartStopTimeEffectCoroutine()
    // {
    //     yield return new WaitForSecondsRealtime(timeStopDelay);
    //     StartStopTimeEffect();
    // }

    // private IEnumerator ResetInvincibilityCoroutine()
    // {
    //     yield return new WaitForSecondsRealtime(playerIFrames);
    //     ResetInvincibility();
    // }


    // private void TurnToTimeStopColor() {
    //     spriteRenderer.color = damageColor;
    // }

    // private void RevertToOriginalColor() {
    //     spriteRenderer.color = originalColor;
    // }

    // private IEnumerator FlashPlayer()
    // {
    //     float elapsed = 0;
    //     Color originalColor = spriteRenderer.color;
    //     Color flashColor = originalColor;
    //     flashColor.a = lowAlpha;

    //     try
    //     {
    //         while (elapsed < playerIFrames - timeStopDelay)
    //         {
    //             spriteRenderer.color = flashColor;
    //             yield return new WaitForSecondsRealtime(flashFrequency / 2);
    //             spriteRenderer.color = originalColor;
    //             yield return new WaitForSecondsRealtime(flashFrequency / 2);
    //             elapsed += flashFrequency;
    //         }
    //     }
    //     finally
    //     {
    //         spriteRenderer.color = originalColor;
    //     }
    // }


    // private void StartStopTimeEffect()
    // {
    //     isTimeStopped = true;
    //     timeStopMaxDuration = timeStopDuration; // Reset the time stop duration
    //     Time.timeScale = timeStopScaleValue;
    //     Time.fixedDeltaTime = 0.02f * Time.timeScale; // Adjust fixed delta time
    // }


    // private void ResetInvincibility()
    // {
    //     isInvincible = false;
    //     // Set a flag to reset collision layers
    //     shouldResetCollision = true;
    // }

    // private void ResumeTimeEffect()
    // {
    //     isTimeStopped = false;
    //     Time.timeScale = 1f; // Reset time scale
    //     Time.fixedDeltaTime = 0.02f; // Reset fixed delta time

    //     RevertToOriginalColor();
    //     playerInfo.DecrementHealth();

    //     // Trigger the Damage Burst particle effect
    //     if (damageBurst != null)
    //     {
    //         damageBurst.Play();
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Damage Burst particle system is not assigned!");
    //     }

    //     StartCoroutine(FlashPlayer());
    //     timeStopMaxDuration = timeStopDuration;
    // }

}