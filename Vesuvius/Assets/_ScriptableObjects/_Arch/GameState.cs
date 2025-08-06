using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "Scriptable Objects/GameState")]
public class GameState : Saveable
{
    public bool isAltReality = false;

    public override void Reset()
    {
        isAltReality = false;
    }
}
