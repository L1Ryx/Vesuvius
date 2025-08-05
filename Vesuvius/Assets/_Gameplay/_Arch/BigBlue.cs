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
    public class BigBlue : Dialogue
    {
        [Header("Component References")]
        [SerializeField] private BigBlueAudio bigBlueAudio;

        [Header("GO References")]
        [SerializeField] private Animator animator; // Animator reference for Big Blue

        private void Awake()
        {
            bigBlueAudio = this.gameObject.GetComponent<BigBlueAudio>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            bigBlueAudio.StopBigBlueSolo();
        }

        protected override void Start()
        {
            base.Start();
            bigBlueAudio.PlayBigBlueSolo();
            animator.Play("Playing"); // Start with the "Playing" animation
        }

        public override void StartDialogue()
        {
            base.StartDialogue();
            // Play the "PutDown" animation before transitioning to "Idle"
            animator.Play("PutDown");
            bigBlueAudio.StopBigBlueSolo();
            StartCoroutine(WaitForAnimation("PutDown", () =>
            {
                animator.Play("Idle"); // Transition to "Idle" after "PutDown" finishes
            }));
        }

        protected override void EndDialogue()
        {
            base.EndDialogue();
            // Play the "PutBack" animation before transitioning to "Playing"
            animator.Play("PutBack");
            bigBlueAudio.PlayBigBlueSolo();
            StartCoroutine(WaitForAnimation("PutBack", () =>
            {
                animator.Play("Playing"); // Transition back to "Playing"
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
