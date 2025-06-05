using _Gameplay._Arch;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(GuidComponent))]
public class InvokeUnityEventsOnTriggerEnter : MonoBehaviour
{
    public UnityEvent OnTriggerEnter; //actions to execute on valid interaction
    public BinaryStateStorage blockedTriggers;
    private GuidComponent guidComponent;
    private Collider2D triggerCollider;
    private bool triggered = false;

    private void Start()
    {
        // Ensure the Collider is set as a trigger
        triggerCollider = GetComponent<Collider2D>();
        guidComponent = GetComponent<GuidComponent>();
        if (!triggerCollider.isTrigger)
        {
            Debug.LogWarning($"{gameObject.name} collider is not set as a trigger. Setting it now.");
            triggerCollider.isTrigger = true;
        }
        if (blockedTriggers != null && blockedTriggers.isInteractableBlocked(guidComponent.GetGuid().ToString()))
        {
            triggerCollider.enabled = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !triggered) // Check if the collider is the player
        {
            triggered = true;
            OnTriggerEnter.Invoke();
            triggerCollider.enabled = false;
        }
    }

    public void AddTriggerToBlockedList()
    {
        blockedTriggers.Add(guidComponent.GetGuid().ToString());
    }
}
