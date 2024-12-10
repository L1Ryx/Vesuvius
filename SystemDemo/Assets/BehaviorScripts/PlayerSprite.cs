using UnityEngine;
using UnityEngine.Events;

public class PlayerSprite : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent playerStartedHeal;
    public UnityEvent playerHealed;

    public void PlayerStartedHeal() {
        playerStartedHeal.Invoke();
    }

    public void PlayerHealed() {
        playerHealed.Invoke();
    }
}
