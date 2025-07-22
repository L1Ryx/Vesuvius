using System.Collections;
using System.Collections.Generic;
using _ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace _Gameplay._Arch
{
    [RequireComponent(typeof(ControlsTextReplacement))]
    public class PlayerTutorial : MonoBehaviour
    {
        [Header("Tutorial Text References")]
        public TextMeshProUGUI moveText;
        public TextMeshProUGUI jumpText;
        public TextMeshProUGUI interactionText;
        public TextMeshProUGUI slashText;
        public TextMeshProUGUI rebalanceText;
        public TextMeshProUGUI doubleJumpText;
        public TextMeshProUGUI realityShiftText;

        [Header("Tutorial Configuration")]
        public TutorialData tutorialData;
        public float fadeRate = 1f; // Fade in/out speed
        public float displayTime = 5f; // Time text stays visible

        private Queue<IEnumerator> tutorialQueue = new Queue<IEnumerator>();
        private bool isPlaying = false;
        private InputEventListener inputEventListener;

        public void PlayMoveText() => EnqueueTutorial("Move", moveText);
        public void PlayJumpText() => EnqueueTutorial("Jump", jumpText);
        public void PlayInteractionText() => EnqueueTutorial("Interaction", interactionText);
        public void PlaySlashText() => EnqueueTutorial("Slash", slashText);
        public void PlayRebalanceText() => EnqueueTutorial("Rebalance", rebalanceText);
        public void PlayDoubleJumpText() => EnqueueTutorial("DoubleJump", doubleJumpText);
        public void PlayRealityShiftText() => EnqueueTutorial("RealityShift", realityShiftText);


        private int healTutorialShown = 0;
        private ControlsTextReplacement controlsTextReplacement;

        private void Awake()
        {
            controlsTextReplacement = GetComponent<ControlsTextReplacement>();
        }
        void Start()
        {
            controlsTextReplacement.Initialize(new TMP_Text[] { moveText,jumpText,interactionText,slashText,rebalanceText,doubleJumpText,realityShiftText });
        }

        public void OnControlsChanged()
        {
            controlsTextReplacement.OnControlsChanged();
        }

        private void EnqueueTutorial(string id, TextMeshProUGUI text)
        {
            if (!tutorialData.HasTutorialBeenShown(id))
            {
                if ("Rebalance".Equals(id) && healTutorialShown <= 2)
                {
                    healTutorialShown++;
                }
                else
                {
                    tutorialData.MarkTutorialAsShown(id);
                }
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
            //text.text = m_SlashText;
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
}
