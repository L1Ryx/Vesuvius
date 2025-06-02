using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeProjectile : MonoBehaviour
{
    public GameObject target;
    public float speed = 10f;
    public float rotationSpeed = 5f;
    public float timeToHome = 2f;
    private Rigidbody2D rb;
    private Vector2 direction;
    private float timeAlive = 0f;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player"); // Or however you identify the player
        rb = GetComponent<Rigidbody2D>();
        direction = (target.transform.position - transform.position).normalized;
    }
    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;
        if (target != null && timeAlive <= timeToHome)
        {
            // Calculate direction and set velocity
            direction = (target.transform.position - transform.position).normalized;
        }

        rb.linearVelocity = direction * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);
    }
    // //When projectile goes offscreen destroy it
    // void OnBecameInvisible() 
    // {
    //     Destroy(this.gameObject);
    // }
}
