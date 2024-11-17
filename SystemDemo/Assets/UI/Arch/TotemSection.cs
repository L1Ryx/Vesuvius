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

    private Color initialTextColor; // Store the initial text color

    private void Start()
    {
        // Cache the initial color of the texts
        if (currentTotemsText != null)
        {
            initialTextColor = currentTotemsText.color;
        }

        // Initial Update
        UpdateTotemSection();
    }

    private void Update()
    {
        // Continuously update the Totem Section
        UpdateTotemSection();
    }

    private void UpdateTotemSection()
    {
        // Update texts based on PlayerInfo values
        int currentTotems = playerInfo.GetTotemPower();
        int requiredTotems = playerInfo.GetAbilityCost();

        currentTotemsText.text = currentTotems.ToString();
        requiredTotemsText.text = requiredTotems.ToString();

        // Change color based on comparison
        bool insufficientTotems = currentTotems < requiredTotems;
        Color targetColor = insufficientTotems ? insufficientTotemsColor : initialTextColor;

        // Apply colors
        currentTotemsText.color = targetColor;
        dividerText.color = targetColor;
        requiredTotemsText.color = targetColor;
    }
}
