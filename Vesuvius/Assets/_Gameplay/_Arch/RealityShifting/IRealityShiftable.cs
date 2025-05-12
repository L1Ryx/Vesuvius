using UnityEngine;

public interface IRealityShiftable
{
    public void RealityShiftCrossFade(bool isAltReality,float crossfadeDuration);
    public void RealityShiftInstantly(bool isAltReality);
}
