using System.Collections;
using TarodevController;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BigBlue : MonoBehaviour
{
    [Header("UI References")]
    public Canvas npcTalkCanvas;
    public TMP_Text npcTalkText;
    public Canvas dialogueCanvas;
    public TMP_Text dialogueText;

    [Header("Component References")]
    [SerializeField] private PlayerSpawner playerSpawner;
    [SerializeField] private NPCDialogueCollection dialogueCollection;
    [SerializeField] private BigBlueAudio bigBlueAudio;

    [Header("GO References")]
    [SerializeField] private GameObject player;
    [SerializeField] private Animator animator; // Animator reference for Big Blue

    [Header("Settings")]
    public float activationDistance = 5f;
    public float lerpSpeed = 2f;
    public float pulseSpeed = 1.2f;
    public float pulseIntensity = 0.06f;
    public float typingSpeed = 0.05f;
    public float dialoguePulseSpeed = 0.5f;
    public float dialoguePulseIntensity = 0.02f;
    public bool isForMainMenu = false;

    public string currentDialogueTreeID;

    private bool isPlayerNear = false;
    private bool isTalking = false;
    private bool isTyping = false;
    private int currentDialogueIndex = 0;
    private Color targetColor;
    private PlayerControls _controls;

    private void Awake()
    {
        _controls = new PlayerControls();
        bigBlueAudio = this.gameObject.GetComponent<BigBlueAudio>();
    }

    private void OnEnable()
    {
        _controls.Player.Interact.performed += OnInteractPerformed;
        _controls.Enable();
        bigBlueAudio.PlayBigBlueSolo();
    }

    private void OnDisable()
    {
        _controls.Player.Interact.performed -= OnInteractPerformed;
        _controls.Disable();
        bigBlueAudio.StopBigBlueSolo();
    }

    private void Start()
    {
        FindPlayer();
        InitializeUI();
        animator.Play("Playing"); // Start with the "Playing" animation
    }

    private void InitializeUI()
    {
        npcTalkCanvas.gameObject.SetActive(true); // Keep the talk canvas always active
        dialogueCanvas.gameObject.SetActive(false);
        SetTextAlpha(0); // Start with invisible text
    }

    private void FindPlayer()
    {
        if (isForMainMenu) {
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
            HandleProximityLogic();

            if (isPlayerNear)
            {
                ApplyTextEffects();
            }
        }
        else
        {
            SetTargetAlpha(0); // Ensure the talk prompt is faded out when in dialogue mode
            LerpTextAlpha();
            ApplyDialogueTextEffects();
        }
    }

    private void HandleProximityLogic()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        isPlayerNear = distance <= activationDistance;

        if (!isTalking)
        {
            if (isPlayerNear)
            {
                SetTargetAlpha(1); // Fade in the talk prompt
            }
            else
            {
                SetTargetAlpha(0); // Fade out the talk prompt
            }
        }

        LerpTextAlpha();
    }

    private void LerpTextAlpha()
    {
        var currentColor = npcTalkText.color;
        npcTalkText.color = Color.Lerp(
            currentColor,
            new Color(currentColor.r, currentColor.g, currentColor.b, targetColor.a),
            Time.deltaTime * lerpSpeed
        );
    }

    private void SetTargetAlpha(float alpha)
    {
        targetColor = new Color(npcTalkText.color.r, npcTalkText.color.g, npcTalkText.color.b, alpha);
    }

    private float GetTextAlpha()
    {
        return npcTalkText.color.a;
    }

    private void SetTextAlpha(float alpha)
    {
        npcTalkText.color = new Color(npcTalkText.color.r, npcTalkText.color.g, npcTalkText.color.b, alpha);
    }

    private void ApplyTextEffects()
    {
        float scaleFactor = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
        npcTalkText.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
    }

    private void StartDialogue()
    {
        isTalking = true;
        isPlayerNear = false;

        // Freeze player movement immediately
        player.GetComponent<PlayerController>().FreezeMode = true;

        // Play the "PutDown" animation before transitioning to "Idle"
        animator.Play("PutDown");
        bigBlueAudio.StopBigBlueSolo();
        StartCoroutine(WaitForAnimation("PutDown", () =>
        {
            animator.Play("Idle"); // Transition to "Idle" after "PutDown" finishes
            dialogueCanvas.gameObject.SetActive(true); // Show the dialogue canvas
            currentDialogueIndex = 0; // Reset dialogue index
            DisplayNextDialogue();
        }));
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
                    bigBlueAudio.PlayWwiseEvent(wwiseEvent);
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

        // Play the "PutBack" animation before transitioning to "Playing"
        animator.Play("PutBack");
        bigBlueAudio.PlayBigBlueSolo();
        StartCoroutine(WaitForAnimation("PutBack", () =>
        {
            animator.Play("Playing"); // Transition back to "Playing"
            player.GetComponent<PlayerController>().FreezeMode = false; // Unfreeze player movement
            HandleProximityLogic(); // Reactivate proximity logic
        }));
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
                DisplayNextDialogue(); // Go to next dialogue if not typing
            }
        }
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
