using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMap_RealityShift : MonoBehaviour,IRealityShiftable
{
    public GameObject NormalRealityLayer;
    public GameObject AlteredRealityLayer;

    private TilemapRenderer[] NormalRealityTilemaps;

    private TilemapRenderer[] AlteredRealityTilemaps;
    private Coroutine currentTransition;


    void Awake()
    {
        NormalRealityTilemaps = NormalRealityLayer.GetComponentsInChildren<TilemapRenderer>();
        AlteredRealityTilemaps = AlteredRealityLayer.GetComponentsInChildren<TilemapRenderer>();
    }

    private IEnumerator CrossfadeTilemaps(bool isNewRealityAlternate, float crossfadeDuration)
    {
        NormalRealityLayer.SetActive(true);
        AlteredRealityLayer.SetActive(true);
        // Perform the crossfade
        float elapsedTime = 0f;
        while (elapsedTime < crossfadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = elapsedTime / crossfadeDuration;

            // Fade out  tilemap of old reality
            foreach (var FadeOutTilemaps in isNewRealityAlternate ? NormalRealityTilemaps: AlteredRealityTilemaps)
            {
                FadeOutTilemaps.material.color = new Color(1, 1, 1, 1 - alpha); // Fade out
            }

            // Fade in tilemap of new reality

            foreach (var FadeInTilemaps in isNewRealityAlternate ? AlteredRealityTilemaps : NormalRealityTilemaps)
            {
                FadeInTilemaps.material.color = new Color(1, 1, 1, alpha); // Fade in
            }

            yield return null;
        }

        // Ensure final alpha values are correct
        foreach (var FadeOutSprites in isNewRealityAlternate ? NormalRealityTilemaps: AlteredRealityTilemaps)
        {
            Color originalColor = FadeOutSprites.material.color;
            originalColor.a = 0f;
            FadeOutSprites.material.color = originalColor;
        }

        foreach (var FadeInSprites in isNewRealityAlternate ? AlteredRealityTilemaps : NormalRealityTilemaps)
        {
            Color cloneColor = FadeInSprites.material.color;
            cloneColor.a = 1f;
            FadeInSprites.material.color = cloneColor;
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
        currentTransition = StartCoroutine(CrossfadeTilemaps(isAltReality, crossfadeDuration));
    }

    public void RealityShiftInstantly(bool isAltReality)
    {
        NormalRealityLayer.SetActive(!isAltReality);
        AlteredRealityLayer.SetActive(isAltReality);
    }
}
