using _Gameplay._Arch;
using _ScriptableObjects.PlayerInfo;
using UnityEngine;

//Call from unity event
public class UpdatePlayerCheckpointData : MonoBehaviour
{
    public EmptyCradle CheckpointCradle;
    [SerializeField] private PlayerInfo playerInfo;

    public void UpdatePlayerCheckpoint()
    {
        playerInfo.SetCheckpoint(CheckpointCradle.checkpointScene,
            CheckpointCradle.checkpointLocation);
    }
}
