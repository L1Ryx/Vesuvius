using _ScriptableObjects;
using _ScriptableObjects.PlayerInfo;
using Events._Arch;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Gameplay._Arch
{
    public class ScriptableObjectManager : MonoBehaviour
    {
        [Header("ScriptableObjects to Save/Load")]
        public GateData gateData;
        public BulbData bulbData;
        public PlayerInfo playerInfo;
        public TutorialData tutorialData;
        public SpawnData spawnData;

        private static ScriptableObjectManager instance;
        private PlayerControls playerControls;

        private const string SaveFileName = "ScriptableObjects.es3";

        private void Awake()
        {
            // Check if an instance already exists
            if (instance != null && instance != this)
            {
                Debug.LogWarning("Another instance of ScriptableObjectManager already exists. Destroying this instance.");
                Destroy(gameObject);
                return;
            }

            // Set the instance and make it persistent
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize PlayerControls
            InitializeControls();
        }

        private void InitializeControls()
        {
            playerControls = new PlayerControls();

            // Subscribe to the Quit action
            playerControls.Player.Quit.performed += OnQuit;

            // Enable controls
            playerControls.Enable();
        }

        private void OnQuit(InputAction.CallbackContext context)
        {
            Debug.Log("Quit action triggered.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the editor
#else
        Application.Quit(); // Quit the application in a build
#endif
        }

        private void OnDestroy()
        {
            // Unsubscribe and disable controls when the object is destroyed
            if (playerControls != null)
            {
                playerControls.Player.Quit.performed -= OnQuit;
                playerControls.Disable();
            }
        }

        public void SaveAllData()
        {
            if (gateData != null)
                ES3.Save("GateData", gateData, SaveFileName);

            if (bulbData != null)
                ES3.Save("BulbData", bulbData, SaveFileName);

            if (playerInfo != null)
                ES3.Save("PlayerInfo", playerInfo, SaveFileName);

            if (tutorialData != null)
                ES3.Save("TutorialData", tutorialData, SaveFileName);

            if (spawnData != null)
                ES3.Save("SpawnData", spawnData, SaveFileName);

            Debug.Log("All ScriptableObjects saved.");
        }

        public void LoadAllData()
        {
            if (gateData != null && ES3.KeyExists("GateData", SaveFileName))
            {
                GateData loadedGateData = ES3.Load<GateData>("GateData", SaveFileName);
                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(loadedGateData), gateData);
            }
            else
            {
                Debug.LogWarning("No saved data found for GateData.");
            }

            if (bulbData != null && ES3.KeyExists("BulbData", SaveFileName))
            {
                BulbData loadedBulbData = ES3.Load<BulbData>("BulbData", SaveFileName);
                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(loadedBulbData), bulbData);
            }
            else
            {
                Debug.LogWarning("No saved data found for BulbData.");
            }

            if (playerInfo != null && ES3.KeyExists("PlayerInfo", SaveFileName))
            {
                PlayerInfo loadedPlayerInfo = ES3.Load<PlayerInfo>("PlayerInfo", SaveFileName);
                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(loadedPlayerInfo), playerInfo);
                playerInfo.RestoreCurrencyChangedEvent();
            }
            else
            {
                Debug.LogWarning("No saved data found for PlayerInfo.");
            }

            if (tutorialData != null && ES3.KeyExists("TutorialData", SaveFileName))
            {
                TutorialData loadedTutorialData = ES3.Load<TutorialData>("TutorialData", SaveFileName);
                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(loadedTutorialData), tutorialData);
            }
            else
            {
                Debug.LogWarning("No saved data found for TutorialData.");
            }

            if (spawnData != null && ES3.KeyExists("SpawnData", SaveFileName))
            {
                SpawnData loadedSpawnData = ES3.Load<SpawnData>("SpawnData", SaveFileName);
                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(loadedSpawnData), spawnData);
            }
            else
            {
                Debug.LogWarning("No saved data found for SpawnData.");
            }

            Debug.Log("All ScriptableObjects loaded.");
        }
    }
}
