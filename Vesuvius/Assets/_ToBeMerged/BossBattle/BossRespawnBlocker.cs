using UnityEngine;

[RequireComponent(typeof(GuidComponent))]

public class BossRespawnBlocker : MonoBehaviour
{
    public BinaryStateStorage defeatedBosses;
    private GuidComponent guidComponent;

    private void Start()
    {
        guidComponent = GetComponent<GuidComponent>();
        if (defeatedBosses.isInteractableBlocked(guidComponent.GetGuid().ToString()))
        {
            this.gameObject.SetActive(false);
        }
    }

    public void AddBossToBlockedList()
    {
        defeatedBosses.Add(guidComponent.GetGuid().ToString());
    }
}
