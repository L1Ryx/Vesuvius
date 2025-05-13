using System.Collections;
using UnityEngine;

public class SpriteRenderer_RealityShift : MonoBehaviour,IRealityShiftable
{
    public GameObject NormalRealityLayer;
    public GameObject AlteredRealityLayer;
    private SpriteRenderer[] NormalRealityRenderers;

    private SpriteRenderer[] AlteredRealityRenderers;
    private Coroutine currentTransition;

    void Awake()
    {
        NormalRealityRenderers = NormalRealityLayer.GetComponentsInChildren<SpriteRenderer>();
        AlteredRealityRenderers = AlteredRealityLayer.GetComponentsInChildren<SpriteRenderer>();
    }

    private IEnumerator CrossfadeSprites(bool isNewRealityAlternate, float crossfadeDuration)
    {
        Debug.Log(Application.persistentDataPath);
        NormalRealityLayer.SetActive(true);
        AlteredRealityLayer.SetActive(true);
        // Perform the crossfade
        float elapsedTime = 0f;
        while (elapsedTime < crossfadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = elapsedTime / crossfadeDuration;

            // Fade out renderers and tilemap of old reality
            foreach (var FadeOutSprites in isNewRealityAlternate ? NormalRealityRenderers: AlteredRealityRenderers)
            {
                Color originalColor = FadeOutSprites.color;
                originalColor.a = 1f - alpha;
                FadeOutSprites.color = originalColor;
            }

            // Fade in renderers and tilemap of new reality
            foreach (var FadeInSprites in isNewRealityAlternate ? AlteredRealityRenderers : NormalRealityRenderers)
            {
                Color cloneColor = FadeInSprites.color;
                cloneColor.a = alpha;
                FadeInSprites.color = cloneColor;
            }

            yield return null;
        }

        // Ensure final alpha values are correct
        foreach (var FadeOutSprites in isNewRealityAlternate ? NormalRealityRenderers: AlteredRealityRenderers)
        {
            Color originalColor = FadeOutSprites.color;
            originalColor.a = 0f;
            FadeOutSprites.color = originalColor;
        }

        foreach (var FadeInSprites in isNewRealityAlternate ? AlteredRealityRenderers : NormalRealityRenderers)
        {
            Color cloneColor = FadeInSprites.color;
            cloneColor.a = 1f;
            FadeInSprites.color = cloneColor;
        }

        NormalRealityLayer.SetActive(!isNewRealityAlternate);
        AlteredRealityLayer.SetActive(isNewRealityAlternate);
    }

    public void RealityShiftCrossFade(bool isAltReality, float crossfadeDuration)
    {
        if (currentTransition != null)
        {
            StopCoroutine(currentTransition);
        }
        currentTransition = StartCoroutine(CrossfadeSprites(isAltReality, crossfadeDuration));
    }

    public void RealityShiftInstantly(bool isAltReality)
    {
        NormalRealityLayer.SetActive(!isAltReality);
        AlteredRealityLayer.SetActive(isAltReality);
    }
}
