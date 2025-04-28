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

        //Hacky context switching for tutorials based on input
        private InputDevice lastDevice = null;
        private InputControl lastControl = null;

        public void OnEnable()
        {
            InputSystem.onActionChange += (obj, change) =>
            {
                
                if (change == InputActionChange.ActionPerformed)
                {
                    var inputAction = (InputAction) obj;
                    lastControl = inputAction.activeControl;
                    if(lastDevice == null)
                    {
                        lastDevice = lastControl.device;
                    }

                    //print(lastDevice.displayName);
                   
                }

                if(lastDevice != lastControl.device)
                {
                    lastDevice = lastControl.device;
                    if(lastDevice.displayName == "Xbox Controller")
                    {
                        moveText.text = "<- Left Stick ->";
                        jumpText.text = "A - Jump";
                        interactionText.text = "A - Interact";
                        slashText.text = "X - Slash";
                        rebalanceText.text = "B - Heal";
                    }
                    else if(lastDevice.displayName == "DualSense Wireless Controller")
                    {
                        moveText.text = "<- Left Stick ->";
                        jumpText.text = "X - Jump";
                        interactionText.text = "X - Interact";
                        slashText.text = "Square - Slash";
                        rebalanceText.text = "Circle - Heal";
                    }
                    else
                    {
                        moveText.text = "<- Arrow Keys ->";
                        jumpText.text = "SPACE - Jump";
                        interactionText.text = "SPACE - Interact";
                        slashText.text = "Z - Slash";
                        rebalanceText.text = "X - Heal";
                    }
                }
            };
        }
        //end hack

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
}
