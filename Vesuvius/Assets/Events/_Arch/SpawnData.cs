using UnityEngine;

namespace Events._Arch
{
    [CreateAssetMenu(fileName = "SpawnData", menuName = "Game/Spawn Data")]
    public class SpawnData : Saveable
    {
        public Vector2 spawnLocation;
        public bool isFacingLeft;
        public bool needsUpwardForce;

        public override void Reset()
        {
            //
        }
    }
}
