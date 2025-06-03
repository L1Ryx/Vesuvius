using _ScriptableObjects;
using UnityEngine;

namespace _Gameplay._Arch
{
    public class Gate : MonoBehaviour
    {
        public enum GateHeight
        {
            Height2 = 2,
            Height4 = 4
        }

        [Header("Gate Settings")]
        public string gateID; // Unique identifier for this gate
        public GateHeight gateHeight = GateHeight.Height2; // Dropdown for height selection
        public float gateSlideDuration = 3f; // Total duration for gate to slide

        [Header("Sprites")]
        public SpriteRenderer gateSpriteRenderer;
        public Sprite spriteForHeight2;
        public Sprite spriteForHeight4;

        [Header("Gate State")]
        public GateData gateData;

        private Vector3 initialPosition;
        private float targetHeight;
        private GateAudio gateAudio;

        void Awake() {
            gateAudio = GetComponent<GateAudio>();
        }

        private void Start()
        {
            initialPosition = transform.position;

            if (gateData == null)
            {
                Debug.LogError($"GateData is not assigned to Gate {gateID}.");
                return;
            }

            UpdateGateSprite(); // Update sprite based on height
            SetTargetHeight();  // Set the height for movement logic

            bool isLocked = gateData.GetGateLockedState(gateID);
            if (isLocked)
            {
                Debug.Log($"Gate {gateID} is locked at start.");
            }
            else
            {
                Debug.Log($"Gate {gateID} is unlocked at start. Moving to open position.");
                MoveToOpenPosition();
            }
        }

        private void UpdateGateSprite()
        {
            if (gateSpriteRenderer == null)
            {
                Debug.LogWarning("Gate SpriteRenderer is not assigned.");
                return;
            }

            switch (gateHeight)
            {
                case GateHeight.Height2:
                    gateSpriteRenderer.sprite = spriteForHeight2;
                    break;
                case GateHeight.Height4:
                    gateSpriteRenderer.sprite = spriteForHeight4;
                    break;
                default:
                    Debug.LogWarning($"No sprite assigned for height {gateHeight}.");
                    break;
            }

            Debug.Log($"Gate {gateID} sprite updated for height {gateHeight}.");
        }

        private void SetTargetHeight()
        {
            targetHeight = (float)gateHeight; // Convert enum to float (2 or 4)
            Debug.Log($"Gate {gateID} target height set to {targetHeight}.");
        }

        private void MoveToOpenPosition()
        {
            transform.position = initialPosition + new Vector3(0, targetHeight, 0);
        }

        public void UnlockGate()
        {
            if (gateData.SetGateLockedState(gateID, false))
            {
                Debug.Log($"Gate {gateID} is now unlocked in GateData.");
                StartCoroutine(SlideGateUp());
                if (gateAudio != null) {
                    gateAudio.PlayGateOpen();
                }
            }
        }

        public float GateSlideDownDuration;
        //But don't lock it - used for cultist boss fight
        public void CloseGate()
        {
            StartCoroutine(SlideGateDown());
            if (gateAudio != null)
            {
                gateAudio.PlayGateOpen();
            }
        }

        private System.Collections.IEnumerator SlideGateUp()
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = initialPosition + new Vector3(0, targetHeight, 0);
            float elapsedTime = 0f;

            while (elapsedTime < gateSlideDuration)
            {
                elapsedTime += Time.deltaTime;
                float normalizedTime = Mathf.Clamp01(elapsedTime / gateSlideDuration);

                // Programmatic easing: start slow, accelerate, then decelerate
                float easedTime = EaseInOut(normalizedTime);

                // Interpolate position based on eased time
                transform.position = Vector3.Lerp(startPosition, targetPosition, easedTime);
                yield return null;
            }

            transform.position = targetPosition; // Ensure exact final position
            Debug.Log($"Gate {gateID} is now fully open.");
        }

        private System.Collections.IEnumerator SlideGateDown()
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition - new Vector3(0, targetHeight, 0);
            float elapsedTime = 0f;

            while (elapsedTime < GateSlideDownDuration)
            {
                elapsedTime += Time.deltaTime;
                float normalizedTime = Mathf.Clamp01(elapsedTime / GateSlideDownDuration);

                // Programmatic easing: start slow, accelerate, then decelerate
                float easedTime = EaseInOut(normalizedTime);

                // Interpolate position based on eased time
                transform.position = Vector3.Lerp(startPosition, targetPosition, easedTime);
                yield return null;
            }

            transform.position = targetPosition; // Ensure exact final position
            Debug.Log($"Gate {gateID} is now fully open.");
        }

        // Easing function: start slow, accelerate, decelerate
        private float EaseInOut(float t)
        {
            return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
        }
    }
}
