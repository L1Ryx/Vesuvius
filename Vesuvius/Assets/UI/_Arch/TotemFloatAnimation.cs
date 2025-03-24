using UnityEngine;

namespace UI._Arch
{
    public class TotemFloatAnimation : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float floatAmplitude = 3f; // Vertical movement range
        [SerializeField] private float floatSpeed = 1.5f; // Speed of vertical movement

        private Vector3 initialPosition;

        private void Start()
        {
            // Cache the initial position
            initialPosition = transform.localPosition;
        }

        private void Update()
        {
            // Vertical floating animation
            float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            transform.localPosition = initialPosition + new Vector3(0, yOffset, 0);
        }
    }
}
