using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class RealityChange : MonoBehaviour
{
    public GameObject caveLayer;
    public GameObject jungleLayer;
    public SpriteRenderer[] caveRenderers;

    public TilemapRenderer[] caveTilemaps;
    public SpriteRenderer[] jungleRenderers;

    public TilemapRenderer[] jungleTilemaps;
    public bool isAltReality;
    public GameState gameState;
    public float crossfadeDuration = 2f; 

    void Awake()
    {
        caveRenderers = caveLayer.GetComponentsInChildren<SpriteRenderer>();
        jungleRenderers = jungleLayer.GetComponentsInChildren<SpriteRenderer>();
        caveTilemaps = caveLayer.GetComponentsInChildren<TilemapRenderer>();
        jungleTilemaps = jungleLayer.GetComponentsInChildren<TilemapRenderer>();
        caveLayer.SetActive(!gameState.isAltReality);
        jungleLayer.SetActive(gameState.isAltReality);
    }

    private IEnumerator CrossfadeBackgrounds()
    {
        //cache current reality and change it on SO in case a scene is loaded and interrupts transition before it can be set later
        isAltReality = gameState.isAltReality;
        gameState.isAltReality = !gameState.isAltReality;
        caveLayer.SetActive(true);
        jungleLayer.SetActive(true);
        // Perform the crossfade
        float elapsedTime = 0f;
        while (elapsedTime < crossfadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = elapsedTime / crossfadeDuration;

            // Fade out renderers and tilemap of old reality
            foreach (var originalRenderer in isAltReality ? jungleRenderers : caveRenderers)
            {
                Color originalColor = originalRenderer.color;
                originalColor.a = 1f - alpha;
                originalRenderer.color = originalColor;
            }

            foreach (var renderer in isAltReality ? jungleTilemaps : caveTilemaps)
            {
                renderer.material.color = new Color(1, 1, 1, 1 - alpha); // Fade out
            }

            // Fade in renderers and tilemap of new reality
            foreach (var clone in !isAltReality ? jungleRenderers : caveRenderers)
            {
                SpriteRenderer cloneRenderer = clone.GetComponent<SpriteRenderer>();
                Color cloneColor = cloneRenderer.color;
                cloneColor.a = alpha;
                cloneRenderer.color = cloneColor;
            }

            foreach (var renderer in !isAltReality ? jungleTilemaps : caveTilemaps)
            {
                renderer.material.color = new Color(1, 1, 1, alpha); // Fade in
            }

            yield return null;
        }

        // Ensure final alpha values are correct
        foreach (var originalRenderer in isAltReality ? jungleRenderers : caveRenderers)
        {
            Color originalColor = originalRenderer.color;
            originalColor.a = 0f;
            originalRenderer.color = originalColor;
        }

        foreach (var clone in !isAltReality ? jungleRenderers : caveRenderers)
        {
            SpriteRenderer cloneRenderer = clone.GetComponent<SpriteRenderer>();
            Color cloneColor = cloneRenderer.color;
            cloneColor.a = 1f;
            cloneRenderer.color = cloneColor;
        }

        // Toggle the mode
        isAltReality = gameState.isAltReality;
        caveLayer.SetActive(!isAltReality);
        jungleLayer.SetActive(isAltReality);
    }

    public void OnRealityShift()
    {
        StartCoroutine(CrossfadeBackgrounds());
        //isAltReality = !isAltReality;
    }
}
