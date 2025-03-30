using System.Collections;
using _ScriptableObjects.PlayerInfo;
using Public.Tarodev_2D_Controller.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace _Gameplay._Arch
{
    public class PlayerHealing : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerInfo playerInfo;
        [SerializeField] private Animator animator; // Animator of the player sprite
        [SerializeField] private ParticleSystem healingParticles; // Looping particle effect
        [SerializeField] private ParticleSystem healedBurstParticles; // Burst particle effect
        [SerializeField] private PlayerController playerController; // Reference to the PlayerController script
        [SerializeField] private Image whiteOverlay; // Reference to the White Overlay UI Image
        [SerializeField] private float healAnimationLength = 1.5f;

        [Header("Overlay Settings")]
        [SerializeField] private float overlayAlpha = 0.5f; // Target alpha for the overlay
        [SerializeField] private float overlayLerpSpeed = 5f; // Speed of the lerp

        private PlayerControls playerControls;

        private bool isHealing = false; // Tracks if the player is currently healing
        private bool overlayActive = false; // Tracks if overlay lerp is active

        private void Awake()
        {
            playerControls = new PlayerControls();

            // Map the Heal action to the OnHealInput method
            playerControls.Player.Heal.performed += OnHealInput;
        }

        private void OnEnable()
        {
            playerControls.Player.Enable();
        }

        private void OnDisable()
        {
            playerControls.Player.Disable();
        }

        public void OnHealInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                TryHeal();
            }
        }

        private void TryHeal()
        {
            // Abort if already healing, not enough totems, or at max health
            if (isHealing || playerInfo.GetCurrentHealth() >= playerInfo.GetMaximumHealth() || !playerInfo.BuyAbility())
            {
                return;
            }

            // Begin healing
            StartHealing();
        }

        private void StartHealing()
        {
            isHealing = true;

            // Freeze player movement
            playerController.SetFreezeMode(true);

            // Play healing animation
            animator.SetTrigger("Heal");

            // Start healing particles
            healingParticles.Play();

            // Schedule healing completion after the animation duration
            Invoke(nameof(CompleteHealing), healAnimationLength);
        }

        private void CompleteHealing()
        {
            if (!isHealing) return;

            // Increase player health
            playerInfo.SetCurrentHealth(playerInfo.GetCurrentHealth() + 1);

            // Stop healing particles and play healed burst effect
            healingParticles.Stop();
            healedBurstParticles.Play();

            // Start the white overlay lerp effect
            StartCoroutine(LerpOverlayAlpha(overlayAlpha, overlayLerpSpeed));

            // Reset freeze mode and return to normal animations
            playerController.SetFreezeMode(false);
            isHealing = false;
        }

        private void InterruptHealing()
        {
            if (!isHealing) return;

            // Cancel scheduled CompleteHealing invocation
            CancelInvoke(nameof(CompleteHealing));

            // Stop healing particles
            healingParticles.Stop();

            // Reset freeze mode and return to normal animations
            playerController.SetFreezeMode(false);
            isHealing = false;

            // Reset healing animation state
            animator.ResetTrigger("Heal");
            animator.Play("Idle"); // Assuming "Idle" is the default animation state

            // Ensure any overlay effects are stopped
            StopAllCoroutines();
            ResetOverlayAlpha();
        }

        public void CancelHeal()
        {
            // This method performs the same actions as InterruptHealing
            InterruptHealing();
        }

        private IEnumerator LerpOverlayAlpha(float targetAlpha, float speed)
        {
            overlayActive = true;

            // Get the current color of the overlay
            Color overlayColor = whiteOverlay.color;

            // Lerp to the target alpha
            while (Mathf.Abs(overlayColor.a - targetAlpha) > 0.001f)
            {
                overlayColor.a = Mathf.Lerp(overlayColor.a, targetAlpha, Time.deltaTime * speed);
                whiteOverlay.color = overlayColor;
                yield return null;
            }

            // Lerp back to alpha 0
            while (Mathf.Abs(overlayColor.a - 0f) > 0.001f)
            {
                overlayColor.a = Mathf.Lerp(overlayColor.a, 0f, Time.deltaTime * speed);
                whiteOverlay.color = overlayColor;
                yield return null;
            }

            // Ensure final alpha is exactly 0
            overlayColor.a = 0f;
            whiteOverlay.color = overlayColor;

            overlayActive = false;
        }

        private void ResetOverlayAlpha()
        {
            // Immediately reset the overlay alpha to 0
            Color overlayColor = whiteOverlay.color;
            overlayColor.a = 0f;
            whiteOverlay.color = overlayColor;
            overlayActive = false;
        }
    }
}
