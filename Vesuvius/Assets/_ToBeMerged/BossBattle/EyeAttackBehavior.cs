using System.Collections;
using System.Collections.Generic;
using _Gameplay._Arch;
using UnityEngine;
using UnityEngine.UIElements;

public class EyeAttackBehavior : MonoBehaviour
{
    [SerializeField] GameObject eyeProjectilePrefab;
    [SerializeField] float fireRate = 1f;
    [SerializeField] float detectionRadius = 5f;

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

        float distance = Vector3.Distance(player.transform.position, transform.position);
        isPlayerNear = distance <= detectionRadius;

        if (isPlayerNear)
        {
            ShootProjectile();
        }
    }

    private void ShootProjectile()
    {
        Instantiate(eyeProjectilePrefab, transform.position, transform.rotation);
        timeSinceLastFire = 0f;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

}
