using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public enum EnemyState { Idle, AttackPlayer, Special }
    public enum Specials { Slam, Beam }

    public EnemyState currentState = EnemyState.Idle;
    public Specials nextSpecial = Specials.Slam;

    [Header("Movement")]
    [SerializeField]
    float MoveSpeed = 2f;

    [SerializeField]
    float frequency = 20f;

    [SerializeField]
    float magnitude = 0.5f;
    [SerializeField]
    Vector3 idleTeleportPosition;

    [Header("Behavior")]
    [SerializeField]
    float timeBetweenSpecials = 5f;

    //[Header("Slam")]
    [SerializeField]
    GameObject spikes;
    [SerializeField]
    float slamSpeed = 2f;
    [SerializeField]
    float spikeSpeed = 3f;
    [SerializeField] Vector3 slamTeleportPosition;

    //[Header("Beam")]
    [SerializeField]
    Vector3 beamTeleportPosition;
    [SerializeField]
    GameObject preBeamBall;
    [SerializeField]
    GameObject BeamSprite;

    [Header("Summon Eye")]
    [SerializeField]
    float timeBeforeEyeSummon = 5f;
    [SerializeField]
    GameObject eye;

    [Header("Animation")]
    [SerializeField]
    Animator animator;
    [SerializeField]
    SpriteRenderer sprite;

    Vector3 pos;

    Vector3 spikePos;

    float accumulator = 0f;

    float timeSinceLastSpecial = 0f;

    float timeSinceEyeSummon = 0f;

    int direction = 1;

    bool isCastingSpecial = false;
    bool isSpecialDone = false;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = rb = GetComponent<Rigidbody2D>();
        pos = transform.position;
        if (!animator) print("animator not set");
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == EnemyState.Idle) { Idle(); }
        else if (currentState == EnemyState.Special) { Special(); }
    }

    void Idle()
    {
        timeSinceEyeSummon += Time.deltaTime;
        timeSinceLastSpecial += Time.deltaTime;
        Move();
        if(timeSinceEyeSummon >= timeBeforeEyeSummon)
        {
            SummonEye();
        }
        if (timeSinceLastSpecial >= timeBetweenSpecials)
        {
            currentState = EnemyState.Special;
        }
    }

    void SummonEye()
    {
        eye.SetActive(true);
        timeSinceEyeSummon = 0f;
    }

    void Special()
    {
        if (!isCastingSpecial)
        {
            isCastingSpecial = true;
            //StartCoroutine(Teleport());
            if (nextSpecial == Specials.Slam)
            {
                //animator.SetTrigger("Teleporting");
                //StartCoroutine(Slam());
                StartCoroutine(Teleport(slamTeleportPosition));
            }
            else if (nextSpecial == Specials.Beam)
            {
                //StartCoroutine(Beam());
                StartCoroutine(Teleport(beamTeleportPosition));
            }

        }
        //return boss back to idle state
        if (isSpecialDone)
        {
            isCastingSpecial = false;
            isSpecialDone = false;
            timeSinceLastSpecial = 0f;
            StartCoroutine(Teleport(idleTeleportPosition));
            //pos = idleTeleportPosition;
            //transform.position = pos;
            currentState = EnemyState.Idle;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.otherCollider.gameObject.tag);
        rb.linearVelocity = new Vector2(0, 0);
    }

    //called after teleport animation hides boss sprite to move them to teleported location
    void MoveBoss()
    {

    }

    IEnumerator Teleport(Vector3 location)
    {
        animator.SetTrigger("Teleporting");
        //animator.SetFloat("TeleportDirection", -1);
        float length = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds( length);
        //print("Test");
        pos = location;
        transform.position = pos;
        //yield return new WaitForSeconds(.25f);
        //sprite.enabled = true;
        animator.SetTrigger("TeleportExit");
        //sprite.enabled = true;
        yield return new WaitForSeconds( length);
        if (isCastingSpecial == true && nextSpecial == Specials.Slam)
        {
            //animator.SetTrigger("Teleporting");
            StartCoroutine(Slam());
        }
        else if (isCastingSpecial == true && nextSpecial == Specials.Beam)
        {
            StartCoroutine(Beam());
        }
    }

    IEnumerator Slam()
    {
        //print("slam");
        isCastingSpecial = true;
        animator.SetTrigger("QueueSpecial");
        spikes.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("Slamming");

        rb.linearVelocity = new Vector2(0, -slamSpeed);
        while (rb.linearVelocity.y == -slamSpeed)
        {
            //print(rb.linearVelocity);
            //print("slamming");
            //pos += -1 * transform.up * Time.deltaTime * slamSpeed;
            //transform.position = pos;
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(SpikesEmerge());
        animator.SetTrigger("DoneSlam");
        rb.linearVelocity = new Vector2(0, slamSpeed);
        while (transform.position.y <= slamTeleportPosition.y)
        {
            //print("recovering");
            //pos += transform.up * Time.deltaTime * slamSpeed;
            //transform.position = pos;
            yield return new WaitForEndOfFrame();
        }
        rb.linearVelocity = new Vector2(0, 0);
        isSpecialDone = true;
        nextSpecial = Specials.Beam;
    }

    IEnumerator SpikesEmerge()
    {
        Collider2D[] colChildren = spikes.GetComponentsInChildren<Collider2D>();
        foreach (var collider in colChildren) 
        {
            collider.enabled = true;
        }
        Vector3 initalSpikesPos = spikes.transform.position;
        spikePos = spikes.transform.position;
        //emergy spikes
        while (spikePos.y <= initalSpikesPos.y + 1)
        {
            spikePos = spikes.transform.position;
            spikePos += transform.up * Time.deltaTime * spikeSpeed;
            spikes.transform.position = spikePos;
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(SpikesSubmerge());
    }

    IEnumerator SpikesSubmerge()
    {
        Vector3 initalSpikesPos = spikes.transform.position;
        spikePos = spikes.transform.position;
        //emergy spikes
        while (spikePos.y >= initalSpikesPos.y - 1)
        {
            spikePos = spikes.transform.position;
            spikePos -= transform.up * Time.deltaTime * spikeSpeed;
            spikes.transform.position = spikePos;
            yield return new WaitForEndOfFrame();
        }
        Collider2D[] colChildren = spikes.GetComponentsInChildren<Collider2D>();
        foreach (var collider in colChildren) 
        {
            collider.enabled = false;
        }
        spikes.SetActive(false);
    }

    IEnumerator Beam()
    {
        print("beam");
        isCastingSpecial = true;
        //pos = beamTeleportPosition;
        //transform.position = pos;
        yield return new WaitForSeconds(.25f);
        preBeamBall.SetActive(true);
        yield return new WaitForSeconds(.5f);
        BeamSprite.SetActive(true);
        yield return new WaitForSeconds(1f);

        preBeamBall.SetActive(false);
        BeamSprite.SetActive(false);

        //recovering (float up and down for a few seconds)

        isSpecialDone = true;
        nextSpecial = Specials.Slam;
    }

    //-1 if left of origin, 1 if right.
    //will also need to flip sprite
    int FindOppositeSideOfPlayer()
    {
        return 1;
    }

    void Move()
    {
        accumulator += direction * Time.deltaTime;
        pos += direction * transform.right * Time.deltaTime * MoveSpeed;
        transform.position = pos + direction * transform.up * Mathf.Sin(accumulator * frequency) * magnitude;


        if (accumulator >= Mathf.PI || accumulator <= -Mathf.PI)
        {
            direction *= -1;
        }
    }
}
