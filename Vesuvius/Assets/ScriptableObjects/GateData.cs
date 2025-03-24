using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "GateData", menuName = "GameData/GateData")]
    public class GateData : ScriptableObject
    {
        [System.Serializable]
        public class GateEntry
        {
            public string gateID;
            public bool isLocked;
        }

        public List<GateEntry> gateEntries = new List<GateEntry>();

        public bool GetGateLockedState(string gateID)
        {
            var gate = gateEntries.Find(entry => entry.gateID == gateID);
            return gate != null && gate.isLocked;
        }

        public bool SetGateLockedState(string gateID, bool isLocked)
        {
            var gate = gateEntries.Find(entry => entry.gateID == gateID);
            if (gate != null)
            {
                gate.isLocked = isLocked;
                return true;
            }
            Debug.LogWarning($"Gate with ID {gateID} not found in GateData.");
            return false;
        }

        public void ResetGates()
        {
            foreach (var gate in gateEntries)
            {
                gate.isLocked = true;
            }
            Debug.Log("All gates have been reset to locked state.");
        }
    }
}
