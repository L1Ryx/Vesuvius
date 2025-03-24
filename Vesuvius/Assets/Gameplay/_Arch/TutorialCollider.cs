using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay._Arch
{
    [RequireComponent(typeof(Collider2D))]
    public class TutorialCollider : MonoBehaviour
    {
        [Header("Available Tutorial Events")]
        [Tooltip("List of UnityEvents to be called when the player enters the trigger.")]
        public List<UnityEvent> tutorialEvents;

        [Header("Event Selection")]
        [Tooltip("Index of the event to invoke from the tutorialEvents list.")]
        public int selectedEventIndex = 0;

        private void Start()
        {
            // Ensure the Collider is set as a trigger
            Collider2D collider = GetComponent<Collider2D>();
            if (!collider.isTrigger)
            {
                Debug.LogWarning($"{gameObject.name} collider is not set as a trigger. Setting it now.");
                collider.isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) // Check if the collider is the player
            {
                if (tutorialEvents != null && selectedEventIndex >= 0 && selectedEventIndex < tutorialEvents.Count)
                {
                    tutorialEvents[selectedEventIndex]?.Invoke();
                    Debug.Log($"Tutorial event {selectedEventIndex} invoked.");
                }
                else
                {
                    Debug.LogWarning("Invalid event index or tutorial events not set.");
                }
            }
        }
    }
}
