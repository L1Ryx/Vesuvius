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
    public SpriteRenderer BeamSprite;

    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
    }

    public void ResetBetweenAttacks()
    {
        grayscaleAmount = 0;
        BeamSprite.sortingOrder = 2;
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
            StartCoroutine(SafeFromBeam());
        }
        else
        {
            StartCoroutine(ReverseSafe());
        }
    }

    IEnumerator SafeFromBeam()
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
        BeamSprite.sortingOrder = 0;
    }

    IEnumerator ReverseSafe()
    {
        float elapsedTime = 0f;
        //yield return new WaitForSeconds(.5f);
        while (elapsedTime < .5f)
        {
            elapsedTime += Time.deltaTime;
            grayscaleAmount = 1 - elapsedTime / .5f;

            material.SetFloat("_GrayscaleAmount", grayscaleAmount);
            yield return new WaitForEndOfFrame();
        }
        BeamSprite.sortingOrder = 2;
    }


    void OnTriggerStay2D(Collider2D collision)
    {
        if (isActive && collision.CompareTag("Player") && isInAltReality == gameState.isAltReality)
        {
            playerDamage.HandleCollisionWithHazard(this.gameObject.GetComponent<Collider2D>());
        }
    }
}
