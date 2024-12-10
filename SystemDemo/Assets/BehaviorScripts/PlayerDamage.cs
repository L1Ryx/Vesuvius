using System;
using System.Collections;
using TarodevController;
using UnityEngine;
using UnityEngine.Events;

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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == null) return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>()
                ?? collision.gameObject.GetComponentInChildren<EnemyHealth>();

            if (enemyHealth != null)
            {
                HandleCollisionWithEnemy(enemyHealth, collision);
            }
        }
    }

    private void HandleCollisionWithEnemy(EnemyHealth enemy, Collision2D collision)
    {
        try
        {
            if (enemy.GetIsDead())
            {
                Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>(), true);
                return;
            }

            if (!isInvincible)
            {
                playerHealing?.CancelHeal();
                isInvincible = true;
                playerDamaged.Invoke();

                if (useTimeStop) TurnToTimeStopColor();

                playerRb.linearVelocity = Vector2.zero;

                if (useTimeStop)
                {
                    StartCoroutine(StartStopTimeEffectCoroutine());
                }

                Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
                // playerInfo?.DecrementHealth();

                if (useParticles && damageBurst != null && !damageBurst.isPlaying)
                {
                    damageBurst.Play();
                }

                if (!isFlashing) StartCoroutine(FlashPlayer());

                StartCoroutine(ResetInvincibilityCoroutine());

                Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
                pendingKnockback = knockbackDirection * Mathf.Min(playerKnockbackVelocity, finalMaxKnockbackVelocity);

                ApplyKnockback();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error in HandleCollisionWithEnemy: " + ex.Message);
        }
    }

    private void ApplyKnockback()
    {
        playerRb.AddForce(pendingKnockback, ForceMode2D.Impulse);
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
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
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
        playerInfo.DecrementHealth();

        if (useParticles && damageBurst != null)
        {
            damageBurst.Play();
        }

        StartCoroutine(FlashPlayer());
    }
}
