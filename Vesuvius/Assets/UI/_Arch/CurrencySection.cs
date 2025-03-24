using ScriptableObjects.PlayerInfo;
using TMPro;
using UnityEngine;

namespace UI._Arch
{
    public class CurrencySection : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text currencyText;
        [SerializeField] private TMP_Text signText; // New TMP text for the sign
        [SerializeField] private TMP_Text numberText; // New TMP text for the number
        [SerializeField] private UnityEngine.UI.Image currencyImage;

        [Header("Player Info Reference")]
        [SerializeField] private PlayerInfo playerInfo;

        [Header("Transition Settings")]
        [SerializeField] private float transitionSpeed = 20f; // Units per second
        [SerializeField] private float changeTextDisplayTime = 5f; // Default display time for change text

        private int displayedCurrency; // The currency value currently displayed
        private float currencyUpdateTimer = 0f; // Timer for the transition
        private float changeTextTimer = 0f; // Timer for hiding the change text

        private void Start()
        {
            // Initialize the displayed currency
            displayedCurrency = playerInfo.GetTotalCurrency();

            // Initial update for the currency text
            UpdateCurrencyText();

            // Ensure the change texts are hidden initially
            if (signText != null) signText.gameObject.SetActive(false);
            if (numberText != null) numberText.gameObject.SetActive(false);
        }

        private void Update()
        {
            int targetCurrency = playerInfo.GetTotalCurrency();
            if (displayedCurrency != targetCurrency)
            {
                // Transition displayedCurrency towards targetCurrency
                currencyUpdateTimer += Time.deltaTime * transitionSpeed;

                if (currencyUpdateTimer >= 1f)
                {
                    int steps = Mathf.FloorToInt(currencyUpdateTimer);
                    currencyUpdateTimer -= steps;

                    if (displayedCurrency < targetCurrency)
                    {
                        displayedCurrency = Mathf.Min(displayedCurrency + steps, targetCurrency);
                    }
                    else if (displayedCurrency > targetCurrency)
                    {
                        displayedCurrency = Mathf.Max(displayedCurrency - steps, targetCurrency);
                    }

                    UpdateCurrencyText();
                }
            }
            else
            {
                currencyUpdateTimer = 0f;
            }

            // Handle hiding the change text after the display time
            if (signText.gameObject.activeSelf || numberText.gameObject.activeSelf)
            {
                changeTextTimer -= Time.deltaTime;
                if (changeTextTimer <= 0f)
                {
                    if (signText != null) signText.gameObject.SetActive(false);
                    if (numberText != null) numberText.gameObject.SetActive(false);
                }
            }
        }

        private void UpdateCurrencyText()
        {
            // Update the currency text with the displayed value
            currencyText.text = displayedCurrency.ToString();
        }

        public void ShowChangeCurrencyText(float displayTime = -1f)
        {
            if (signText == null || numberText == null || playerInfo == null) return;

            // Determine the text to display
            int changeAmount = playerInfo.lastCurrencyChangeAmount;

            // Update the sign and number separately
            signText.text = changeAmount > 0 ? "+" : "-";
            numberText.text = Mathf.Abs(changeAmount).ToString();

            // Make the texts visible
            signText.gameObject.SetActive(true);
            numberText.gameObject.SetActive(true);

            // Set the timer for hiding the text
            changeTextTimer = displayTime > 0f ? displayTime : changeTextDisplayTime;

            Debug.Log("Checkpoint 1");
        }
    }
}
