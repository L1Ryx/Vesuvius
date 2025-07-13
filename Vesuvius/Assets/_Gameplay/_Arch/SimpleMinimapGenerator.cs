using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;

public class SimpleMinimapGenerator : MonoBehaviour {
    [Header("Tilemap Sources")]
    public List<Tilemap> sourceTilemaps;

    [Header("Output")]
    public RawImage minimapDisplay;

    [Header("Colors")]
    public Color tileColor = Color.white;
    public Color emptyColor = Color.black;
    [Header("Player Tracking")]
    public Transform player; // public for debugging rn
    public RectTransform playerDot;
    public Color flickerColorA = Color.red;
    public Color flickerColorB = Color.yellow;
    public float flickerRate = 0.5f; // seconds between color switches

    private float flickerTimer = 0f;
    private bool useColorA = true;

    private Vector2Int textureOrigin;
    private Vector2Int textureSize;

    [Header("Visuals")]
    public float baseMapSize = 100f; 
    public Vector2 playerDotOffset = Vector2.zero; // pixels
    void Start() {
        GenerateMinimap();
        AutoAssignPlayer();
    }

    void AutoAssignPlayer()
    {
        if (player == null) {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null) player = found.transform;
        }

        GenerateMinimap();
    }

    void GenerateMinimap() {
        if (sourceTilemaps == null || sourceTilemaps.Count == 0 || minimapDisplay == null) {
            Debug.LogWarning("SimpleMinimapGenerator: Missing references.");
            return;
        }

        // Get combined bounds
        BoundsInt combinedBounds = sourceTilemaps[0].cellBounds;
        foreach (var tm in sourceTilemaps) {
            combinedBounds.xMin = Mathf.Min(combinedBounds.xMin, tm.cellBounds.xMin);
            combinedBounds.yMin = Mathf.Min(combinedBounds.yMin, tm.cellBounds.yMin);
            combinedBounds.xMax = Mathf.Max(combinedBounds.xMax, tm.cellBounds.xMax);
            combinedBounds.yMax = Mathf.Max(combinedBounds.yMax, tm.cellBounds.yMax);
        }

        int width = combinedBounds.size.x;
        int height = combinedBounds.size.y;
        Texture2D tex = new Texture2D(width, height);
        tex.filterMode = FilterMode.Point;

        for (int x = combinedBounds.xMin; x < combinedBounds.xMax; x++) {
            for (int y = combinedBounds.yMin; y < combinedBounds.yMax; y++) {
                Vector3Int pos = new Vector3Int(x, y, 0);
                bool tileExists = false;

                foreach (var tm in sourceTilemaps) {
                    if (tm.HasTile(pos)) {
                        tileExists = true;
                        break;
                    }
                }

                Color pixel = tileExists ? tileColor : emptyColor;
                tex.SetPixel(x - combinedBounds.xMin, y - combinedBounds.yMin, pixel);
            }
        }

        tex.Apply();
        minimapDisplay.texture = tex;
        RectTransform rt = minimapDisplay.rectTransform;

        float aspectRatio = (float)width / height;

        if (aspectRatio >= 1f) {
            rt.sizeDelta = new Vector2(baseMapSize, baseMapSize / aspectRatio);
        } else {
            rt.sizeDelta = new Vector2(baseMapSize * aspectRatio, baseMapSize);
        }
        
        // position mapping bounds
        textureOrigin = new Vector2Int(combinedBounds.xMin, combinedBounds.yMin);
        textureSize = new Vector2Int(width, height);
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(minimapDisplay.rectTransform);

        RectTransform mapRT = minimapDisplay.rectTransform;
        Debug.Log($"Minimap sizeDelta: {mapRT.sizeDelta}, rect: {mapRT.rect.size}");


    }
    
    void Update() {
        if (player == null || playerDot == null || textureSize == Vector2Int.zero)
            return;

        // flickering
        flickerTimer += Time.deltaTime;
        if (flickerTimer >= flickerRate) {
            flickerTimer = 0f;
            useColorA = !useColorA;
            playerDot.GetComponent<Image>().color = useColorA ? flickerColorA : flickerColorB;
        }

        // convert player world pos to minimap texture pos
        Vector3 worldPos = player.position;
        Vector3Int cellPos = sourceTilemaps[0].WorldToCell(worldPos);
        int x = cellPos.x - textureOrigin.x;
        int y = cellPos.y - textureOrigin.y;

        // normalize to 0-1
        float normX = (float)x / textureSize.x;
        float normY = (float)y / textureSize.y;

        // scale to minimap size
        RectTransform mapRT = minimapDisplay.rectTransform;
        float px = normX * mapRT.rect.width;
        float py = normY * mapRT.rect.height;

        playerDot.anchoredPosition = new Vector2(px, py) + playerDotOffset;
        
        Debug.DrawLine(player.position, player.position + Vector3.up * 2f, Color.red);

        
        




    }
}
