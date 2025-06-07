using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using _Gameplay._Arch;
using UnityEngine;
using UnityEngine.UIElements;

public class EyeAttackBehavior : MonoBehaviour
{
    [SerializeField] GameObject eyeProjectilePrefab;
    [SerializeField] float fireRate = 1f;
    [SerializeField] float detectionRadius = 5f;
    public LayerMask terrainLayers;

    private GameObject player;

    private bool isPlayerNear = false;

    float timeSinceLastFire = 0f;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerSpawner>()?.GetRuntimePlayer();
        if (player == null)
        {
            Debug.LogError("Player not found! Ensure PlayerSpawner and runtime player setup is correct.");
            return;
        }
    }
    void Update()
    {
        timeSinceLastFire += Time.deltaTime;
        if (timeSinceLastFire >= fireRate)
        {
            HandleProximityLogic();
        }
    }

    private void HandleProximityLogic()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.GetComponent<Collider2D>().bounds.center, transform.position);
        isPlayerNear = distance <= detectionRadius;


        //Player must be nearby and not out of Line of sight
        if (isPlayerNear)
        {
            RaycastHit2D result = Physics2D.Linecast(transform.position, player.GetComponent<Collider2D>().bounds.center, terrainLayers);
            if (!result)
            {
                ShootProjectile();
            }
        }
    }

    private void ShootProjectile()
    {
        Instantiate(eyeProjectilePrefab, this.gameObject.GetComponent<Collider2D>().bounds.center, transform.rotation, transform);
        timeSinceLastFire = 0f;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

}
