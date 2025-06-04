using System;
using AK.Wwise;
using UnityEngine;
using UnityEngine.Events;

public class GauntletManager : MonoBehaviour
{
    public int stage = -1;
    [Header("Cultist")]
    public GameObject[] cultistTeleportPoints;
    public GameObject cultist;
    [Header("Ice Ground Spikes")]
    public UnityEvent triggerAttacks;
    [Header("Ice Ground Spikes")]
    public IceGroundSpikesBehavior spikesCollection;

    [Header("Completion")]
    public UnityEvent OnGauntletCompleted;

    private CultistBossBehavior cultistBehavior;

    void Awake()
    {
        cultistBehavior = cultist.GetComponent<CultistBossBehavior>();
    }

    void Start()
    {
        //StartCoroutine(cultistBehavior.TeleportTo(cultistTeleportPoints[stage].transform.position));
        //cultistBehavior.FaceRight();
        //TriggerAttacks();
    }

    public void TriggerAttacks()
    {
        switch (stage)
        {
            case 1:
                cultistBehavior.SlamUp();
                break;

            case 2:
                cultistBehavior.SlamDown();
                break;

            case 3:
                StartCoroutine(cultistBehavior.Beam());
                break;

            default:
                break;
        }
    }

    public void ResetStage()
    {
        stage = 1; //set to one to skip first part of stage after player death
    }

    public void AdvanceStage()
    {
        stage++;
        switch (stage)
        {
            case 0:
                StartCoroutine(cultistBehavior.TeleportTo(cultistTeleportPoints[stage].transform.position));
                cultistBehavior.FaceRight();
                break;

            case 1:
                StartCoroutine(cultistBehavior.TeleportTo(cultistTeleportPoints[stage].transform.position));
                cultistBehavior.FaceRight();
                TriggerAttacks();
                break;

            case 2:
                StartCoroutine(cultistBehavior.TeleportTo(cultistTeleportPoints[stage].transform.position));
                cultistBehavior.FaceLeft();
                break;

            case 3:
                StartCoroutine(cultistBehavior.TeleportTo(cultistTeleportPoints[stage].transform.position));
                cultistBehavior.FaceRight();
                break;

            case 4:
                CompleteGauntlet();
                break;

            default:
                break;
        }
    }

    private void CompleteGauntlet()
    {
        //change to teleport away
        cultist.SetActive(false);
        OnGauntletCompleted.Invoke();
    }
}
