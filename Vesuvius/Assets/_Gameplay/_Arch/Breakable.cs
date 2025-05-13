using UnityEngine;

public class Breakable : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 12)
        {
            print("colliding");
            this.transform.parent.gameObject.SetActive(false);
        }
    }
}
