using System;
using _Gameplay._Arch;
using _ScriptableObjects;
using Public.Tarodev_2D_Controller.Scripts;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GuidComponent))]
public class Interactable : MonoBehaviour
{
    [Header("UI References")]
    public Canvas canvas; //canvas with interact text object in it
    public TMP_Text interactPromptText;
    public TMP_Text[] otherTexts;
    public ControlPrompts controlPrompts;

    [Header("Settings")]
    public float activationRadius = 2f; // Distance for interaction
    public UnityEvent OnInteract; //actions to execute on valid interaction
    public UnsavedPlayerInfo unsavedPlayerInfo;
    public BinaryStateStorage blockedInteractables;
    public string interactableID;
    public bool interactOnce;

    private GameObject player;
    private PlayerControls playerControls;
    private bool isPlayerNear = false; // Tracks if the player is near
    private Color targetColor;
    private GuidComponent guidComponent;
    public float lerpSpeed = 2f;

    private void Awake()
    {
        playerControls = new PlayerControls();
        guidComponent = GetComponent<GuidComponent>();
    }

    private void OnEnable()
    {
        playerControls.Player.Interact.performed += OnInteractPerformed;
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Interact.performed -= OnInteractPerformed;
        playerControls.Player.Disable();
    }

    public void OnControlsChanged()
    {
        UpdateUIHints(); // Force re-generation of our cached text strings to pick up new bindings.
    }

    private void UpdateUIHints()
    {
        interactPromptText.text = controlPrompts.interactPrompt;
    }

    private void Start()
    {
        player = FindFirstObjectByType<PlayerSpawner>()?.GetRuntimePlayer();
        if (player == null)
        {
            Debug.LogError("Player not found! Ensure PlayerSpawner and runtime player setup is correct.");
            return;
        }

        InitializeUI();
    }

    private void InitializeUI()
    {
        canvas.gameObject.SetActive(false);
        SetTextAlpha(0);
    }

    private void Update()
    {
        if(interactOnce && blockedInteractables.isInteractableBlocked(guidComponent.GetGuid().ToString()))
        {
            print("blocked");
            canvas.gameObject.SetActive(false);
            this.enabled = false;
        }

        if (unsavedPlayerInfo.isInMenuMode)
        {
            HandlePromptLerpOut(); // Smoothly lerp the prompt out when the menu is active
            return;
        }

        HandleProximityLogic();
    }

    private void HandleProximityLogic()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        isPlayerNear = distance <= activationRadius;

        if (isPlayerNear)
        {
            ActivatePrompt();
        }
        else
        {
            DeactivatePrompt();
        }

        LerpTextAlpha();
    }

    private void ActivatePrompt()
    {
        if (!canvas.gameObject.activeSelf)
        {
            canvas.gameObject.SetActive(true);
        }
        SetTargetAlpha(1);
    }

    private void DeactivatePrompt()
    {
        SetTargetAlpha(0);
        if (GetTextAlpha() <= 0.01f && canvas.gameObject.activeSelf)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    private void HandlePromptLerpOut()
    {
        SetTargetAlpha(0); // Set the target alpha to 0 for a smooth fade-out
        LerpTextAlpha();

        if (GetTextAlpha() <= 0.01f && canvas.gameObject.activeSelf)
        {
            canvas.gameObject.SetActive(false); // Deactivate the canvas after fade-out
        }
    }

    private void LerpTextAlpha()
    {
        var currentColor = interactPromptText.color;
        interactPromptText.color = Color.Lerp(
            currentColor,
            new Color(currentColor.r, currentColor.g, currentColor.b, targetColor.a),
            Time.deltaTime * lerpSpeed
        );
        foreach(TMP_Text text in otherTexts)
        {
            text.color = Color.Lerp(
                currentColor,
                new Color(currentColor.r, currentColor.g, currentColor.b, targetColor.a),
                Time.deltaTime * lerpSpeed
            );
        }

    }

    private void SetTargetAlpha(float alpha)
    {
        targetColor = new Color(interactPromptText.color.r, interactPromptText.color.g, interactPromptText.color.b, alpha);
        foreach(TMP_Text text in otherTexts)
        {
            text.color = new Color(interactPromptText.color.r, interactPromptText.color.g, interactPromptText.color.b, alpha);
        }
    }

    private float GetTextAlpha()
    {
        return interactPromptText.color.a;
    }

    private void SetTextAlpha(float alpha)
    {
        interactPromptText.color = new Color(interactPromptText.color.r, interactPromptText.color.g, interactPromptText.color.b, alpha);
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if(unsavedPlayerInfo.isInMenuMode)
        {
            return;
        }
        else if (isPlayerNear)
        {
            OnInteract.Invoke();
            if(interactOnce)
            {
                blockedInteractables.Add(guidComponent.GetGuid().ToString());
                HandlePromptLerpOut();
            }
        }
    }
}
