using System.Collections;
using UnityEngine;

public class IceGroundSpikesBehavior : MonoBehaviour
{
    public float spikeSpeed = 3f;
    private Vector3 spikePos;
    Vector3 initalSpikesPos;

    void Start()
    {
        initalSpikesPos = this.gameObject.transform.position;
    }

    public IEnumerator SpikesEmerge()
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
        yield return new WaitForSeconds(.25f);
        //activate colliders
        Collider2D[] colChildren = this.gameObject.GetComponentsInChildren<Collider2D>();
        foreach (var collider in colChildren)
        {
            collider.enabled = true;
        }


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
        Collider2D[] colChildren = this.gameObject.GetComponentsInChildren<Collider2D>();
        foreach (var collider in colChildren) 
        {
            collider.enabled = false;
        }
    }
}
