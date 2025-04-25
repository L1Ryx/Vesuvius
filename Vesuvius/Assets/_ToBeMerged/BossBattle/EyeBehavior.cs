using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EyeBehavior : MonoBehaviour
{
    [SerializeField] GameObject eyePrefab;
    [SerializeField] float fireRate = 1f;

    float timeSinceLastFire = 0f;

    void Update()
    {
        timeSinceLastFire += Time.deltaTime;
        if(timeSinceLastFire >= fireRate)
        {
            Instantiate(eyePrefab,transform.position,transform.rotation);
            timeSinceLastFire = 0f;
        }
    }

}
