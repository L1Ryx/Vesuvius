using System.Collections;
using Public.Tarodev_2D_Controller.Scripts;
using ScriptableObjects.PlayerInfo;
using UnityEngine;

namespace Gameplay._Arch
{
    public class Swing : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int damageAmount = 20;
        [SerializeField] private float knockbackForce = 5f; // Parameterized knockback force
        [SerializeField] private float knockbackCooldown = 0.2f; // cooldown to prevent stacking
        [Header("Data Cubes")]
        public PlayerInfo playerInfo;

        private PlayerController playerController;
        private PlayerMelee playerMelee;
        private GameObject playerParent;
        private Rigidbody2D rb;
        private Vector2 direction;
        private bool collided;
        private bool downwardStrike;
        private bool isAltSwing; // Tracks if this swing is alt variation
        private SwingAudio swingAudio;
        private float lastDamageTime = 0f;


        public void Init(PlayerController pc, PlayerMelee pm, GameObject playerParent, bool altSwing)
        {
            this.playerController = pc;
            this.playerMelee = pm;
            this.playerParent = playerParent;
            this.isAltSwing = altSwing;

            rb = playerParent.GetComponent<Rigidbody2D>();
            swingAudio = GetComponent<SwingAudio>();

            if (playerController == null) Debug.LogError("PlayerController is null in Swing.Init");
            if (playerMelee == null) Debug.LogError("PlayerMelee is null in Swing.Init");
            if (playerParent == null) Debug.LogError("PlayerParent is null in Swing.Init");
            if (rb == null) Debug.LogError("Rigidbody2D is missing on PlayerParent");

            // Set animation based on swing type
            PlayAnimation();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void PlayAnimation()
        {
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                string animationName = isAltSwing ? "SwingAttackAlt" : "SwingAttack1";
                animator.Play(animationName);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent(out EnemyHealth enemyHealth))
            {
                HandleCollision(enemyHealth, collision);
            }
        }


        private void HandleCollision(EnemyHealth objHealth, Collider2D collision)
        {
            // Reset velocity before applying new knockback
            rb.linearVelocity = Vector2.zero;

            // Calculate knockback direction
            Vector2 knockbackDirection = (collision.transform.position - playerParent.transform.position).normalized * knockbackForce;

            // Cap the force to avoid excessive movement
            knockbackDirection = Vector2.ClampMagnitude(knockbackDirection, knockbackForce);

            // Apply knockback immediately (overriding any previous knockback)
            rb.AddForce(knockbackDirection, ForceMode2D.Impulse);

            // Continue with attack logic as normal
            int enemyHealthBefore = objHealth.GetCurrentHealth();
            objHealth.Damage(damageAmount, knockbackDirection);

            if (objHealth.GetIsDead() && enemyHealthBefore > 0)
            {
                swingAudio.PlayPlayerAttackHit();
                swingAudio.PlayPlayerAttackKill();
            }
            else if (enemyHealthBefore > 0)
            {
                swingAudio.PlayPlayerAttackHit();
            }

            Vector2 inputDirection = playerMelee.GetInputDirection();

            if (inputDirection.y < 0 && !playerController.IsGrounded())
            {
                if (objHealth.giveUpwardsForce)
                {
                    direction = Vector2.up;
                    downwardStrike = true;
                    playerController.CancelJump();
                }
                else
                {
                    direction = Vector2.down;
                }
                collided = true;
            }
            else if (inputDirection.y == 0)
            {
                direction = playerController.IsFacingLeft() ? Vector2.right : Vector2.left;
                collided = true;
            }

            StartCoroutine(NoLongerColliding());
        }






        private void ApplyKnockback(EnemyHealth objHealth, Collider2D collision)
        {
            Rigidbody2D enemyRb = collision.GetComponent<Rigidbody2D>();
            if (enemyRb == null) {
                enemyRb = collision.GetComponentInParent<Rigidbody2D>();
            }
            if (enemyRb == null) {
                enemyRb = collision.GetComponentInChildren<Rigidbody2D>();
            }
            if (enemyRb == null) {
                Debug.LogError("no enemyRb in Swing");
            }
            if (enemyRb != null)
            {
                // Calculate knockback direction (opposite of the swing's force direction)
                Vector2 knockbackDirection = (collision.transform.position - playerParent.transform.position).normalized;
                Vector2 knockbackForceVector = knockbackDirection * knockbackForce;

                // Apply knockback via the TestEnemyMovement script
                var enemyMovement = collision.GetComponent<TestEnemyMovement>();
                if (enemyMovement != null)
                {
                    enemyMovement.ApplyKnockback(knockbackForceVector);
                }
                var watcherMovement = collision.GetComponentInParent<WatcherAI>();
                if (watcherMovement != null) {
                    watcherMovement.ApplyKnockback(knockbackForceVector);
                } 

            }
            else
            {
                Debug.LogWarning($"No Rigidbody2D found on {collision.gameObject.name}. Knockback skipped.");
            }
        }

        private void HandleMovement()
        {
            if (collided)
            {
                if (downwardStrike)
                {
                    // Reset upward velocity to avoid stacking velocity
                    if (rb.linearVelocity.y > 0)
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                    }

                    // Apply upward force using Impulse mode
                    rb.AddForce(Vector2.up * playerMelee.upwardsForce, ForceMode2D.Impulse);
                }
                else
                {
                    // Apply force in the determined direction using Impulse mode
                    rb.AddForce(direction * playerMelee.defaultForce, ForceMode2D.Impulse);
                }
            }
        }

        private IEnumerator NoLongerColliding()
        {
            // Waits for a specified time before resetting collision flags
            yield return new WaitForSeconds(playerMelee.movementTime);
            collided = false;
            downwardStrike = false;
        }

        public void SwingFinish()
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
