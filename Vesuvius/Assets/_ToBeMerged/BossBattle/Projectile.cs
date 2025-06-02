using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject target;
    public float speed = 10f;
    public float rotationSpeed = 5f;
    private Rigidbody2D rb;
    //public float speed  = 2f;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player"); // Or however you identify the player
        rb = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            // Calculate direction and set velocity
            Vector2 direction = (target.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
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
