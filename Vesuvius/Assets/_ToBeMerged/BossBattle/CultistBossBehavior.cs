using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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

    bool isSlammingUpwards;
    public UnityEvent startSlamUp;
    public UnityEvent startSlamDown;
    public UnityEvent slamHitCeiling;
    public UnityEvent slamHitGround;
    public UnityEvent beamDoneAttack;
    public LayerMask terrainLayer;
    private Vector3 initialPosition;

    Rigidbody2D rb;
    private SpriteRenderer beamSpriteRenderer;
    private CollisionDamageIfSameReality beamDamage;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        beamSpriteRenderer = BeamSprite.GetComponent<SpriteRenderer>();
        beamDamage = BeamSprite.GetComponent<CollisionDamageIfSameReality>();
        pos = transform.position;
        if (!animator) print("animator not set");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SlamUp()
    {
        isSlammingUpwards = true;
        StartCoroutine(Slam());
    }

    public void SlamDown()
    {
        isSlammingUpwards = false;
        StartCoroutine(Slam());
    }

    public IEnumerator Slam()
    {
        initialPosition = transform.position;
        animator.SetTrigger("QueueSpecial");
        if (isSlammingUpwards)
        {
            startSlamUp.Invoke();
        }
        else
        {
            startSlamDown.Invoke();
        }  
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("Slamming");
        if (isSlammingUpwards)
        {
            rb.linearVelocity = new Vector2(0, slamSpeed);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, -slamSpeed);
        }    
    }

    //Used to determine when the slam has hit the ceiling or floor
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == Mathf.Log(terrainLayer.value, 2))
        {
            rb.linearVelocity = new Vector2(0, 0);
            if (isSlammingUpwards)
            {
                slamHitCeiling.Invoke();
            }
            else
            {
                slamHitGround.Invoke();
            }
            StartCoroutine(SlamRecover());
        }
    }

    public IEnumerator SlamRecover()
    {
        animator.SetTrigger("DoneSlam");
        //recovery from slamming up
        if (isSlammingUpwards)
        {
            rb.linearVelocity = new Vector2(0, -slamSpeed);
            while (transform.position.y > initialPosition.y)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, slamSpeed);
            while (transform.position.y < initialPosition.y)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        rb.linearVelocity = new Vector2(0, 0);
    }

    [Header("Beam Variables")]
    public float warningMinAlpha = 0.25f;
    public float warningMaxAlpha = 0.5f;
    public float warningTime = 1f;
    public float beamStayTime = 0.5f;
    public float beamFadeOutTime = 0.25f;
    private Color beamColor;

    public IEnumerator Beam()
    {
        print("beam");
        isCastingSpecial = true;
        //pos = beamTeleportPosition;
        //transform.position = pos;
        yield return new WaitForSeconds(.25f);
        preBeamBall.SetActive(true);
        beamDamage.StoreRealityState();
        yield return new WaitForSeconds(.5f);

        //set warning color
        beamColor = beamSpriteRenderer.color;
        beamColor.a = warningMinAlpha;
        beamSpriteRenderer.color = beamColor;
        BeamSprite.SetActive(true);
        float elapsedTime = 0f;
        // warning: Fade in Beam
        while (elapsedTime < warningTime / 2)
        {
            elapsedTime += Time.deltaTime;
            float alpha = (warningMaxAlpha - warningMinAlpha) * (elapsedTime / (warningTime / 2));

            beamColor = beamSpriteRenderer.color;
            beamColor.a = warningMinAlpha + alpha;
            beamSpriteRenderer.color = beamColor;
            yield return new WaitForEndOfFrame();
        }
        elapsedTime = 0f;
        //warning: Fade out Beam
        while (elapsedTime < warningTime / 2)
        {
            elapsedTime += Time.deltaTime;
            float alpha = (warningMaxAlpha - warningMinAlpha) * (elapsedTime / (warningTime / 2));

            beamColor = beamSpriteRenderer.color;
            beamColor.a = warningMaxAlpha - alpha;
            beamSpriteRenderer.color = beamColor;
            yield return new WaitForEndOfFrame();
        }

        //Beam Attack Active
        beamDamage.Activate();
        beamColor.a = 1f;
        beamSpriteRenderer.color = beamColor;
        yield return new WaitForSeconds(beamStayTime);

        //FadeOutBeam
        beamDamage.Deactivate();
        elapsedTime = 0f;
        while (elapsedTime < beamFadeOutTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = elapsedTime / beamFadeOutTime;

            beamColor = beamSpriteRenderer.color;
            beamColor.a = 1 - alpha;
            beamSpriteRenderer.color = beamColor;
            yield return new WaitForEndOfFrame();
        }


        preBeamBall.SetActive(false);
        BeamSprite.SetActive(false);
        

        //recovering (float up and down for a few seconds)

        isSpecialDone = true;
        nextSpecial = Specials.Slam;
        beamDoneAttack.Invoke();
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
