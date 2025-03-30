using TMPro;
using UnityEngine;

namespace _Gameplay._Arch
{
    public class WoodenSign : MonoBehaviour
    {
        [Header("UI References")]
        public Canvas woodenSignCanvas;
        public TMP_Text woodenSignText;

        [Header("Component References")]
        [SerializeField] private PlayerSpawner playerSpawner;

        [Header("GO References")]
        [SerializeField] private GameObject player;

        [Header("Settings")]
        public float activationDistance = 5f;
        public float lerpSpeed = 2f;
        public float pulseSpeed = 1.2f; // Speed of the pulsating effect
        public float pulseIntensity = 0.06f; // Intensity of the pulsating effect

        private Color targetColor;
        private bool isPlayerNear = false;

        private void Start()
        {
            FindPlayer();
            InitializeUI();
        }

        private void InitializeUI()
        {
            woodenSignCanvas.gameObject.SetActive(false);
            SetTextAlpha(0);
        }

        private void FindPlayer()
        {
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
            HandleSignProximityLogic();

            if (isPlayerNear)
            {
                ApplyTextEffects();
            }
        }

        private void HandleSignProximityLogic()
        {
            if (player == null) return;

            float distance = Vector3.Distance(player.transform.position, transform.position);
            isPlayerNear = distance <= activationDistance;

            if (isPlayerNear)
            {
                ActivateSign();
            }
            else
            {
                DeactivateSign();
            }
            LerpTextAlpha();
        }

        private void ActivateSign()
        {
            if (!woodenSignCanvas.gameObject.activeSelf)
            {
                woodenSignCanvas.gameObject.SetActive(true);
            }
            SetTargetAlpha(1);
        }

        private void DeactivateSign()
        {
            SetTargetAlpha(0);
            if (GetTextAlpha() <= 0.01f && woodenSignCanvas.gameObject.activeSelf)
            {
                woodenSignCanvas.gameObject.SetActive(false);
            }
        }

        private void LerpTextAlpha()
        {
            var currentColor = woodenSignText.color;
            woodenSignText.color = Color.Lerp(
                currentColor,
                new Color(currentColor.r, currentColor.g, currentColor.b, targetColor.a),
                Time.deltaTime * lerpSpeed
            );
        }

        private void SetTargetAlpha(float alpha)
        {
            targetColor = new Color(woodenSignText.color.r, woodenSignText.color.g, woodenSignText.color.b, alpha);
        }

        private float GetTextAlpha()
        {
            return woodenSignText.color.a;
        }

        private void SetTextAlpha(float alpha)
        {
            woodenSignText.color = new Color(woodenSignText.color.r, woodenSignText.color.g, woodenSignText.color.b, alpha);
        }

        private void ApplyTextEffects()
        {
            // Pulsating effect
            float scaleFactor = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
            woodenSignText.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
        }
    }
}
