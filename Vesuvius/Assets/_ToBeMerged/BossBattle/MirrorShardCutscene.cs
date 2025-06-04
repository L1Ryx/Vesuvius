using UnityEngine;

public class MirrorShardCutscene : MonoBehaviour
{
    public CultistBossBehavior cultist;
    public GameObject cultistTeleportPoint;

    public void TeleportIntoPosition()
    {
        StartCoroutine(cultist.TeleportTo(cultistTeleportPoint.transform.position));
    }
}
