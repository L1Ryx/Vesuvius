using System.Collections;
using UnityEngine;

public class IceGroundSpikesBehavior : MonoBehaviour
{
    public float spikeSpeed = 3f;
    private Vector3 spikePos;
    Vector3 initalSpikesPos;
    private Collider2D hazardCollider;

    void Awake()
    {
        hazardCollider = GetComponentInChildren<Collider2D>();
    }

    void Start()
    {
        initalSpikesPos = this.gameObject.transform.position;
    }

    public void ShowSpikes()
    {
        StartCoroutine(SpikesEmerge());
    }

    IEnumerator SpikesEmerge()
    {
        //Push spikes out a little bit without colliders as a warning
        spikePos = this.gameObject.transform.position;
        while (spikePos.y <= initalSpikesPos.y + .25)
        {
            spikePos = this.gameObject.transform.position;
            spikePos += transform.up * Time.deltaTime * spikeSpeed;
            this.gameObject.transform.position = spikePos;
            yield return new WaitForEndOfFrame();
        }
    }

    public void AttackSpikes()
    {
        StartCoroutine(SpikesAttack());
    }

    IEnumerator SpikesAttack()
    {
        //activate colliders
        hazardCollider.enabled = true;
        
        //emerge spikes fully
        while (spikePos.y <= initalSpikesPos.y + 1.25)
        {
            spikePos = this.gameObject.transform.position;
            spikePos += transform.up * Time.deltaTime * spikeSpeed;
            this.gameObject.transform.position = spikePos;
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(SpikesSubmerge());
    }

    IEnumerator SpikesSubmerge()
    {
        spikePos = this.gameObject.transform.position;
        //submerge spikes back underground
        while (spikePos.y >= initalSpikesPos.y - 1)
        {
            spikePos = this.gameObject.transform.position;
            spikePos -= transform.up * Time.deltaTime * spikeSpeed;
            this.gameObject.transform.position = spikePos;
            yield return new WaitForEndOfFrame();
        }
        hazardCollider.enabled = false;
    }
}
