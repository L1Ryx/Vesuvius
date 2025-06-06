using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using _ScriptableObjects.SceneAudio;

namespace Audio._Arch
{
    public class MusicPlayer : MonoBehaviour
    {
        public SceneAudioData sceneAudioData;
        public bool shouldPlayMusic = false;

        public static MusicPlayer Instance { get; private set; }

        // Tracks currently playing (base names only, e.g., "BohrmanTunnels")
        [Header("Debug View")] public List<string> activeTracks = new List<string>();

        [Header("Settings")]
        [Tooltip("Delay (in seconds) before triggering audio events after update")]
        public float eventDelay = 1f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                sceneAudioData.InitializeDictionary();
                UpdateMusicState();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            UpdateMusicState();
        }

        public void UpdateMusicState()
        {
            if (shouldPlayMusic)
            {
                StartCoroutine(DelayedMusicUpdate());
            }
        }

        public void EnableMusic()
        {
            shouldPlayMusic = true;
        }
        
        public void DisableMusic()
        {
            shouldPlayMusic = false;
        }

        private IEnumerator DelayedMusicUpdate()
        {
            yield return new WaitForSeconds(eventDelay);

            string sceneName = SceneManager.GetActiveScene().name;

            if (!sceneAudioData.sceneAudioDictionary.TryGetValue(sceneName, out AudioInfo audioInfo))
            {
                Debug.LogWarning($"No music track data found for scene: {sceneName}");
                yield break;
            }

            List<string> targetTracks = audioInfo.musicTracks;

            // Stop any tracks not in the new list
            for (int i = activeTracks.Count - 1; i >= 0; i--)
            {
                string oldTrack = activeTracks[i];
                if (!targetTracks.Contains(oldTrack))
                {
                    AkSoundEngine.PostEvent($"Stop_{oldTrack}", gameObject);
                    activeTracks.RemoveAt(i);
                    Debug.Log($"Stopped: {oldTrack}");
                }
            }

            // Start any new tracks not currently playing
            foreach (string newTrack in targetTracks)
            {
                if (!activeTracks.Contains(newTrack))
                {
                    AkSoundEngine.PostEvent($"Play_{newTrack}", gameObject);
                    activeTracks.Add(newTrack);
                    Debug.Log($"Playing: {newTrack}");
                }
            }
        }

        private void OnApplicationQuit()
        {
            StopAllTracks();
        }

        private void StopAllTracks()
        {
            foreach (string track in activeTracks)
            {
                AkSoundEngine.PostEvent($"Stop_{track}", gameObject);
            }

            activeTracks.Clear();
        }
    }
}
