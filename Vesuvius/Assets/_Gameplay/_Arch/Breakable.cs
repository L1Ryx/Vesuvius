using UnityEngine;
using UnityEngine.Events;

public class Breakable : MonoBehaviour
{
    public UnityEvent broken;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 12)
        {
            broken.Invoke();
        }
    }
}
