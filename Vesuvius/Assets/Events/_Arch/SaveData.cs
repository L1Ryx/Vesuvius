using UnityEngine;
using UnityEngine.SceneManagement;

namespace Events._Arch
{
    [CreateAssetMenu]
    public class SaveData : ScriptableObject
    {
        public Transform playerTransform;
        public Scene playerScene;
    }
}
