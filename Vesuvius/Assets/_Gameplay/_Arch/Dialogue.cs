using System.Collections;
using _ScriptableObjects.Dialogue.Arch;
using Public.Tarodev_2D_Controller.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Gameplay._Arch
{
    public class Dialogue : MonoBehaviour
    {
        [Header("UI References")]
        public Canvas dialogueCanvas;
        public TMP_Text dialogueText;

        [Header("Component References")]
        [SerializeField] private PlayerSpawner playerSpawner;
        [SerializeField] private NPCDialogueCollection dialogueCollection;

        [Header("GO References")]
        [SerializeField] private GameObject player;

        [Header("Settings")]
        public float activationDistance = 5f;
        public float lerpSpeed = 2f;
        public float pulseSpeed = 1.2f;
        public float pulseIntensity = 0.06f;
        public float typingSpeed = 0.05f;
        public float dialoguePulseSpeed = 0.5f;
        public float dialoguePulseIntensity = 0.02f;
        public bool isForMainMenu = false;
        public bool waitForSignalBeforeContinue = false;

        public string currentDialogueTreeID;

        private bool isPlayerNear = false;
        private bool isTalking = false;
        private bool isTyping = false;
        private bool canContinue = false;
        private int currentDialogueIndex = 0;
        private Color targetColor;


        private void Awake()
        {

        }

        private void OnEnable()
        {
            PlayerControlManager.Instance.controls.Player.Interact.performed += OnInteractPerformed;
        }

        private void OnDisable()
        {
            PlayerControlManager.Instance.controls.Player.Interact.performed -= OnInteractPerformed;
        }

        private void Start()
        {
            FindPlayer();
            InitializeUI();
        }

        private void InitializeUI()
        {
            dialogueCanvas.gameObject.SetActive(false);
        }

        private void FindPlayer()
        {
            if (isForMainMenu)
            {
                return;
            }
            playerSpawner = FindFirstObjectByType<PlayerSpawner>();
            if (playerSpawner == null)
            {
                Debug.LogError("PlayerSpawner not assigned or not found.");
                return;
            }
            player = playerSpawner.GetRuntimePlayer();
        }

        private void Update()
        {
            if (!isTalking)
            {

            }
            else
            {
                ApplyDialogueTextEffects();
            }
        }

        public void StartDialogue()
        {
            isTalking = true;
            isPlayerNear = false;

            // Freeze player movement immediately
            print("Player frozen");
            PlayerControlManager.Instance.DisableNormalControls();

            dialogueCanvas.gameObject.SetActive(true); // Show the dialogue canvas
            currentDialogueIndex = 0; // Reset dialogue index
            DisplayNextDialogue();
        }

        private void DisplayNextDialogue()
        {
            var dialogueTree = dialogueCollection.GetDialogueTreeByID(currentDialogueTreeID);

            if (dialogueTree != null && currentDialogueIndex < dialogueTree.dialogues.Count)
            {
                // Play the associated Wwise event if specified
                if (dialogueTree.wwiseEvents != null && currentDialogueIndex < dialogueTree.wwiseEvents.Count)
                {
                    string wwiseEvent = dialogueTree.wwiseEvents[currentDialogueIndex];
                    if (!string.IsNullOrEmpty(wwiseEvent))
                    {

                    }
                }

                // Display the dialogue text
                isTyping = true; // Set typing flag
                StartCoroutine(TypeDialogue(dialogueTree.dialogues[currentDialogueIndex]));
                currentDialogueIndex++;
            }
            else
            {
                EndDialogue();
            }
        }


        private IEnumerator TypeDialogue(string dialogue)
        {
            dialogueText.text = "";
            foreach (char letter in dialogue)
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
            isTyping = false; // Reset typing flag when done
        }

        private void EndDialogue()
        {
            var dialogueTree = dialogueCollection.GetDialogueTreeByID(currentDialogueTreeID);
            if (dialogueTree != null && currentDialogueIndex >= dialogueTree.dialogues.Count)
            {
                if (!string.IsNullOrEmpty(dialogueTree.nextTreeID))
                {
                    currentDialogueTreeID = dialogueTree.nextTreeID;
                }
                else
                {
                    Debug.LogWarning($"No nextTreeID defined for dialogue tree: {currentDialogueTreeID}. Staying on the current tree.");
                }
            }

            isTalking = false;
            dialogueCanvas.gameObject.SetActive(false);
            dialogueTree.eventsOnDialogueEnd.Invoke();


            PlayerControlManager.Instance.EnableNormalControls();
        }

        private void ApplyDialogueTextEffects()
        {
            float scaleFactor = 1 + Mathf.Sin(Time.time * dialoguePulseSpeed) * dialoguePulseIntensity;
            dialogueText.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            if (!isTalking && isPlayerNear)
            {
                StartDialogue();
            }
            else if (isTalking)
            {
                if (isTyping) // If still typing, skip to end of current dialogue
                {
                    StopAllCoroutines();
                    dialogueText.text = dialogueCollection.GetDialogueTreeByID(currentDialogueTreeID)
                        .dialogues[currentDialogueIndex - 1];
                    isTyping = false; // Reset typing flag
                }
                else
                {
                    if (!waitForSignalBeforeContinue)
                    {
                        DisplayNextDialogue(); // Go to next dialogue if not typing
                    }
                    else if (canContinue)
                    {
                        DisplayNextDialogue();
                        canContinue = false;
                    }

                }
            }
        }

        public void SignalContinue()
        {
            canContinue = true;
        }
    }
}
