using System.Collections;
using UnityEngine;

public class CultistBossBehavior : MonoBehaviour
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

    }

    public IEnumerator Beam()
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

    public IEnumerator TeleportTo(Vector3 location)
    {
        animator.SetTrigger("Teleporting");
        //animator.SetFloat("TeleportDirection", -1);
        float length = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(length);
        //print("Test");
        pos = location;
        transform.position = pos;
        //yield return new WaitForSeconds(.25f);
        //sprite.enabled = true;
        animator.SetTrigger("TeleportExit");
        yield return new WaitForSeconds(length);
    }

    public void FaceRight()
    {
        transform.Rotate(0, 180, 0);
    }

    public void FaceLeft()
    {
        transform.Rotate(0, -180, 0);
    }
}
