using UnityEngine;

public class CutsceneEyeManager : MonoBehaviour
{
    public GameObject mainEye;
    public GameObject[] otherEyes;
    public BinaryStateStorage blockedTriggers;
    private GuidComponent guidComponent;

    void Awake()
    {
        guidComponent = GetComponent<GuidComponent>();
    }

    void Start()
    {
        foreach (GameObject eye in otherEyes)
        {
            eye.SetActive(false);
        }

        if (blockedTriggers.isInteractableBlocked(guidComponent.GetGuid().ToString()))
        {
            mainEye.SetActive(false);
        }
    }

    public void AddToStorage()
    {
        blockedTriggers.Add(guidComponent.GetGuid().ToString());
    }

    public bool isBlocked()
    {
        return blockedTriggers.isInteractableBlocked(guidComponent.GetGuid().ToString());
    }

}
