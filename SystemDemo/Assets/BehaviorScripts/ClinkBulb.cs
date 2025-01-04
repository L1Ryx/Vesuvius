using System.Collections;
using UnityEngine;

public class ClinkBulb : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string bulbID; // Unique ID for the bulb
    [SerializeField] private GameObject currencyParticleSystemPrefab; // Prefab of the Currency Particle System
    [SerializeField] private float fadeOutRate = 2f; // Speed at which the bulb fades out and disappears

    [Header("References")]
    [SerializeField] private GameObject clinkBulbHead;
    [SerializeField] private GameObject clinkBulbBase;
    
    [Header("Data References")]
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private BulbData bulbData; // Reference to the BulbData ScriptableObject

    private bool isCollected = false; // Tracks if the bulb has already been collected
    private SpriteRenderer headRenderer; // Renderer for the bulb head
    private Collider2D headCollider; // Collider for the bulb head

    private void Start()
    {
        // Get the head's SpriteRenderer and Collider2D
        headRenderer = clinkBulbHead.GetComponent<SpriteRenderer>();
        headCollider = GetComponent<Collider2D>(); // COLLIDER IS ON THE HEAD CHILD

        if (headRenderer == null || headCollider == null)
        {
            Debug.LogError("Clink Bulb: Missing required components on the head.");
        }

        // Check if the bulb is alive based on BulbData
        if (bulbData != null && !bulbData.GetBulbAliveState(bulbID))
        {
            Destroy(clinkBulbHead); // Destroy if the bulb HEAD is not alive
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected || !collision.CompareTag("Player")) return;

        isCollected = true; // Mark the bulb as collected
        StartCoroutine(FadeOutAndCollect());
    }

private IEnumerator FadeOutAndCollect()
{
    // Disable collision immediately
    headCollider.enabled = false;

    float alpha = 1f; // Start with full opacity

    // Gradually fade out the bulb head
    while (alpha > 0f)
    {
        alpha -= Time.deltaTime * fadeOutRate; // Decrease alpha over time
        alpha = Mathf.Clamp01(alpha); // Clamp between 0 and 1

        if (headRenderer != null)
        {
            Color color = headRenderer.color;
            color.a = alpha; // Set the alpha value
            headRenderer.color = color;
        }

        yield return null; // Wait for the next frame
    }

    // Once fully faded, add currency and spawn particles
    if (playerInfo != null && bulbData != null)
    {
        int amount = bulbData.GetClinkAmount(bulbID); // Retrieve clink amount from BulbData
        playerInfo.AddCurrency(amount); // Add clink amount to total currency
    }
    else
    {
        Debug.LogError("PlayerInfo or BulbData not found in the scene.");
    }

    if (currencyParticleSystemPrefab != null)
    {
        GameObject particleSystem = Instantiate(currencyParticleSystemPrefab, transform.position, Quaternion.identity);
        CurrencyParticleSystem currencySystem = particleSystem.GetComponent<CurrencyParticleSystem>();
        
        int amount = bulbData.GetClinkAmount(bulbID); // Retrieve clink amount again for particles
        currencySystem.EmitCurrencyParticles(amount); // Emit particles based on clink amount
    }

    // Update BulbData to mark this bulb as collected
    if (bulbData != null)
    {
        bulbData.SetBulbAliveState(bulbID, false); // Mark bulb as not alive
    }

    Destroy(clinkBulbHead); // ONLY DESTROY THE HEAD CHILD
}

}
