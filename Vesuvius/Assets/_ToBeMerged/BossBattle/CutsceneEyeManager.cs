using UnityEngine;

public class CutsceneEyeManager : MonoBehaviour
{
    public GameObject mainEye;
    public GameObject[] otherEyes;
    public BinaryStateStorage blockedTriggers;
    private GuidComponent guidComponent;

    void Start()
    {
        guidComponent = GetComponent<GuidComponent>();
        foreach (GameObject eye in otherEyes)
        {
            eye.SetActive(false);
        }

        if (blockedTriggers.isInteractableBlocked(guidComponent.GetGuid().ToString()))
        {
            mainEye.SetActive(false);
        }   
    }

}
