using UnityEngine;
using UnityEngine.AI;

public class Animator_RealityShift : MonoBehaviour, IRealityShiftable
{
    public RuntimeAnimatorController NormalRealityController;
    public RuntimeAnimatorController AlteredRealityController;
    public Animator animator;
    public void RealityShiftCrossFade(bool isAltReality, float crossfadeDuration)
    {
        RealityShiftInstantly(isAltReality);
    }

    public void RealityShiftInstantly(bool isAltReality)
    {
        if (isAltReality)
        {
            animator.runtimeAnimatorController = AlteredRealityController;
        }
        else
        {
            animator.runtimeAnimatorController = NormalRealityController;
        }
    }
}
