using UnityEngine;

[CreateAssetMenu(fileName = "PlayerUnlocks", menuName = "Scriptable Objects/PlayerUnlocks")]
public class PlayerUnlocks : ScriptableObject
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
}
