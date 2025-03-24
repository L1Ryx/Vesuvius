using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundSwitcher : MonoBehaviour
{
    [Header("Background Settings")]
    public BackgroundMapping backgroundMapping;  // Reference to the BackgroundMapping ScriptableObject
    public List<SpriteRenderer> backgroundLayers; // List of SpriteRenderers for the background layers
    public bool isForestMode = false;            // Track current mode (icy or forest)
    public float crossfadeDuration = 2f;         // Duration of the crossfade (default 2 seconds)

    [ContextMenu("Switch Backgrounds")]
    public void SwitchBackgrounds()
    {
        StartCoroutine(CrossfadeBackgrounds());
    }

    private IEnumerator CrossfadeBackgrounds()
    {
        List<GameObject> newClones = new List<GameObject>();

        // Clone each background layer and set the new sprites
        for (int i = 0; i < backgroundLayers.Count; i++)
        {
            SpriteRenderer originalRenderer = backgroundLayers[i];

            // Clone the current GameObject
            GameObject clone = Instantiate(originalRenderer.gameObject, originalRenderer.transform.parent);
            SpriteRenderer cloneRenderer = clone.GetComponent<SpriteRenderer>();

            // Set the new sprite on the clone
            Sprite currentSprite = originalRenderer.sprite;
            Sprite newSprite = isForestMode
                ? backgroundMapping.GetIcySprite(currentSprite)    // Switch to icy
                : backgroundMapping.GetForestSprite(currentSprite); // Switch to forest

            if (newSprite != null)
            {
                cloneRenderer.sprite = newSprite;
            }

            // Set initial transparency to 0 for the clone (fully transparent)
            Color cloneColor = cloneRenderer.color;
            cloneColor.a = 0f;
            cloneRenderer.color = cloneColor;

            // Rename the clone for clarity
            clone.name = $"{originalRenderer.gameObject.name}_Clone";

            newClones.Add(clone);
        }

        // Perform the crossfade
        float elapsedTime = 0f;
        while (elapsedTime < crossfadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = elapsedTime / crossfadeDuration;

            // Fade out the original layers and fade in the clones
            foreach (var originalRenderer in backgroundLayers)
            {
                Color originalColor = originalRenderer.color;
                originalColor.a = 1f - alpha;
                originalRenderer.color = originalColor;
            }

            foreach (var clone in newClones)
            {
                SpriteRenderer cloneRenderer = clone.GetComponent<SpriteRenderer>();
                Color cloneColor = cloneRenderer.color;
                cloneColor.a = alpha;
                cloneRenderer.color = cloneColor;
            }

            yield return null;
        }

        // Ensure final alpha values are correct
        foreach (var originalRenderer in backgroundLayers)
        {
            Color originalColor = originalRenderer.color;
            originalColor.a = 0f;
            originalRenderer.color = originalColor;
            Destroy(originalRenderer.gameObject); // Destroy original backgrounds
        }

        foreach (var clone in newClones)
        {
            SpriteRenderer cloneRenderer = clone.GetComponent<SpriteRenderer>();
            Color cloneColor = cloneRenderer.color;
            cloneColor.a = 1f;
            cloneRenderer.color = cloneColor;
        }

        // Update the backgroundLayers list to reference the clones
        backgroundLayers.Clear();
        foreach (var clone in newClones)
        {
            backgroundLayers.Add(clone.GetComponent<SpriteRenderer>());
        }

        // Toggle the mode
        isForestMode = !isForestMode;
    }

    private void Reset()
    {
        // Automatically populate the backgroundLayers list with all child SpriteRenderers
        backgroundLayers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SwitchBackgrounds();
        }
    }
}
