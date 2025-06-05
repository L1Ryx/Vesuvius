using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GauntletBlockerGrid : MonoBehaviour
{
    public CutsceneEyeManager cutscene;
    private TilemapRenderer tilemapRenderer;
    void Start()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
        //if player has gotten the mirror then disable
        if (cutscene.isBlocked())
        {
            this.gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        //if player has gotten the mirror then disable
        if (cutscene.isBlocked())
        {
            this.gameObject.SetActive(false);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        //is beam attack
        if (collision.gameObject.GetComponent<CollisionDamageIfSameReality>())
        {
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        // Perform the crossfade
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            float alpha = elapsedTime / 1f;

            tilemapRenderer.material.color = new Color(1, 1, 1, 1 - alpha); // Fade out
            yield return null;
        }

        Color originalColor = tilemapRenderer.material.color;
        originalColor.a = 0f;
        tilemapRenderer.material.color = originalColor;

        this.gameObject.SetActive(false);
    }
}
