using UnityEngine;

public class WatcherSprite : MonoBehaviour
{
    [Header("Refs")]
    public WatcherAI watcherAI;

    public void EndPokeFromSprite() {
        watcherAI.EndPoke();
    }
}
