using _Gameplay._Arch;
using UnityEngine;
using UnityEngine.WSA;

public class CollisionDamageIfSameReality : MonoBehaviour
{
    public bool isActive = false;
    public bool isInAltReality = false;
    public GameState gameState;
    public PlayerDamage playerDamage;

    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
    }

    public void StoreRealityState()
    {
        isInAltReality = gameState.isAltReality;
    }


    void OnTriggerStay2D(Collider2D collision)
    {
        if (isActive && collision.CompareTag("Player") && isInAltReality == gameState.isAltReality)
        {
            playerDamage.HandleCollisionWithHazard(this.gameObject.GetComponent<Collider2D>());
        }
    }
}
