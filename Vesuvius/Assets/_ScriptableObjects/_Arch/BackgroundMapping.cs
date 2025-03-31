using System.Collections.Generic;
using UnityEngine;

namespace _ScriptableObjects
{
    [CreateAssetMenu(fileName = "BackgroundMapping", menuName = "ScriptableObjects/BackgroundMapping", order = 1)]
    public class BackgroundMapping : ScriptableObject
    {
        [System.Serializable]
        public class BackgroundPair
        {
            public Sprite icySprite;       // Reference to the icy sprite
            public Sprite forestSprite;   // Reference to the forest sprite
        }

        public List<BackgroundPair> backgroundPairs = new List<BackgroundPair>();

        // Get the forest sprite for a given icy sprite
        public Sprite GetForestSprite(Sprite icySprite)
        {
            foreach (var pair in backgroundPairs)
            {
                if (pair.icySprite == icySprite)
                    return pair.forestSprite;
            }
            return null;
        }

        // Get the icy sprite for a given forest sprite (optional)
        public Sprite GetIcySprite(Sprite forestSprite)
        {
            foreach (var pair in backgroundPairs)
            {
                if (pair.forestSprite == forestSprite)
                    return pair.icySprite;
            }
            return null;
        }
    }
}
