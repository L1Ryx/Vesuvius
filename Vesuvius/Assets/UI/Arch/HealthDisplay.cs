using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform healthStartPoint; // RectTransform for proper UI positioning
    [SerializeField] private GameObject filledHealthBlockPrefab;
    [SerializeField] private GameObject unfilledHealthBlockPrefab;
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private GameObject mockup;

    [Header("Block Settings")]
    [SerializeField] private float blockSpacing = 50f; // Distance between each health block in local space

    private int lastMaxHealth;
    private int lastCurrentHealth;

    private readonly System.Collections.Generic.List<GameObject> healthBlocks = new();

    private void Start()
    {
        mockup.SetActive(false);
        UpdateHealthDisplay();
        CacheHealthValues();
    }

    private void Update()
    {
        if (HealthChanged())
        {
            UpdateHealthDisplay();
            CacheHealthValues();
        }
    }

    private void UpdateHealthDisplay()
    {
        ClearHealthBlocks();

        // Instantiate unfilled health blocks for maximum health
        for (int i = 0; i < playerInfo.GetMaximumHealth(); i++)
        {
            InstantiateHealthBlock(unfilledHealthBlockPrefab, i);
        }

        // Instantiate filled health blocks for current health
        for (int i = 0; i < playerInfo.GetCurrentHealth(); i++)
        {
            InstantiateHealthBlock(filledHealthBlockPrefab, i);
        }
    }

    private void InstantiateHealthBlock(GameObject prefab, int index)
    {
        // Instantiate the prefab as a child of the healthStartPoint's parent
        GameObject healthBlock = Instantiate(prefab, healthStartPoint.parent);

        // Get the RectTransform of the new health block
        RectTransform rectTransform = healthBlock.GetComponent<RectTransform>();

        // Match the size of the prefab
        RectTransform prefabRect = prefab.GetComponent<RectTransform>();
        rectTransform.sizeDelta = prefabRect.sizeDelta;

        // Position the block based on the index and spacing
        rectTransform.anchoredPosition = healthStartPoint.anchoredPosition + new Vector2(index * blockSpacing, 0);

        // Reset the scale to avoid unintended scaling issues
        rectTransform.localScale = Vector3.one;

        // Add the block to the list for tracking
        healthBlocks.Add(healthBlock);
    }

    private void ClearHealthBlocks()
    {
        foreach (var block in healthBlocks)
        {
            Destroy(block);
        }
        healthBlocks.Clear();
    }

    private bool HealthChanged()
    {
        return lastMaxHealth != playerInfo.GetMaximumHealth() || lastCurrentHealth != playerInfo.GetCurrentHealth();
    }

    private void CacheHealthValues()
    {
        lastMaxHealth = playerInfo.GetMaximumHealth();
        lastCurrentHealth = playerInfo.GetCurrentHealth();
    }
}
