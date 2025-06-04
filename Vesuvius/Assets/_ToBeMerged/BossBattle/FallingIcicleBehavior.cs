using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class FallingIcicleBehavior : MonoBehaviour
{
    public float fallSpeed = 3f;
    private Vector3 currentPosition;
    public float raycastDistance = 0.5f; // Distance to check for ground
    public LayerMask groundLayer;
    public UnityEvent icicleHitGround;
    public float fadeDuration = 1f;
    Vector3 initialPosition;
    private bool isFalling = false;
    private Collider2D hazardCollider;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        hazardCollider = GetComponentInChildren<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        initialPosition = this.gameObject.transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.down * raycastDistance);
    }


    void FixedUpdate()
    {
        if (isFalling)
        {
            Fall();
            // Cast a ray straight down.
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, groundLayer);

            if (hit)
            {
                StartCoroutine(FadeOut());
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FadeOut());
        }
    }

    public void ResetIcicle()
    {
        isFalling = false;
        this.gameObject.transform.position = initialPosition;
        Color finalColor = spriteRenderer.color;
        finalColor.a = 1f;
        spriteRenderer.color = finalColor;
        icicleHitGround.Invoke();
    }

    public void ShowIcicles()
    {
        StartCoroutine(IciclesEmerge());
    }

    IEnumerator IciclesEmerge()
    {
        //Push icicles out a little bit without colliders as a warning
        currentPosition = this.gameObject.transform.position;
        while (currentPosition.y >= initialPosition.y - .25)
        {
            Fall();
            yield return new WaitForEndOfFrame();
        }
    }

    void Fall()
    {
        currentPosition = this.gameObject.transform.position;
        currentPosition -= transform.up * Time.deltaTime * fallSpeed;
        this.gameObject.transform.position = currentPosition;
    }

    public void DropIcicles()
    {
        //activate colliders
        hazardCollider.enabled = true;
        isFalling = true;
    }

    private IEnumerator FadeOut()
    {
        // Perform the crossfade
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = elapsedTime / fadeDuration;

            Color cloneColor = spriteRenderer.color;
            cloneColor.a = 1 - alpha;
            spriteRenderer.color = cloneColor;


            yield return null;
        }

        //ensure final is correct
        Color finalColor = spriteRenderer.color;
        finalColor.a = 0f;
        spriteRenderer.color = finalColor;
        ResetIcicle();
    }
}
