using System.Collections;
using UnityEngine;

public class Fader : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public bool startFadedOut = false;
    public float fadeDuration = .5f;

    private bool isFadedOut;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isFadedOut = startFadedOut;
        Color originalColor = spriteRenderer.color;
        originalColor.a = startFadedOut ? 0f : originalColor.a;
        spriteRenderer.color = originalColor;
    }

    private IEnumerator FadeIn()
    {
        // Perform the crossfade
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = elapsedTime / fadeDuration;

            Color cloneColor = spriteRenderer.color;
            cloneColor.a = alpha;
            spriteRenderer.color = cloneColor;


            yield return null;
        }

        //ensure final is correct
        Color finalColor = spriteRenderer.color;
        finalColor.a = 1f;
        spriteRenderer.color = finalColor;

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

        this.gameObject.SetActive(false);
    }

    public void CallFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    public void CallFadeOut()
    {
        StartCoroutine(FadeOut());
    }
    public void CallFadeOutAndDisable()
    {
        print("called");
        StartCoroutine(FadeOut());
    }
}
