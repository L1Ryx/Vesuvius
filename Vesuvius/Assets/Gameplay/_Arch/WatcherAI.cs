using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay._Arch
{
    public class WatcherAI : MonoBehaviour
    {
        public enum State { Passive, Aggro }
        public enum PassiveSubstate { Idle, Walking }
        public enum AggroSubstate { Idle, Walking, Poke }

        [Header("Passive State Settings")]
        [SerializeField] private float minIdleTime = 2f;
        [SerializeField] private float maxIdleTime = 5f;
        [SerializeField] private float minWalkTime = 1f;
        [SerializeField] private float maxWalkTime = 3f;
        [SerializeField] private float passiveWalkSpeed = 2f;

        [Header("Aggro State Settings")]
        [SerializeField] private float aggroRadius = 5f;
        [SerializeField] private float deaggroTime = 3f;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private float xLevelPadding = 0.5f;
        [SerializeField] private float aggroWalkSpeed = 4f;
        [SerializeField] private float pokeRange = 2f;
        [SerializeField] private float pokeDuration = 3f;
        [SerializeField] private float pokeChance = 0.4f; // 40% chance per second
        // [SerializeField] private float chargeSpeed = 6f;
        // [SerializeField] private float chargeDuration = 2f;
        // [SerializeField] private float chargeChance = 0.25f; // 25% chance per second

        [Header("Detection Settings")]
        [SerializeField] private bool enableDetection = true;
        [SerializeField] private float wallDetectionOffset = 0.3f;
        [SerializeField] private float edgeDetectionDistance = 0.5f;
        [SerializeField] private LayerMask terrainLayer;
        [SerializeField] private float aggroDelay = 1f; // Delay before first move in Aggro


        [Header("Debug")]
        [SerializeField] private State currentState = State.Passive;
        [SerializeField] private PassiveSubstate currentPassiveSubstate;
        [SerializeField] private AggroSubstate currentAggroSubstate;
        [Header("Animation Settings")]
        [SerializeField] private Animator animator; // Reference to the Animator
        [Header("References")]
        public EnemyHealth enemyHealth;
        public UnityEvent watcherStepped;
        private WatcherAudio watcherAudio;

        [Header("Knockback")]
        private bool isKnockedBack = false; // Flag for knockback state
        private float knockbackTimer = 0f; // Timer to track knockback recovery
        public float knockbackRecoveryTime = 0.5f; // Time to recover from knockback
        [Header("Footstep Settings")]
        [SerializeField] private float footstepInterval = 0.5f; // Interval between footsteps in seconds
        private Coroutine footstepCoroutine;


        private const string idleTrigger = "Watcher-Idle";
        private const string walkTrigger = "Watcher-Walk";
        private const string pokeTrigger = "Watcher-Poke";

        private Rigidbody2D rb;
        private Transform spriteTransform;
        private Transform playerTransform; // Reference to the player
        private bool movingRight = true; // Direction flag
        private float deaggroTimer = 0f;
        private bool isPerformingAttack = false; // Flag to indicate if Poke or Charge is in progress
        private bool isDead = false;
        private bool isAggroDelayActive = false;
        private bool hasPlayedDeathSFX = false;


        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            watcherAudio = GetComponent<WatcherAudio>();
            spriteTransform = transform.GetChild(0); // Assuming the sprite is the first child

            // Correctly initialize EnemyHealth reference from child
            enemyHealth = GetComponentInChildren<EnemyHealth>();

            StartCoroutine(PassiveBehavior());
            isDead = false;

        }


        private void Update()
        {
            // Check if the enemy is dead
            if (enemyHealth != null && enemyHealth.GetIsDead())
            {   
                isDead = true;
                HandleDeathBehavior();
                return; // Skip all other logic
            }

            // Handle knockback state
            if (isKnockedBack)
            {
                knockbackTimer -= Time.deltaTime;
                if (knockbackTimer <= 0f)
                {
                    isKnockedBack = false; // Recover from knockback
                }
                return; // Skip normal behavior while knocked back
            }

            // Proceed with normal AI behavior
            if (!isDead)
            {
                HandleStateTransitions();

                if (currentState == State.Aggro)
                {
                    HandleAggroState();
                }
                HandleFootsteps(); 
            }
        }



        private void OnDestroy()
        {
            StopAllCoroutines(); // Stop all ongoing coroutines to avoid referencing destroyed objects
        }


        private void HandleDeathBehavior()
        {
            // Stop all movement and AI logic
            StopAllCoroutines(); // Stops any active PassiveBehavior or AggroState coroutines

            // Disable velocity modifications
            //rb.linearVelocity = Vector2.zero; // Stop movement completely

            // Set to Passive Idle substate (for consistency)
            currentState = State.Passive;
            currentPassiveSubstate = PassiveSubstate.Idle;

            // Play idle animation to indicate death (if desired)
            SetAnimation(idleTrigger);
            if (!hasPlayedDeathSFX) {
                watcherAudio.PlayWatcherDeath();
                hasPlayedDeathSFX = true;
            }
        
        }



        private void ResetAllTriggers()
        {
            animator.ResetTrigger(idleTrigger);
            animator.ResetTrigger(walkTrigger);
            animator.ResetTrigger(pokeTrigger);
        }

        private void SetAnimation(string triggerName)
        {
            if (animator != null)
            {
                ResetAllTriggers(); // Clear all other triggers
                animator.SetTrigger(triggerName); // Set the correct trigger
            }
        }



        private void HandleStateTransitions()
        {
            if (enemyHealth == null || enemyHealth.GetIsDead())
            {
                return;
            }

            Collider2D player = Physics2D.OverlapCircle(transform.position, aggroRadius, playerLayer);

            if (player != null)
            {
                if (currentState != State.Aggro)
                {
                    watcherAudio.PlayWatcherAlert();
                    currentState = State.Aggro;
                    playerTransform = player.transform;
                    StopAllCoroutines();
                    StartCoroutine(AggroDelay());
                }
                deaggroTimer = 0f;
            }

            else if (currentState == State.Aggro)
            {
                deaggroTimer += Time.deltaTime;
                if (deaggroTimer >= deaggroTime)
                {
                    currentState = State.Passive;
                    playerTransform = null;

                    StopAllCoroutines(); // Ensure no conflicting coroutines
                    StartCoroutine(PassiveBehavior());
                }
            }
        }

        private IEnumerator AggroDelay()
        {
            isAggroDelayActive = true; // Activate the delay
            yield return new WaitForSeconds(aggroDelay); // Wait for the specified duration
            isAggroDelayActive = false; // End the delay
        }




        private IEnumerator PassiveBehavior()
        {
            currentState = State.Passive;

            while (currentState == State.Passive)
            {
                if (isKnockedBack) yield break; // Stop behavior if knocked back

                currentPassiveSubstate = PassiveSubstate.Idle;
                rb.linearVelocity = Vector2.zero;
                SetAnimation(idleTrigger);

                float idleDuration = Random.Range(minIdleTime, maxIdleTime);
                yield return new WaitForSeconds(idleDuration);

                currentPassiveSubstate = PassiveSubstate.Walking;
                SetAnimation(walkTrigger);

                bool walkRight = Random.value > 0.5f;
                movingRight = walkRight;
                FlipSprite();

                float walkDuration = Random.Range(minWalkTime, maxWalkTime);
                float elapsedTime = 0f;

                while (elapsedTime < walkDuration)
                {
                    if (enableDetection && (CheckWall() || !CheckEdge()) || isKnockedBack)
                    {
                        break;
                    }

                    rb.linearVelocity = new Vector2((walkRight ? passiveWalkSpeed : -passiveWalkSpeed), rb.linearVelocity.y);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                rb.linearVelocity = Vector2.zero;
            }
        }


        private void HandleAggroState()
        {
            if (isPerformingAttack || isAggroDelayActive)
            {
                // If an attack is in progress, do not transition substates
                return;
            }

            float playerX = playerTransform.position.x;
            float watcherX = transform.position.x;

            if (Mathf.Abs(playerX - watcherX) <= xLevelPadding)
            {
                SetAggroSubstate(AggroSubstate.Idle);
                SetAnimation(idleTrigger);
                TryInitiateAttack();
            }
            else
            {
                SetAggroSubstate(AggroSubstate.Walking);
                SetAnimation(walkTrigger);
                WalkTowardsPlayer();
                TryInitiateAttack();
            }
        }


        private void SetAggroSubstate(AggroSubstate substate)
        {
            if (currentAggroSubstate != substate)
            {
                currentAggroSubstate = substate;

                // Trigger animations based on state
                switch (substate)
                {
                    case AggroSubstate.Idle:
                        SetAnimation(idleTrigger);
                        break;
                    case AggroSubstate.Walking:
                        SetAnimation(walkTrigger);
                        break;
                }
            }
        }


        private void WalkTowardsPlayer()
        {
            if (playerTransform == null || isKnockedBack) return;

            bool playerToRight = playerTransform.position.x > transform.position.x;

            movingRight = playerToRight;
            FlipSprite();

            if (enableDetection && (CheckWall() || !CheckEdge()))
            {
                StopMovement();
                return;
            }

            rb.linearVelocity = new Vector2((movingRight ? aggroWalkSpeed : -aggroWalkSpeed), rb.linearVelocity.y);
        }


        private void TryInitiateAttack()
        {
            if (isPerformingAttack) return; // Prevent overlap with current attack

            if (Random.value < pokeChance * Time.deltaTime)
            {
                StartCoroutine(PokeAttack());
            }
        }


        private IEnumerator PokeAttack()
        {
            SetAggroSubstate(AggroSubstate.Poke);
            isPerformingAttack = true;

            rb.linearVelocity = Vector2.zero; // Stop movement during Poke
            FlipTowardsPlayer();

            SetAnimation(pokeTrigger); // Trigger Poke animation
            watcherAudio.PlayWatcherAttack();

            // Wait for EndPoke to be called by animation event
            yield return null;
        }


        public void EndPoke()
        {
            isPerformingAttack = false; // Mark attack as complete
            // Transition back to Idle or Walking depending on player's position
            SetAggroSubstate(Mathf.Abs(playerTransform.position.x - transform.position.x) <= xLevelPadding
                ? AggroSubstate.Idle
                : AggroSubstate.Walking);
        }



        private void StopMovement()
        {
            if (enemyHealth != null && enemyHealth.GetIsDead())
            {
                return; // Don't stop movement if the enemy is dead
            }

            rb.linearVelocity = Vector2.zero;
        }



        private bool CheckWall()
        {
            if (!enableDetection) return false;

            Vector2 origin = (Vector2)transform.position + Vector2.down * 0.1f;
            Vector2 direction = movingRight ? Vector2.right : Vector2.left;
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, wallDetectionOffset, terrainLayer);

            return hit.collider != null;
        }

        private bool CheckEdge()
        {
            if (!enableDetection) return true;

            Vector2 origin = (Vector2)transform.position + Vector2.down * 0.1f + (movingRight ? Vector2.right : Vector2.left) * (edgeDetectionDistance / 2);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, edgeDetectionDistance, terrainLayer);

            return hit.collider != null;
        }

        private void FlipSprite()
        {
            Vector3 localScale = spriteTransform.localScale;
            localScale.x = movingRight ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
            spriteTransform.localScale = localScale;


        }

        private void FlipTowardsPlayer()
        {
            if (playerTransform == null) return;

            movingRight = playerTransform.position.x > transform.position.x;
            FlipSprite();
        }

        private void OnDrawGizmosSelected()
        {
            Vector2 edgeOrigin = (Vector2)transform.position + Vector2.down * 0.1f + (movingRight ? Vector2.right : Vector2.left) * (edgeDetectionDistance / 2);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(edgeOrigin, edgeOrigin + Vector2.down * edgeDetectionDistance);

            Vector2 wallOrigin = (Vector2)transform.position + Vector2.down * 0.1f;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(wallOrigin, wallOrigin + (movingRight ? Vector2.right : Vector2.left) * wallDetectionOffset);

            // Draw aggro radius
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, aggroRadius);
        }

        public void ApplyKnockback(Vector2 knockbackForce)
        {
            if (enemyHealth != null && enemyHealth.GetIsDead()) return; // Skip if dead

            if (rb != null)
            {
                rb.AddForce(knockbackForce, ForceMode2D.Impulse); // Apply knockback force
                isKnockedBack = true; // Enter knockback state
                knockbackTimer = knockbackRecoveryTime; // Set recovery timer
            }
        }

        private void HandleFootsteps()
        {
            bool isWalking = (currentState == State.Passive && currentPassiveSubstate == PassiveSubstate.Walking) ||
                             (currentState == State.Aggro && currentAggroSubstate == AggroSubstate.Walking);

            if (isWalking && footstepCoroutine == null)
            {
                footstepCoroutine = StartCoroutine(FootstepLoop());
            }
            else if (!isWalking && footstepCoroutine != null)
            {
                StopCoroutine(footstepCoroutine);
                footstepCoroutine = null;
            }
        }

        private IEnumerator FootstepLoop()
        {
            while (true)
            {
                watcherStepped.Invoke(); // Trigger the event for a footstep
                yield return new WaitForSeconds(footstepInterval); // Wait for the interval before playing the next step
            }
        }






    }
}
