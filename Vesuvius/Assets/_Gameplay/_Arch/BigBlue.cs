using System.Collections;
using _ScriptableObjects;
using _ScriptableObjects.Dialogue.Arch;
using ES3Types;
using Public.Tarodev_2D_Controller.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Gameplay._Arch
{
    public class BigBlue : MonoBehaviour
    {

        [Header("Component References")]
        [SerializeField] private PlayerSpawner playerSpawner;
        [SerializeField] private NPCDialogueCollection dialogueCollection;
        [SerializeField] private BigBlueAudio bigBlueAudio;

        [Header("GO References")]
        [SerializeField] private GameObject player;
        [SerializeField] private Animator animator; // Animator reference for Big Blue

        private void Awake()
        {
            bigBlueAudio = this.gameObject.GetComponent<BigBlueAudio>();
        }

        private void OnEnable()
        {
            bigBlueAudio.PlayBigBlueSolo();
        }

        private void OnDisable()
        {
            bigBlueAudio.StopBigBlueSolo();
        }

        private void Start()
        {
            animator.Play("Playing"); // Start with the "Playing" animation
        }



        public void StartDialogue()
        {

            // Play the "PutDown" animation before transitioning to "Idle"
            animator.Play("PutDown");
            bigBlueAudio.StopBigBlueSolo();
            StartCoroutine(WaitForAnimation("PutDown", () =>
            {
                animator.Play("Idle"); // Transition to "Idle" after "PutDown" finishes
                //dialogueCanvas.gameObject.SetActive(true); // Show the dialogue canvas
                //currentDialogueIndex = 0; // Reset dialogue index
                //DisplayNextDialogue();
            }));
        }

        private void DisplayNextDialogue()
        {
            //string wwiseEvent = dialogueTree.wwiseEvents[currentDialogueIndex];
            //if (!string.IsNullOrEmpty(wwiseEvent))
            //{
            //    bigBlueAudio.PlayWwiseEvent(wwiseEvent);
            //}
        }

        private void EndDialogue()
        {

            // Play the "PutBack" animation before transitioning to "Playing"
            animator.Play("PutBack");
            bigBlueAudio.PlayBigBlueSolo();
            StartCoroutine(WaitForAnimation("PutBack", () =>
            {
                animator.Play("Playing"); // Transition back to "Playing"
                PlayerControlManager.Instance.EnableNormalControls();
                //HandleProximityLogic(); // Reactivate proximity logic
            }));
        }

        private IEnumerator WaitForAnimation(string animationName, System.Action onComplete)
        {
            yield return new WaitForEndOfFrame();
            while (animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) &&
                   animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            }
            onComplete?.Invoke();
        }
    }
}
