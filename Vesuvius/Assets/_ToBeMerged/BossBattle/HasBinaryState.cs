using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(GuidComponent))]
public class HasBinaryState : MonoBehaviour
{
    public BinaryStateStorage stateStorage;
    public bool callEventsIfInStorage = false;
    public UnityEvent eventsToCallOnStart;
    private GuidComponent guidComponent;

    void Awake()
    {
        guidComponent = GetComponent<GuidComponent>();
    }

    void Start()
    {
        if (isInStorageState())
        {
            eventsToCallOnStart.Invoke();
        }
    }

    public void AddToStorage()
    {
        stateStorage.Add(guidComponent.GetGuid().ToString());
    }

    public bool isInStorageState()
    {
        return stateStorage.isInteractableBlocked(guidComponent.GetGuid().ToString());
    }
}
