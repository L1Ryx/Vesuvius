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
        private Saveable[] saveables;

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

            //automatically load all saveable SOs stored in the resources folder
            saveables = Resources.LoadAll<Saveable>("");
        }

        public void ResetAllData()
        {
            foreach (Saveable SO in saveables)
            {
                if (SO == null)
                {
                    Debug.LogWarning("Null reference in saveables list");
                    continue;
                }
                SO.Reset();
            }

        }

        public void SaveAllData()
        {
            foreach (Saveable SO in saveables)
            {
                if(SO == null)
                {
                    Debug.LogWarning("Null reference in saveables list");
                    continue;
                }
                ES3.Save(SO.name, SO, SaveFileName);
            }

            Debug.Log("All ScriptableObjects saved.");
        }

        public void LoadAllData()
        {
            foreach (Saveable SO in saveables)
            {
                if (SO == null)
                {
                    Debug.LogWarning("Null reference in saveables list");
                    continue;
                }
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
