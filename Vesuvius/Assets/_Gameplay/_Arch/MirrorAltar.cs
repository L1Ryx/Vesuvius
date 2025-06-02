using _Gameplay._Arch;
using _ScriptableObjects.PlayerInfo;
using UnityEngine;

public class MirrorAltar : MonoBehaviour
{
    public BinaryStateStorage blockedInteractables;
    public GameObject mirrorSprite;
    public GameObject CheckpointCradle;

    [SerializeField] private PlayerInfo playerInfo;

    private GuidComponent guidComponent;

    private void Awake()
    {
        guidComponent = GetComponent<GuidComponent>();
    }

    void Start()
    {
        if (blockedInteractables.isInteractableBlocked(guidComponent.GetGuid().ToString()))
        {
            HideMirrorSprite();
        }
    }

    public void MirrorShardCollected()
    {
        playerInfo.SetCheckpoint(CheckpointCradle.GetComponent<EmptyCradle>().checkpointScene,
            CheckpointCradle.GetComponent<EmptyCradle>().checkpointLocation);
        HideMirrorSprite();
    }

    private void HideMirrorSprite()
    {
        mirrorSprite.SetActive(false);
    }
}
