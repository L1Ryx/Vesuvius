using System.Collections;
using _Gameplay._Arch;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class CultistBossBehavior : MonoBehaviour
{
    public enum EnemyState { Idle, AttackPlayer, Special, NotEngaged, Teleporting, Dead }
    public enum Specials { SlamUp, Beam , SlamDown}

    public EnemyState currentState = EnemyState.NotEngaged;
    public Specials nextSpecial = Specials.SlamUp;

    [Header("Movement")]
    [SerializeField]
    float MoveSpeed = 2f;

    [SerializeField]
    float frequency = 20f;

    [SerializeField]
    float magnitude = 0.5f;
    [SerializeField]
    GameObject idleTeleportPosition;

    [Header("Behavior")]
    [SerializeField]
    float timeBetweenSpecials = 5f;

    [SerializeField]
    float slamSpeed = 2f;
    [SerializeField] GameObject slamTeleportPosition;

    //[Header("Beam")]
    [SerializeField]
    GameObject beamTeleportPosition;
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
    private Coroutine currentAttack;

    Rigidbody2D rb;
    private SpriteRenderer beamSpriteRenderer;
    private SpriteRenderer ballSpriteRenderer;
    private CollisionDamageIfSameReality beamDamage;

    public UnityEvent doneSlamRecovery;
    public UnityEvent teleportDone;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        beamSpriteRenderer = BeamSprite.GetComponent<SpriteRenderer>();
        ballSpriteRenderer = preBeamBall.GetComponent<SpriteRenderer>();
        beamDamage = BeamSprite.GetComponent<CollisionDamageIfSameReality>();
        pos = transform.position;
        if (!animator) print("animator not set");
    }
    #region Controller
    // Update is called once per frame
    void Update()
    {
        if (currentState == EnemyState.Idle) { Idle(); }
        else if (currentState == EnemyState.Special) { Special(); }
    }

    #endregion

    public void BeginFight()
    {
        currentState = EnemyState.Idle;
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
    public float amplitude = 5f;      // Size of the figure 8
    public float speed = 1f;          // Speed of movement
    public Vector3 center; // Center position of the figure 8
    private float timeCounter;
    void Move()
    {
        center = idleTeleportPosition.transform.position;
        //accumulator += direction * Time.deltaTime;
        //pos += direction * transform.right * Time.deltaTime * MoveSpeed;
        //transform.position = pos + direction * transform.up * Mathf.Sin(accumulator * frequency) * magnitude;


        //if (accumulator >= Mathf.PI || accumulator <= -Mathf.PI)
        //{
        //    direction *= -1;
        //}

        timeCounter += Time.deltaTime * speed;

        // Lissajous figure-8 pattern in XY
        float x = amplitude * Mathf.Sin(timeCounter);
        float y = amplitude * Mathf.Sin(timeCounter) * Mathf.Cos(timeCounter);

        transform.position = center + new Vector3(x, y, 0f);
    }

    void SummonEye()
    {
        //eye.SetActive(true);
        timeSinceEyeSummon = 0f;
    }

    void Special()
    {
        if (!isCastingSpecial && !isSpecialDone)
        {
            isCastingSpecial = true;
            //StartCoroutine(Teleport());
            if (nextSpecial == Specials.SlamUp || nextSpecial == Specials.SlamDown)
            {
                //animator.SetTrigger("Teleporting");
                //StartCoroutine(Slam());
                StartCoroutine(Teleport(slamTeleportPosition.transform.position));
            }
            else if (nextSpecial == Specials.Beam)
            {
                //StartCoroutine(Beam());
                StartCoroutine(Teleport(beamTeleportPosition.transform.position));
            }

        }
        //return boss back to idle state
        if (isSpecialDone)
        {
            isCastingSpecial = false;
            isSpecialDone = false;
            timeSinceLastSpecial = 0f;
            currentState = EnemyState.Teleporting;
            StartCoroutine(Teleport(idleTeleportPosition.transform.position));
        }
    }

    public void SlamUp()
    {
        isSlammingUpwards = true;
        currentAttack = StartCoroutine(Slam());
    }

    public void SlamDown()
    {
        isSlammingUpwards = false;
        currentAttack = StartCoroutine(Slam());
    }
    #region Behaviors

    IEnumerator Teleport(Vector3 location)
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
        //sprite.enabled = true;
        yield return new WaitForSeconds(length);
        if (isCastingSpecial == true && nextSpecial == Specials.SlamUp)
        {
            SlamUp();
        }
        else if (isCastingSpecial == true && nextSpecial == Specials.SlamDown)
        {
            SlamDown();
        }
        else if (isCastingSpecial == true && nextSpecial == Specials.Beam)
        {
            FaceLeft();
            currentAttack = StartCoroutine(Beam());
        }
        else
        {
            currentState = EnemyState.Idle;
            timeCounter = 0f;
        }
    }
    public IEnumerator Slam()
    {
        initialPosition = transform.position;
        if (isSlammingUpwards)
        {
            startSlamUp.Invoke();
        }
        else
        {
            startSlamDown.Invoke();
        }
        yield return new WaitForSeconds(0.5f);
        if (isSlammingUpwards)
        {
            animator.SetTrigger("SlammingUp");
            rb.linearVelocity = new Vector2(0, slamSpeed);
        }
        else
        {
            animator.SetTrigger("SlammingDown");
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
        //wait at bottom of recovery for a second
        animator.SetTrigger("RecoveryDone");
        yield return new WaitForSeconds(1f);
        isSpecialDone = true;
        nextSpecial = Specials.Beam;
        doneSlamRecovery.Invoke();
    }

    [Header("Beam Variables")]
    public float warningMinAlpha = 0.25f;
    public float warningMaxAlpha = 0.5f;
    public float warningTime = 1f;
    public float beamStayTime = 0.5f;
    public float beamFadeOutTime = 0.25f;
    private Color beamColor;
    private Color ballColor;

    public void CastBeam()
    {
        if (!isCastingSpecial)
            currentAttack = StartCoroutine(Beam());
    }

    public IEnumerator Beam()
    {
        print("beam");
        animator.SetTrigger("ChargeBeam");
        isCastingSpecial = true;
        float elapsedTime = 0f;
        yield return new WaitForSeconds(.25f);
        preBeamBall.SetActive(true);
        beamDamage.StoreRealityState();
        ballColor = ballSpriteRenderer.color; 
        //yield return new WaitForSeconds(.5f);
        while (elapsedTime < .5f)
        {
            elapsedTime += Time.deltaTime;
            float alpha = elapsedTime / .5f;

            ballColor = ballSpriteRenderer.color;
            ballColor.a = 1 - alpha;
            ballSpriteRenderer.color = ballColor;
            yield return new WaitForEndOfFrame();
        }

        preBeamBall.SetActive(false);
        //set warning color
        beamColor = beamSpriteRenderer.color;
        beamColor.a = warningMinAlpha;
        beamSpriteRenderer.color = beamColor;
        BeamSprite.SetActive(true);
        elapsedTime = 0f;
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
        animator.SetTrigger("UnleashBeam");
        beamDamage.Activate();
        beamColor.a = 1f;
        beamSpriteRenderer.color = beamColor;
        elapsedTime = 0f;
        BeamSprite.transform.localScale = Vector2.zero;
        while (elapsedTime < 2f)
        {
            elapsedTime += Time.deltaTime;
            float alpha = elapsedTime / 2f;

            Vector2 scale = new Vector2(alpha, alpha);
            BeamSprite.transform.localScale = scale;
            yield return new WaitForEndOfFrame();
        }

        //yield return new WaitForSeconds(beamStayTime);

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
        animator.SetTrigger("RecoveryDone");
        yield return new WaitForSeconds(2f);

        isSpecialDone = true;
        isCastingSpecial = false;
        if (isSlammingUpwards)
        {
            nextSpecial = Specials.SlamDown;
        }
        else
        {
            nextSpecial = Specials.SlamUp;
        }
        
        beamDoneAttack.Invoke();
    }

    public IEnumerator TeleportTo(Vector3 location)
    {
        if(currentAttack != null)
            StopCoroutine(currentAttack);
        animator.SetTrigger("Teleporting");
        float length = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(length);
        pos = location;
        transform.position = pos;
        animator.SetTrigger("TeleportExit");
        yield return new WaitForSeconds(length);
        teleportDone.Invoke();
    }

    public void FaceRight()
    {
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    public void FaceLeft()
    {
        transform.rotation = Quaternion.Euler(0f, 0, 0f);
    }

    public Dialogue dialogue;

    public void Death()
    {
        if (currentAttack != null)
        {
            StopCoroutine(currentAttack);
        }
        animator.SetTrigger("Death");
        currentState = EnemyState.Dead;
        //fall to the ground
        rb.gravityScale = 1;
        dialogue.StartDialogue();
    }
    #endregion
}
