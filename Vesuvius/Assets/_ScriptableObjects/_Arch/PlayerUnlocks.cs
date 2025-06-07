using UnityEngine;

[CreateAssetMenu(fileName = "PlayerUnlocks", menuName = "Scriptable Objects/PlayerUnlocks")]
public class PlayerUnlocks : ScriptableObject, Saveable
{
    public bool canDoubleJump;
    public bool canRealityShift;

    public void UnlockDoubleJump()
    {
        canDoubleJump = true;
    }

    public void UnlockRealityShift()
    {
        canRealityShift = true;
    }

    public void Reset()
    {
        canDoubleJump = false;
        canRealityShift = false;
    }
}

public interface Saveable
{
    public void Reset();
}