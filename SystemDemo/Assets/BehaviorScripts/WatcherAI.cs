using UnityEngine;
using System.Collections;

public class WatcherAI : MonoBehaviour
{
    public enum State { Passive, Aggro }
    public enum PassiveSubstate { Idle, Walking }
    public enum AggroSubstate { Idle, Walking, Poke, Charge }

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
    [SerializeField] private float chargeSpeed = 6f;
    [SerializeField] private float chargeDuration = 2f;
    [SerializeField] private float chargeChance = 0.25f; // 25% chance per second

    [Header("Detection Settings")]
    [SerializeField] private bool enableDetection = true;
    [SerializeField] private float wallDetectionOffset = 0.3f;
    [SerializeField] private float edgeDetectionDistance = 0.5f;
    [SerializeField] private LayerMask terrainLayer;

    [Header("Debug")]
    [SerializeField] private State currentState = State.Passive;
    [SerializeField] private PassiveSubstate currentPassiveSubstate;
    [SerializeField] private AggroSubstate currentAggroSubstate;

    private Rigidbody2D rb;
    private Transform spriteTransform;
    private Transform playerTransform; // Reference to the player
    private bool movingRight = true; // Direction flag
    private float deaggroTimer = 0f;
    private bool isPerformingAttack = false; // Flag to indicate if Poke or Charge is in progress

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteTransform = transform.GetChild(0); // Assuming the sprite is the first child
        StartCoroutine(PassiveBehavior());
    }

    private void Update()
    {
        HandleStateTransitions();

        if (currentState == State.Aggro)
        {
            HandleAggroState();
        }
    }

    private void HandleStateTransitions()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, aggroRadius, playerLayer);

        if (player != null)
        {
            if (currentState != State.Aggro)
            {
                currentState = State.Aggro;
                playerTransform = player.transform;
                Debug.Log("Watcher has entered Aggro state!");
                StopAllCoroutines();
            }

            deaggroTimer = 0f;
        }
        else if (currentState == State.Aggro)
        {
            deaggroTimer += Time.deltaTime;
            if (deaggroTimer >= deaggroTime)
            {
                currentState = State.Passive;
                Debug.Log("Watcher has exited Aggro state and returned to Passive state.");
                playerTransform = null;
                StartCoroutine(PassiveBehavior());
            }
        }
    }

    private IEnumerator PassiveBehavior()
    {
        currentState = State.Passive;

        while (currentState == State.Passive)
        {
            currentPassiveSubstate = PassiveSubstate.Idle;
            Debug.Log("Watcher entered Passive Idle state.");
            rb.linearVelocity = Vector2.zero;

            float idleDuration = Random.Range(minIdleTime, maxIdleTime);
            yield return new WaitForSeconds(idleDuration);

            currentPassiveSubstate = PassiveSubstate.Walking;
            Debug.Log("Watcher entered Passive Walking state.");
            bool walkRight = Random.value > 0.5f;
            movingRight = walkRight;
            FlipSprite();

            float walkDuration = Random.Range(minWalkTime, maxWalkTime);
            float elapsedTime = 0f;

            while (elapsedTime < walkDuration)
            {
                if (enableDetection && (CheckWall() || !CheckEdge()))
                {
                    Debug.Log("Watcher detected wall or edge, stopping movement.");
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
        if (isPerformingAttack)
        {
            // If an attack is in progress, do not transition substates
            return;
        }

        float playerX = playerTransform.position.x;
        float watcherX = transform.position.x;

        if (Mathf.Abs(playerX - watcherX) <= xLevelPadding)
        {
            SetAggroSubstate(AggroSubstate.Idle);
            TryInitiateAttack();
        }
        else
        {
            SetAggroSubstate(AggroSubstate.Walking);
            WalkTowardsPlayer();
            TryInitiateAttack();
        }
    }


    private void SetAggroSubstate(AggroSubstate substate)
    {
        if (currentAggroSubstate != substate)
        {
            currentAggroSubstate = substate;
            Debug.Log($"Watcher entered {substate} state.");
        }
    }

    private void WalkTowardsPlayer()
    {
        if (playerTransform == null) return;

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
        if (Random.value < pokeChance * Time.deltaTime)
        {
            StartCoroutine(PokeAttack());
        }
        else if (Random.value < chargeChance * Time.deltaTime)
        {
            StartCoroutine(ChargeAttack());
        }
    }

    private IEnumerator PokeAttack()
    {
        SetAggroSubstate(AggroSubstate.Poke);
        isPerformingAttack = true; // Mark attack as active

        rb.linearVelocity = Vector2.zero; // Stop movement during attack
        FlipTowardsPlayer();

        Debug.Log("Watcher is performing Poke attack.");

        yield return new WaitForSeconds(pokeDuration); // Placeholder duration for attack

        isPerformingAttack = false; // Mark attack as complete
        SetAggroSubstate(Mathf.Abs(playerTransform.position.x - transform.position.x) <= xLevelPadding
            ? AggroSubstate.Idle
            : AggroSubstate.Walking);
    }


    private IEnumerator ChargeAttack()
    {
        SetAggroSubstate(AggroSubstate.Charge);
        isPerformingAttack = true; // Mark attack as active

        FlipTowardsPlayer();
        Debug.Log("Watcher is performing Charge attack.");

        float elapsedTime = 0f;
        while (elapsedTime < chargeDuration)
        {
            if (enableDetection && (CheckWall() || !CheckEdge()))
            {
                Debug.Log("Charge interrupted by wall or edge.");
                break;
            }

            rb.linearVelocity = new Vector2((movingRight ? chargeSpeed : -chargeSpeed), rb.linearVelocity.y);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StopMovement(); // Stop movement after the charge ends
        isPerformingAttack = false; // Mark attack as complete
        SetAggroSubstate(Mathf.Abs(playerTransform.position.x - transform.position.x) <= xLevelPadding
            ? AggroSubstate.Idle
            : AggroSubstate.Walking);
    }


    private void StopMovement()
    {
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
}
