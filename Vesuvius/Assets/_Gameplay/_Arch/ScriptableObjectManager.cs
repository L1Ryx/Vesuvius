using System;
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
        public Saveable[] saveables;

        private static ScriptableObjectManager instance;

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
        }

        public void ResetAllData()
        {
            foreach (Saveable saveable in saveables)
            {
                saveable.Reset();
            }

        }

        public void SaveAllData()
        {
            foreach (Saveable SO in saveables)
            {
                ES3.Save(SO.name, SO, SaveFileName);
            }

            Debug.Log("All ScriptableObjects saved.");
        }

        public void LoadAllData()
        {
            foreach (Saveable SO in saveables)
            {
                if (!ES3.KeyExists(SO.name, SaveFileName))
                {
                    Debug.LogWarning($"No saved data found for {SO.name}.");
                    continue;
                }

                Saveable instance = ES3.Load<Saveable>(SO.name, SaveFileName);
                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(instance), SO);
            }

            Debug.Log("All ScriptableObjects loaded.");
        }

        public bool CheckSaveDataExists()
        {
            return ES3.FileExists(SaveFileName);
        }

        public bool DeleteSaveData()
        {
            if(CheckSaveDataExists())
            {
                ES3.DeleteFile(SaveFileName);
                return true;
            }
            else return false;
        }
    }
}
