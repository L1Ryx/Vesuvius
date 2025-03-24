using UnityEngine;

// TODO: DECOUPLE FROM CURRENT SCENE
namespace Events._Arch
{
    [CreateAssetMenu(fileName = "TransitionData", menuName = "Scene Management/Transition Data")]
    public class TransitionData : ScriptableObject {
        public string sceneToLoad;
        public Vector2 spawnPosition;
    
    }
}
