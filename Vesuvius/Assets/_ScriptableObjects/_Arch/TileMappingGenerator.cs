using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _ScriptableObjects
{
    public class TileMappingGenerator : MonoBehaviour
    {
        [Header("Tile Palette References")]
        public List<TileBase> icyTiles;      // Drag the icy tile assets here
        public List<TileBase> forestTiles;  // Drag the forest tile assets here
        public TileMapping tileMapping;     // The TileMapping ScriptableObject to populate

        [ContextMenu("Generate Tile Mapping")]
        public void GenerateTileMapping()
        {
            if (tileMapping == null || icyTiles == null || forestTiles == null)
            {
                Debug.LogError("Please assign all references before generating the mapping.");
                return;
            }

            if (icyTiles.Count != forestTiles.Count)
            {
                Debug.LogError("The number of icy tiles and forest tiles must match!");
                return;
            }

            tileMapping.tilePairs.Clear(); // Clear any existing entries

            for (int i = 0; i < icyTiles.Count; i++)
            {
                TileMapping.TilePair newPair = new TileMapping.TilePair
                {
                    icyTile = icyTiles[i],
                    forestTile = forestTiles[i]
                };
                tileMapping.tilePairs.Add(newPair);
            }

            // Ensure this part only runs in the Unity Editor
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(tileMapping); // Save changes to the ScriptableObject
#endif

            Debug.Log("Tile Mapping successfully generated!");
        }
    }
}
