using UnityEngine;
using TMPro;

public class TotemSection : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text currentTotemsText;
    [SerializeField] private TMP_Text dividerText;
    [SerializeField] private TMP_Text requiredTotemsText;
    [SerializeField] private UnityEngine.UI.Image totemImage;

    [Header("Player Info Reference")]
    [SerializeField] private PlayerInfo playerInfo;

    [Header("Color Settings")]
    [SerializeField] private Color insufficientTotemsColor = Color.red; // Customizable color for insufficient totems

    [Header("Transition Settings")]
    [SerializeField] private float transitionSpeed = 5f; // Integers per second

    private Color initialTextColor; // Store the initial text color
    private int displayedTotems; // The totem value currently displayed
    private float totemUpdateTimer = 0f; // Timer for the transition

    private void Start()
    {
        // Cache the initial color of the texts
        if (currentTotemsText != null)
        {
            initialTextColor = currentTotemsText.color;
        }

        // Initialize the displayed totems
        displayedTotems = playerInfo.GetTotemPower();

        // Initial Update
        UpdateTotemSection();
    }

    private void Update()
    {
        int targetTotems = playerInfo.GetTotemPower();
        if (displayedTotems != targetTotems)
        {
            // Transition displayedTotems towards targetTotems
            totemUpdateTimer += Time.deltaTime * transitionSpeed;

            if (totemUpdateTimer >= 1f)
            {
                int steps = Mathf.FloorToInt(totemUpdateTimer);
                totemUpdateTimer -= steps;

                if (displayedTotems < targetTotems)
                {
                    displayedTotems = Mathf.Min(displayedTotems + steps, targetTotems);
                }
                else if (displayedTotems > targetTotems)
                {
                    displayedTotems = Mathf.Max(displayedTotems - steps, targetTotems);
                }

                UpdateTotemSection();
            }
        }
        else
        {
            totemUpdateTimer = 0f;
        }
    }

    private void UpdateTotemSection()
    {
        // Update texts based on displayedTotems and requiredTotems
        int requiredTotems = playerInfo.GetAbilityCost();

        currentTotemsText.text = displayedTotems.ToString();
        requiredTotemsText.text = requiredTotems.ToString();

        // Change color based on comparison
        bool insufficientTotems = displayedTotems < requiredTotems;
        Color targetColor = insufficientTotems ? insufficientTotemsColor : initialTextColor;

        // Apply colors
        currentTotemsText.color = targetColor;
        dividerText.color = targetColor;
        requiredTotemsText.color = targetColor;
    }
}
