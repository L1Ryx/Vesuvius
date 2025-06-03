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
    Vector3 initialPosition;
    private bool isFalling = false;
    private Collider2D hazardCollider;

    void Awake()
    {
        hazardCollider = GetComponentInChildren<Collider2D>();
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
                icicleHitGround.Invoke();
                ResetIcicle();
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ResetIcicle();
        }
    }

    public void ResetIcicle()
    {
        isFalling = false;
        this.gameObject.transform.position = initialPosition;
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
}
