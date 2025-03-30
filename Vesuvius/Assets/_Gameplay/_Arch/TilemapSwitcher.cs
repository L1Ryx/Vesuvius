using System.Collections;
using System.Collections.Generic;
using _ScriptableObjects;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Gameplay._Arch
{
    public class TilemapSwitcher : MonoBehaviour
    {
        [Header("Tilemap Settings")]
        public TileMapping tileMapping;          // Reference to the TileMapping ScriptableObject
        public List<Tilemap> tilemaps;           // List of original Tilemaps
        public GameObject tilemapParent;         // Parent GameObject for new Tilemaps
        public float crossfadeDuration = 1f;     // Duration of the crossfade
        private bool isForestMode = false;       // Track current mode (icy or forest)

        [ContextMenu("[CAUTION] Switch Tilemaps")]
        public void SwitchWithCrossfade()
        {
            StartCoroutine(CrossfadeTilemaps());
        }

        private void Update()
        {
        
            // DEBUGGING ONLY
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                // Trigger the tilemap switching with crossfade
                SwitchWithCrossfade();
                Debug.Log("Tilemap switch activated through debug key `0`.");
            }
        }


        private IEnumerator CrossfadeTilemaps()
        {
            List<TilemapRenderer> newRenderers = new List<TilemapRenderer>();

            // Duplicate each tilemap and assign the alternate tileset
            for (int i = 0; i < tilemaps.Count; i++)
            {
                Tilemap tilemap = tilemaps[i];

                // Create a duplicate GameObject
                GameObject newTilemapObj = Instantiate(tilemap.gameObject, tilemapParent.transform);

                // Rename the cloned GameObject for clarity
                newTilemapObj.name = $"Tilemap_{i}_{(isForestMode ? "Icy" : "Forest")}";

                // Get the TilemapRenderer and Tilemap from the duplicate
                TilemapRenderer newRenderer = newTilemapObj.GetComponent<TilemapRenderer>();
                Tilemap newTilemap = newTilemapObj.GetComponent<Tilemap>();

                // Assign the alternate tiles to the new tilemap using TileMapping
                ReplaceTilesWithMapping(newTilemap);

                // Reset initial alpha to 0 (fully transparent)
                newRenderer.material.color = new Color(1, 1, 1, 0);

                newRenderers.Add(newRenderer);
            }

            // Fade out old tilemaps and fade in new ones
            float elapsed = 0f;
            while (elapsed < crossfadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = elapsed / crossfadeDuration;

                // Update alpha for both old and new tilemaps
                foreach (var tilemap in tilemaps)
                {
                    TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();
                    renderer.material.color = new Color(1, 1, 1, 1 - alpha); // Fade out
                }

                foreach (var renderer in newRenderers)
                {
                    renderer.material.color = new Color(1, 1, 1, alpha); // Fade in
                }

                yield return null;
            }

            // Clean up old tilemaps and finalize the switch
            foreach (var tilemap in tilemaps)
            {
                Destroy(tilemap.gameObject);
            }

            tilemaps.Clear();
            foreach (var renderer in newRenderers)
            {
                tilemaps.Add(renderer.GetComponent<Tilemap>());
            }

            isForestMode = !isForestMode;
        }

        private void ReplaceTilesWithMapping(Tilemap tilemap)
        {
            // Iterate through the tilemap and replace tiles based on the mapping
            BoundsInt bounds = tilemap.cellBounds;
            TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int position = new Vector3Int(x, y, 0);
                    TileBase currentTile = tilemap.GetTile(position);

                    // Get the replacement tile using the TileMapping
                    TileBase newTile = isForestMode
                        ? tileMapping.GetIcyTile(currentTile)    // Replace with icy tiles
                        : tileMapping.GetForestTile(currentTile); // Replace with forest tiles

                    if (newTile != null)
                    {
                        tilemap.SetTile(position, newTile);
                    }
                }
            }
        }
    }
}
