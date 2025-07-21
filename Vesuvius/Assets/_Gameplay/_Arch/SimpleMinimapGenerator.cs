using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;

public class SimpleMinimapGenerator : MonoBehaviour {
    [Header("Tilemap Sources")]
    public List<Tilemap> dimension1Tilemaps;
    public List<Tilemap> dimension2Tilemaps;

    private List<Tilemap> sourceTilemaps = new List<Tilemap>();
    [Header("UI References")]
    public GameObject minimapCanvas; 
    
    [Header("Data Cubes")]
    public GameState gameState;


    [Header("Output")]
    public RawImage minimapDisplay;

    [Header("Dimension 1 Colors")]
    public Color tileColor1 = Color.white;
    public Color emptyColor1 = Color.black;

    [Header("Dimension 2 Colors")]
    public Color tileColor2 = Color.cyan;
    public Color emptyColor2 = Color.gray;
    [Header("Player Tracking")]
    public Transform player; // public for debugging rn
    public RectTransform playerDot;
    public Color flickerColorA = Color.red;
    public Color flickerColorB = Color.yellow;
    public float flickerRate = 0.5f; // seconds between color switches

    private bool lastRealityState = false;
    private float flickerTimer = 0f;
    private bool useColorA = true;
    private CanvasGroup canvasGroup;

    private Vector2Int textureOrigin;
    private Vector2Int textureSize;
    private Color currentTileColor;
    private Color currentEmptyColor;
    private Color targetTileColor;
    private Color targetEmptyColor;
    private float colorLerpTime = 0f;
    private bool isTransitioning = false;
    private float targetAlpha = 0f;
    private float currentAlpha = 0f;

    [Header("Visuals")]
    public float minimapPixelsPerTile = 4f;
    public float baseMapSize = 100f; 
    public Vector2 playerDotOffset = Vector2.zero; // pixels
    public float transitionDuration = 1f; // Seconds
    public float showMapFadeSpeed = 5f; 

    void Awake()
    {
        minimapCanvas.SetActive(true);
        canvasGroup = minimapCanvas.GetComponent<CanvasGroup>();
        if (!canvasGroup)
        {
            Debug.LogWarning("no canvasGroup");
        }
    }
    void Start() 
    {
        AutoAssignPlayer();
        currentTileColor = tileColor1;
        currentEmptyColor = emptyColor1;
        EnterFirstDimension();
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

                Color pixel = tileExists ? currentTileColor : currentEmptyColor; 
                tex.SetPixel(x - combinedBounds.xMin, y - combinedBounds.yMin, pixel);
            }
        }

        tex.Apply();
        minimapDisplay.texture = tex;
        RectTransform rt = minimapDisplay.rectTransform;

        float uiWidth = width * minimapPixelsPerTile;
        float uiHeight = height * minimapPixelsPerTile;

        rt.sizeDelta = new Vector2(uiWidth, uiHeight);

        
        // position mapping bounds
        textureOrigin = new Vector2Int(combinedBounds.xMin, combinedBounds.yMin);
        textureSize = new Vector2Int(width, height);
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(minimapDisplay.rectTransform);

        RectTransform mapRT = minimapDisplay.rectTransform;
        //Debug.Log($"Minimap sizeDelta: {mapRT.sizeDelta}, rect: {mapRT.rect.size}");


    }
    
    void Update() {
        if (isTransitioning)
        {
            colorLerpTime += Time.deltaTime;
            float t = Mathf.Clamp01(colorLerpTime / transitionDuration);
            currentTileColor = Color.Lerp(currentTileColor, targetTileColor, t);
            currentEmptyColor = Color.Lerp(currentEmptyColor, targetEmptyColor, t);
            GenerateMinimap(); 

            if (t >= 1f)
            {
                isTransitioning = false;
                currentTileColor = targetTileColor;
                currentEmptyColor = targetEmptyColor;
            }
        }
        if (gameState != null && gameState.isAltReality != lastRealityState)
        {
            lastRealityState = gameState.isAltReality;
            if (gameState.isAltReality)
            {
                EnterSecondDimension();
            }
            else
            {
                EnterFirstDimension();
            }
        }
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

        RevealWithTabKey();
    }

    private void RevealWithTabKey()
    {
        targetAlpha = Input.GetKey(KeyCode.Tab) ? 1f : 0f;

        // Smoothly lerp the alpha
        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime * showMapFadeSpeed);
        canvasGroup.alpha = currentAlpha;

        // Optional: disable interaction + raycasts when invisible
        canvasGroup.blocksRaycasts = currentAlpha > 0.01f;
        canvasGroup.interactable = currentAlpha > 0.01f;
    }
    public void EnterFirstDimension()
    {
        sourceTilemaps = dimension1Tilemaps;
        SetTargetColors(tileColor1, emptyColor1);
    }

    public void EnterSecondDimension()
    {
        sourceTilemaps = dimension2Tilemaps;
        SetTargetColors(tileColor2, emptyColor2);
    }

    void SetTargetColors(Color tile, Color empty)
    {
        targetTileColor = tile;
        targetEmptyColor = empty;
        colorLerpTime = 0f;
        isTransitioning = true;
    }

}
