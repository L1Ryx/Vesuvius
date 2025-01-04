using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerTutorial : MonoBehaviour
{
    [Header("Tutorial Text References")]
    public TextMeshProUGUI moveText;
    public TextMeshProUGUI jumpText;
    public TextMeshProUGUI interactionText;
    public TextMeshProUGUI slashText;
    public TextMeshProUGUI rebalanceText;

    [Header("Tutorial Configuration")]
    public TutorialData tutorialData;
    public float fadeRate = 1f; // Fade in/out speed
    public float displayTime = 5f; // Time text stays visible

    private Queue<IEnumerator> tutorialQueue = new Queue<IEnumerator>();
    private bool isPlaying = false;

    public void PlayMoveText() => EnqueueTutorial("Move", moveText);
    public void PlayJumpText() => EnqueueTutorial("Jump", jumpText);
    public void PlayInteractionText() => EnqueueTutorial("Interaction", interactionText);
    public void PlaySlashText() => EnqueueTutorial("Slash", slashText);
    public void PlayRebalanceText() => EnqueueTutorial("Rebalance", rebalanceText);

    private void EnqueueTutorial(string id, TextMeshProUGUI text)
    {
        if (!tutorialData.HasTutorialBeenShown(id))
        {
            tutorialData.MarkTutorialAsShown(id);
            tutorialQueue.Enqueue(PlayTextCoroutine(text));
            if (!isPlaying) StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isPlaying = true;
        while (tutorialQueue.Count > 0)
        {
            yield return StartCoroutine(tutorialQueue.Dequeue());
        }
        isPlaying = false;
    }

    private IEnumerator PlayTextCoroutine(TextMeshProUGUI text)
    {
        text.gameObject.SetActive(true);
        CanvasGroup canvasGroup = text.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = text.gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0;

        // Fade In
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * fadeRate;
            yield return null;
        }

        // Wait for display time
        yield return new WaitForSeconds(displayTime);

        // Fade Out
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeRate;
            yield return null;
        }

        text.gameObject.SetActive(false);
    }
}
