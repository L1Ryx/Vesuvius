using UnityEngine;

[CreateAssetMenu(fileName = "PlayerUnlocks", menuName = "Scriptable Objects/PlayerUnlocks")]
public class PlayerUnlocks : Saveable
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

    public override void Reset()
    {
        canDoubleJump = false;
        canRealityShift = false;
    }
}

public abstract class Saveable : ScriptableObject
{
    public abstract void Reset();
}