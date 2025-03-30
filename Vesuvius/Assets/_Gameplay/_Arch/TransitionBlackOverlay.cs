using UnityEngine;
using UnityEngine.UI;

namespace _Gameplay._Arch
{
    public class SceneTransitionOverlay : MonoBehaviour
    {
        [Header("Fade Settings")]
        public float fadeSpeed = 1f; // Speed of the fade effect

        private Image overlayImage;

        private void Awake()
        {
            // Get the Image component
            overlayImage = GetComponent<Image>();
            if (overlayImage == null)
            {
                Debug.LogError("No Image component found on the Transition Overlay GameObject!");
                return;
            }

            // Set initial alpha to 1 (fully black for fade-in effect)
            SetAlpha(1f);
        }


        private void SetAlpha(float alpha)
        {
            if (overlayImage != null)
            {
                Color color = overlayImage.color;
                color.a = Mathf.Clamp01(alpha);
                overlayImage.color = color;
            }
        }

        private System.Collections.IEnumerator FadeToTransparent()
        {
            float currentAlpha = 1;

            while (currentAlpha > 0f)
            {
                currentAlpha -= Time.deltaTime * fadeSpeed;
                SetAlpha(currentAlpha);
                yield return null;
            }

            // Ensure it's completely transparent
            SetAlpha(0f);
            // Optionally, deactivate the overlay once faded out
            // gameObject.SetActive(false);
        }

        public void FadeOutOverlay() {
            gameObject.SetActive(true);
            StartCoroutine(FadeToTransparent());
        }
        // Public method to fade the overlay in (simulate a blackout)
        public void FadeToBlack()
        {
            // Reactivate the overlay if it was deactivated
            gameObject.SetActive(true);
            StartCoroutine(FadeToOpaque());
        }

        private System.Collections.IEnumerator FadeToOpaque()
        {
            float currentAlpha = overlayImage.color.a;

            while (currentAlpha < 1f)
            {
                currentAlpha += Time.deltaTime * fadeSpeed;
                SetAlpha(currentAlpha);
                yield return null;
            }

            // Ensure it's completely opaque
            SetAlpha(1f);
        }
    }
}
