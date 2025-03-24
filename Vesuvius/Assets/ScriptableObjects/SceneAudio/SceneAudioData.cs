using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.SceneAudio
{
    [System.Serializable]
    public class AudioInfo {
        [Range(0, 100)]
        public float Windiness;

        [Range(0, 100)]
        public float Openness;
        [Range(0, 100)]
        public float Creakiness;
    }

    [CreateAssetMenu(fileName = "SceneAudioData", menuName = "Audio/Scene Audio Data")]
    public class SceneAudioData : ScriptableObject
    {
        // A dictionary mapping scene names to their audio information
        public Dictionary<string, AudioInfo> sceneAudioDictionary = new Dictionary<string, AudioInfo>();

        // For Unity Inspector support (Dictionary is not natively serialized)
        [System.Serializable]
        public class SceneAudioEntry
        {
            public string sceneName; // Name of the scene
            public AudioInfo audioInfo; // Audio settings for this scene
        }

        // Unity-friendly list for editing in Inspector
        public List<SceneAudioEntry> sceneAudioEntries = new List<SceneAudioEntry>();

        // Populate the dictionary from the list (called on load or via editor button)
        public void InitializeDictionary()
        {
            sceneAudioDictionary.Clear();
            foreach (var entry in sceneAudioEntries)
            {
                if (!sceneAudioDictionary.ContainsKey(entry.sceneName))
                {
                    sceneAudioDictionary.Add(entry.sceneName, entry.audioInfo);
                }
            }
        }
    }
}