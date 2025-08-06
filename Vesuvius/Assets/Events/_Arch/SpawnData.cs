using UnityEngine;

namespace Events._Arch
{
    [CreateAssetMenu(fileName = "SpawnData", menuName = "Game/Spawn Data")]
    public class SpawnData : ScriptableObject
    {
        public Vector2 spawnLocation;
        public bool isFacingLeft;
        public bool needsUpwardForce;
    }
}
