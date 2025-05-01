using System.Collections;
using System.Collections.Generic;
using _ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace _Gameplay._Arch
{
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
        private InputEventListener inputEventListener;

        public void PlayMoveText() => EnqueueTutorial("Move", moveText);
        public void PlayJumpText() => EnqueueTutorial("Jump", jumpText);
        public void PlayInteractionText() => EnqueueTutorial("Interaction", interactionText);
        public void PlaySlashText() => EnqueueTutorial("Slash", slashText);
        public void PlayRebalanceText() => EnqueueTutorial("Rebalance", rebalanceText);

        //new attempt
        public PlayerInput m_PlayerInput;
        public ControlPrompts controlPrompts;

        public void OnControlsChanged()
        {
            UpdateUIHints();
        }

        //TODO: We can eventually consider trimming down these tutorial text components to just one that we replace given the situation.
        //maybe a switch case depending on which tutorial is active
        private void UpdateUIHints()
        {
            moveText.text = controlPrompts.movePrompt;
           
            jumpText.text = controlPrompts.jumpPrompt;
            
            slashText.text = controlPrompts.swingPrompt;

            rebalanceText.text = controlPrompts.healPrompt;
        }

        string GetBindingDisplayStringOrCompositeName(InputAction action)
        {
            // if composite action / can't use binding index or need to specify
            int bindingIndex = action.GetBindingIndex(group: m_PlayerInput.currentControlScheme);

            if (action.bindings[bindingIndex].isPartOfComposite)
            {
                // hard coded logic - assumes that if you found a part of a composite, that it's the first one.
                // And that the one preceeding it, must be the 'Composite head' that contains the parts
                return action.bindings[bindingIndex-1].name;
            }
            else { return action.GetBindingDisplayString(); } // if not a composite, bindingId can just be updated
        }

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
