using UnityEngine;
using TMPro;

public class CurrencySection : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text currencyText;
    [SerializeField] private UnityEngine.UI.Image currencyImage;

    [Header("Player Info Reference")]
    [SerializeField] private PlayerInfo playerInfo;

    private void Start()
    {
        // Initial update for the currency text
        UpdateCurrencyText();
    }

    private void Update()
    {
        // Continuously update the currency text
        UpdateCurrencyText();
    }

    private void UpdateCurrencyText()
    {
        // Update the currency text with the current value from PlayerInfo
        currencyText.text = playerInfo.GetTotalCurrency().ToString();
    }
}
