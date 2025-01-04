using UnityEngine;
using System.Collections;

public class ClinkBulb : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string bulbID; // Unique ID for the bulb
    [SerializeField] private GameObject currencyParticleSystemPrefab; // Prefab of the Currency Particle System
    [SerializeField] private float fadeOutRate = 2f; // Speed at which the bulb fades out and disappears

    [Header("Light Tweening")]
    [SerializeField] private float flickerIntensityMin = 0.5f; // Minimum light intensity
    [SerializeField] private float flickerIntensityMax = 1.5f; // Maximum light intensity
    [SerializeField] private float flickerSpeed = 0.1f; // Speed of flickering
    [Header("Sprite Tweening")]
    [SerializeField] private float movementAmplitude = 0.1f;
    [SerializeField] private float movementSpeed = 1f; // Speed of up-and-down movement

    [SerializeField] private float pulsationAmplitude = 0.05f;
    [SerializeField] private float pulsationSpeed = 1f; // Speed of size pulsation


    [Header("References")]
    [SerializeField] private GameObject clinkBulbHead;
    [SerializeField] private GameObject clinkBulbBase;
    [SerializeField] private UnityEngine.Rendering.Universal.Light2D bulbLight; // Reference to the 2D spotlight

    [Header("Data References")]
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private BulbData bulbData; // Reference to the BulbData ScriptableObject

    private bool isCollected = false; // Tracks if the bulb has already been collected
    private SpriteRenderer headRenderer; // Renderer for the bulb head
    private Collider2D headCollider; // Collider for the bulb head

    private void Start()
    {
        // THE HEAD's COLLIDER IS ON THE PARENT
        headRenderer = clinkBulbHead.GetComponent<SpriteRenderer>();
        headCollider = GetComponent<Collider2D>();

        if (headRenderer == null || headCollider == null)
        {
            Debug.LogError("Clink Bulb: Missing required components on the head.");
        }

        // Check if the bulb is alive based on BulbData
        if (bulbData != null && !bulbData.GetBulbAliveState(bulbID))
        {
            Destroy(clinkBulbHead); // Destroy if the bulb HEAD is not alive
        }

        // Start the flicker coroutine
        if (bulbLight != null)
        {
            StartCoroutine(FlickerLight());
        }

        if (clinkBulbHead != null)
        {
            StartCoroutine(TweenHead());
        }

    }


    private IEnumerator FlickerLight()
    {
        float currentIntensity = bulbLight.intensity; // Start with the current intensity
        float targetIntensity = Random.Range(flickerIntensityMin, flickerIntensityMax); // Set an initial target intensity

        while (!isCollected)
        {
            if (bulbLight != null)
            {
                // Smoothly interpolate the intensity
                currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, Time.deltaTime * flickerSpeed);
                bulbLight.intensity = currentIntensity;

                // If close to the target intensity, choose a new target
                if (Mathf.Abs(currentIntensity - targetIntensity) < 0.05f)
                {
                    targetIntensity = Random.Range(flickerIntensityMin, flickerIntensityMax);
                }
            }

            yield return null; // Wait for the next frame
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected || !collision.CompareTag("Player")) return;

        isCollected = true; // Mark the bulb as collected
        StartCoroutine(FadeOutAndCollect());
    }

    private IEnumerator TweenHead()
    {
        if (clinkBulbHead == null) yield break; // Ensure head exists before starting

        Vector3 initialPosition = clinkBulbHead.transform.localPosition; // Save the initial position
        Vector3 initialScale = clinkBulbHead.transform.localScale; // Save the initial scale
        float timer = 0f;

        while (!isCollected && clinkBulbHead != null) // Stop tweening if the head is collected or destroyed
        {
            timer += Time.deltaTime;

            // Tween position (up and down movement)
            float offsetY = Mathf.Sin(timer * movementSpeed) * movementAmplitude; 
            clinkBulbHead.transform.localPosition = new Vector3(
                initialPosition.x,
                initialPosition.y + offsetY,
                initialPosition.z
            );

            // Tween scale (pulsating size)
            float scaleFactor = 1f + Mathf.Sin(timer * pulsationSpeed) * pulsationAmplitude; 
            clinkBulbHead.transform.localScale = initialScale * scaleFactor;

            yield return null; // Wait for the next frame
        }
    }



    private IEnumerator FadeOutAndCollect()
    {
        // Disable collision immediately
        headCollider.enabled = false;

        float alpha = 1f; // Start with full opacity
        float currentLightIntensity = bulbLight.intensity; // Start with the current light intensity

        // Gradually fade out the bulb head and light intensity
        while (alpha > 0f || currentLightIntensity > 0f)
        {
            // Decrease alpha over time
            alpha -= Time.deltaTime * fadeOutRate;
            alpha = Mathf.Clamp01(alpha); // Clamp between 0 and 1

            // Decrease light intensity over time
            if (bulbLight != null)
            {
                currentLightIntensity -= Time.deltaTime * fadeOutRate;
                currentLightIntensity = Mathf.Clamp01(currentLightIntensity); // Clamp to ensure it doesn't go negative
                bulbLight.intensity = currentLightIntensity;
            }

            // Update the bulb head's alpha
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
            // currencySystem.EmitCurrencyParticles(amount); // Emit particles based on clink amount
        }

        // Update BulbData to mark this bulb as collected
        if (bulbData != null)
        {
            bulbData.SetBulbAliveState(bulbID, false); // Mark bulb as not alive
        }

        Destroy(clinkBulbHead); // ONLY DESTROY THE HEAD CHILD
    }

}
