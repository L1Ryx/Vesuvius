using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _ScriptableObjects
{
    [CreateAssetMenu(fileName = "TileMapping", menuName = "ScriptableObjects/TileMapping", order = 1)]
    public class TileMapping : ScriptableObject
    {
        [System.Serializable]
        public class TilePair
        {
            public TileBase icyTile;       // Reference to the icy tile
            public TileBase forestTile;   // Reference to the lush forest tile
        }
        public List<TilePair> tilePairs = new List<TilePair>();

        // Find the forest tile corresponding to a given icy tile
        public TileBase GetForestTile(TileBase icyTile)
        {
            foreach (var pair in tilePairs)
            {
                if (pair.icyTile == icyTile)
                    return pair.forestTile;
            }
            return null; // Return null if no match is found
        }

        // Find the icy tile corresponding to a given forest tile (optional)
        public TileBase GetIcyTile(TileBase forestTile)
        {
            foreach (var pair in tilePairs)
            {
                if (pair.forestTile == forestTile)
                    return pair.icyTile;
            }
            return null;
        }
    }
}
