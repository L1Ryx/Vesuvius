using System.Collections;
using _Gameplay._Arch;
using UnityEngine;

public class CollisionDamageIfSameReality : MonoBehaviour
{
    public bool isActive = false;
    public bool isInAltReality = false;
    public GameState gameState;
    public PlayerDamage playerDamage;
    public Material material;
    [Range(0, 1)] public float grayscaleAmount = 0;

    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
        grayscaleAmount = 0;
        material.SetFloat("_GrayscaleAmount", grayscaleAmount);
    }

    void Start()
    {
        material.SetFloat("_GrayscaleAmount", grayscaleAmount);
    }

    public void StoreRealityState()
    {
        isInAltReality = gameState.isAltReality;
    }

    public void PlayerRealityShifted()
    {
        if (isInAltReality != gameState.isAltReality)
        {
            StartCoroutine(Beam());

        }
        else
        {
            
        }
    }

    IEnumerator Beam()
    {
        float elapsedTime = 0f;
        //yield return new WaitForSeconds(.5f);
        while (elapsedTime < .5f)
        {
            elapsedTime += Time.deltaTime;
            grayscaleAmount = elapsedTime / .5f;

            material.SetFloat("_GrayscaleAmount", grayscaleAmount);
            yield return new WaitForEndOfFrame();
        }
    }


    void OnTriggerStay2D(Collider2D collision)
    {
        if (isActive && collision.CompareTag("Player") && isInAltReality == gameState.isAltReality)
        {
            playerDamage.HandleCollisionWithHazard(this.gameObject.GetComponent<Collider2D>());
        }
    }
}
