using UnityEngine;

[RequireComponent(typeof(GuidComponent))]
public class DisableIfInBinaryStateStorage : MonoBehaviour
{
    public BinaryStateStorage stateStorage;
    private GuidComponent guidComponent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        guidComponent = GetComponent<GuidComponent>();
        if (stateStorage.isInteractableBlocked(guidComponent.GetGuid().ToString()))
        {
            this.gameObject.SetActive(false);
        }
    }

    public void AddToStorage()
    {
        stateStorage.Add(guidComponent.GetGuid().ToString());
    }
}
