using Unity.VisualScripting;
using UnityEngine;

public class TestEnemyMovement : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 2f; // Movement speed of the enemy
    public float edgeCheckDistance = 0.5f; // Distance to check for edges
    public LayerMask terrainLayer; // Layer mask for terrain
    public float knockbackRecoveryTime = 0.5f; // Time to recover from knockback
    public float wallCheckOffset = 0.3f; // Reduced offset for smaller sprite

    [Header("References")]
    public EnemyHealth eh;

    private Rigidbody2D rb;
    private Transform spriteTransform; // Reference to the sprite for flipping
    private bool movingRight = true; // Direction flag
    private bool isKnockedBack = false; // Flag for knockback state
    private float knockbackTimer = 0f; // Timer to track knockback recovery

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteTransform = transform.GetChild(0); // Assuming the sprite is the first child
    }

    void Update()
    {  
        if (!eh.GetIsDead()) {
            HandleMovementLogic();
        }

    }

    private void HandleMovementLogic() {
        if (isKnockedBack)
        {
            // Handle knockback recovery
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }
            return; // Skip normal movement while knocked back
        }

        // Perform wall and edge checks
        bool wallAhead = CheckWall();
        bool edgeAhead = !CheckEdge();

        if (wallAhead || edgeAhead)
        {
            // Reverse direction
            movingRight = !movingRight;

            // Flip the sprite
            Vector3 localScale = spriteTransform.localScale;
            localScale.x *= -1; // Flip the x-axis
            spriteTransform.localScale = localScale;
        }

        // Apply movement
        rb.linearVelocity = new Vector2((movingRight ? moveSpeed : -moveSpeed), rb.linearVelocity.y);
        // Debug.Log($"Wall Check: {CheckWall()}, Edge Check: {CheckEdge()}");
    }

    bool CheckWall()
    {
        // Adjusted ray origin to be slightly lower for small sprite
        Vector2 origin = (Vector2)transform.position + Vector2.down * 0.1f;
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, wallCheckOffset, terrainLayer);

        return hit.collider != null; // True if a wall is detected
    }

    bool CheckEdge()
    {
        // Adjusted ray origin for edge check
        Vector2 origin = (Vector2)transform.position + Vector2.down * 0.1f + (movingRight ? Vector2.right : Vector2.left) * (edgeCheckDistance / 2);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 1f, terrainLayer);

        return hit.collider != null; // True if ground is detected
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the edge check
        Vector2 edgeOrigin = (Vector2)transform.position + Vector2.down * 0.1f + (movingRight ? Vector2.right : Vector2.left) * (edgeCheckDistance / 2);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(edgeOrigin, edgeOrigin + Vector2.down * 1f);

        // Visualize the wall check
        Vector2 wallOrigin = (Vector2)transform.position + Vector2.down * 0.1f;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(wallOrigin, wallOrigin + (movingRight ? Vector2.right : Vector2.left) * wallCheckOffset);
    }

    public void ApplyKnockback(Vector2 knockbackForce)
    {
        if (rb != null)
        {
            // Apply the knockback force
            rb.AddForce(knockbackForce, ForceMode2D.Impulse);

            // Set knockback state
            isKnockedBack = true;
            knockbackTimer = knockbackRecoveryTime;
        }
    }
}
