using System.Collections.Generic;
using UnityEngine;

namespace _ScriptableObjects
{
    [CreateAssetMenu(fileName = "BulbData", menuName = "GameData/BulbData")]
    public class BulbData : Saveable
    {
        [System.Serializable]
        public class BulbEntry
        {
            public string bulbID;
            public bool isAlive;
            public int clinkAmount; // Amount of clink this bulb contains
        }

        public int GetClinkAmount(string bulbID)
        {
            var bulb = bulbEntries.Find(entry => entry.bulbID == bulbID);
            if (bulb != null)
            {
                return bulb.clinkAmount;
            }
            Debug.LogWarning($"Bulb with ID {bulbID} not found in BulbData.");
            return 0;
        }


        public List<BulbEntry> bulbEntries = new List<BulbEntry>();

        public bool GetBulbAliveState(string bulbID)
        {
            var bulb = bulbEntries.Find(entry => entry.bulbID == bulbID);
            return bulb != null && bulb.isAlive;
        }

        public bool SetBulbAliveState(string bulbID, bool isAlive)
        {
            var bulb = bulbEntries.Find(entry => entry.bulbID == bulbID);
            if (bulb != null)
            {
                bulb.isAlive = isAlive;
                return true;
            }
            Debug.LogWarning($"Bulb with ID {bulbID} not found in BulbData.");
            return false;
        }

        public void ResetBulbs()
        {
            foreach (var bulb in bulbEntries)
            {
                bulb.isAlive = true;
            }
            Debug.Log("All bulbs have been reset to alive state.");
        }

        public override void Reset()
        {
            //none
        }
    }
}
