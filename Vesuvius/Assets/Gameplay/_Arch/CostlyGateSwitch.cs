using ScriptableObjects.PlayerInfo;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Gameplay._Arch
{
    public class CostlyGateSwitch : GateSwitch
    {
        [Header("Cost UI References")]
        public TMP_Text costText;
        public Image costImage;

        [Header("Cost Settings")]
        public int cost = 75; // Default cost value
        public Color enoughCurrencyColor = Color.green;
        public Color notEnoughCurrencyColor = Color.red;

        [Header("Player Info Reference")]
        public PlayerInfo playerInfo; // Reference to the PlayerInfo ScriptableObject

        private Color costTargetColor; // Current target color for both text and image
        private float costTextAlphaTarget = 0;
        private float costImageAlphaTarget = 0;
        [Header("Events")]
        public UnityEvent currencyChanged;

        protected override void InitializeUI()
        {
            base.InitializeUI();

            // Ensure the cost text and image are initialized
            if (costText != null) SetCostTextAlpha(0);
            if (costImage != null) SetCostImageAlpha(0);

            // Set the cost text to the cost amount
            UpdateCostText();
        }

        protected override void HandleSwitchProximityLogic()
        {
            base.HandleSwitchProximityLogic();

            if (isPlayerNear)
            {
                ActivateCostUI();
            }
            else
            {
                DeactivateCostUI();
            }

            UpdateCostUIColor(); // Update all relevant UI colors
            LerpCostUIAlpha(); // Lerp cost UI with the same logic as the prompt
        }

        private void ActivateCostUI()
        {
            costTextAlphaTarget = 1;
            costImageAlphaTarget = 1;
        }

        private void DeactivateCostUI()
        {
            costTextAlphaTarget = 0;
            costImageAlphaTarget = 0;
        }

        private void LerpCostUIAlpha()
        {
            if (costText != null)
            {
                var currentColor = costText.color;
                costText.color = Color.Lerp(
                    currentColor,
                    new Color(costTargetColor.r, costTargetColor.g, costTargetColor.b, costTextAlphaTarget),
                    Time.deltaTime * lerpSpeed
                );
            }

            if (costImage != null)
            {
                var currentColor = costImage.color;
                costImage.color = Color.Lerp(
                    currentColor,
                    new Color(costTargetColor.r, costTargetColor.g, costTargetColor.b, costImageAlphaTarget),
                    Time.deltaTime * lerpSpeed
                );
            }

            if (interactPromptText != null)
            {
                var currentPromptColor = interactPromptText.color;
                interactPromptText.color = Color.Lerp(
                    currentPromptColor,
                    new Color(costTargetColor.r, costTargetColor.g, costTargetColor.b, currentPromptColor.a),
                    Time.deltaTime * lerpSpeed
                );
            }
        }

        private void SetCostTextAlpha(float alpha)
        {
            if (costText != null)
            {
                var currentColor = costText.color;
                costText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            }
        }

        private void SetCostImageAlpha(float alpha)
        {
            if (costImage != null)
            {
                var currentColor = costImage.color;
                costImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            }
        }

        private void UpdateCostText()
        {
            if (costText != null)
            {
                costText.text = cost.ToString(); // Update the text to show the cost amount
            }
        }

        private void UpdateCostUIColor()
        {
            if (playerInfo == null) return;

            // Check if the player has enough currency
            bool hasEnoughCurrency = playerInfo.GetTotalCurrency() >= cost;

            // Set target color based on currency status
            costTargetColor = hasEnoughCurrency ? enoughCurrencyColor : notEnoughCurrencyColor;
        }

        protected override void OnInteract(InputAction.CallbackContext context)
        {
            if (!isPlayerNear || gate == null || gateUnlocked) return;

            // Check if the player has enough currency
            if (playerInfo != null && playerInfo.GetTotalCurrency() >= cost)
            {
                // Deduct the cost
                playerInfo.AddCurrency(-cost);
                currencyChanged.Invoke();

                // Unlock the gate
                base.OnInteract(context);

                Debug.Log($"Gate unlocked. {cost} currency deducted. Remaining currency: {playerInfo.GetTotalCurrency()}");
            }
            else
            {
                Debug.Log("Not enough currency to unlock the gate.");
            }
        }
    }
}
