using System.Collections;
using System.Collections.Generic;
using _Gameplay._Arch;
using UnityEngine;
using UnityEngine.Events;

public class EyeProjectile : MonoBehaviour
{
    public GameObject target;
    public float speed = 10f;
    public float rotationSpeed = 5f;
    public float timeToHome = 2f;
    private Rigidbody2D rb;
    private Vector2 direction;
    private float timeAlive = 0f;

    public UnityEvent projectileDeath;


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
            direction = (target.GetComponent<Collider2D>().bounds.center - transform.position).normalized;
        }


        rb.linearVelocity = direction * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //if you collide with enemyhealth (which can only happen after volley, damage it)
        if (collision.gameObject.TryGetComponent(out EnemyHealth enemyHealth))
        {
            enemyHealth.Damage(20, Vector2.zero);
        }
        projectileDeath.Invoke();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //player turns it back to the enemy
        if (other.gameObject.layer == 12)
        {
            target = transform.parent.gameObject;
            timeAlive = 0f;
            this.gameObject.layer = 12;
        }
        if (other.CompareTag("Player"))
        {
            projectileDeath.Invoke();
        }
    }

    public void DestroyProjectile()
    {
        Destroy(this.gameObject);
    }
    // //When projectile goes offscreen destroy it
    // void OnBecameInvisible() 
    // {
    //     Destroy(this.gameObject);
    // }
}
