using UnityEngine;
using UnityEngine.Playables;

public class StartCutsceneOnTriggerEnter : MonoBehaviour
{
    public PlayableDirector playableDirector;

    private void Start()
    {
        // Ensure the Collider is set as a trigger
        Collider2D collider = GetComponent<Collider2D>();
        if (!collider.isTrigger)
        {
            Debug.LogWarning($"{gameObject.name} collider is not set as a trigger. Setting it now.");
            collider.isTrigger = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Check if the collider is the player
        {
            playableDirector.Play();
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
