using UnityEngine;

public class TestEnemyMovement : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 2f; // Movement speed of the enemy
    public float edgeCheckDistance = 0.5f; // Distance to check for edges
    public LayerMask terrainLayer; // Layer mask for terrain
    public float knockbackRecoveryTime = 0.5f; // Time to recover from knockback
    public float wallCheckOffset = 0.5f;

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
    }

    bool CheckWall()
    {
        // Cast a ray in the direction the enemy is moving to check for a wall
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, wallCheckOffset, terrainLayer);

        return hit.collider != null; // True if a wall is detected
    }

    bool CheckEdge()
    {
        // Cast a ray downward slightly ahead of the enemy to check for ground
        Vector2 origin = (Vector2)transform.position + (movingRight ? Vector2.right : Vector2.left) * edgeCheckDistance;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 1f, terrainLayer);

        return hit.collider != null; // True if ground is detected
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the edge check
        Vector2 edgeOrigin = (Vector2)transform.position + (movingRight ? Vector2.right : Vector2.left) * edgeCheckDistance;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(edgeOrigin, edgeOrigin + Vector2.down * 1f);

        // Visualize the wall check
        Vector2 wallOrigin = transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(wallOrigin, wallOrigin + (movingRight ? Vector2.right : Vector2.left) * 0.1f);
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
