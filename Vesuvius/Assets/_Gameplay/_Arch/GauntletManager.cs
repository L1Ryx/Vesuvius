using System;
using UnityEngine;
using UnityEngine.Events;

public class GauntletManager : MonoBehaviour
{
    public int stage = 0;
    [Header("Cultist")]
    public GameObject[] cultistTeleportPoints;
    public GameObject cultist;
    public float timeBetweenAttacks = 2f;
    [Header("Ice Ground Spikes")]
    public UnityEvent triggerAttacks;
    [Header("Ice Ground Spikes")]
    public IceGroundSpikesBehavior spikesCollection;

    [Header("Completion")]
    public UnityEvent OnGauntletCompleted;

    private CultistBossBehavior cultistBehavior;
    private float timeSinceLastAttack = 0f;

    void Awake()
    {
        cultistBehavior = cultist.GetComponent<CultistBossBehavior>();
    }

    void Start()
    {
        StartCoroutine(cultistBehavior.TeleportTo(cultistTeleportPoints[stage].transform.position));
        cultistBehavior.FaceRight();
    }

    void Update()
    {
        timeSinceLastAttack += Time.deltaTime;
        if (timeSinceLastAttack >= timeBetweenAttacks)
        {
            TriggerAttacks();
        }
    }

    private void TriggerAttacks()
    {
        triggerAttacks.Invoke();
        StartCoroutine(spikesCollection.SpikesEmerge());
        if (stage == 3)
        {
            StartCoroutine(cultistBehavior.Beam());
        }
        timeSinceLastAttack = 0f;
    }

    public void ResetStage()
    {
        stage = 1; //set to one to skip first part of stage after player death
    }

    public void AdvanceStage()
    {
        stage++;
        //final stage
        if (stage == 4)
        {
            CompleteGauntlet();
        }
        else if (stage == 2)
        {
            StartCoroutine(cultistBehavior.TeleportTo(cultistTeleportPoints[stage].transform.position));
            cultistBehavior.FaceRight();
        }
        else
        {
            StartCoroutine(cultistBehavior.TeleportTo(cultistTeleportPoints[stage].transform.position));
            cultistBehavior.FaceLeft();
        }
    }

    private void CompleteGauntlet()
    {
        //change to teleport away
        cultist.SetActive(false);
        OnGauntletCompleted.Invoke();
    }
}
