using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System.Collections;

// public class ReadOnlyAttribute : PropertyAttribute
// {

// }

// [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
// public class ReadOnlyDrawer : PropertyDrawer

// {
//     public override float GetPropertyHeight(SerializedProperty property,
//                                             GUIContent label)
//     {
//         return EditorGUI.GetPropertyHeight(property, label, true);
//     }

//     public override void OnGUI(Rect position,
//                                SerializedProperty property,
//                                GUIContent label)
//     {
//         GUI.enabled = false;
//         EditorGUI.PropertyField(position, property, label, true);
//         GUI.enabled = true;
//     }
// }

[CreateAssetMenu(fileName = "PlayerInfo", menuName = "ScriptableObjects/PlayerInfo", order = 1)]
public class PlayerInfo : ScriptableObject
{
    [Header("Player Info")]
    public bool hasCompletedDemo = false;
    [SerializeField] private int maximumHealth = 5;
    [SerializeField] private int currentHealth = 5;
    [SerializeField] private int totalCurrency = 0;
    [SerializeField] [Range(0, 100)] private int totemPower = 0;
    [SerializeField] private int abilityCost = 30; // Required totems to use the ability
    [Header("Combat Cooldowns")]
    // [SerializeField] private bool disableSwingKnockback = false;
    private float swingKnockbackCooldownEnd = 0f;
    private float lastDamageTime = 0f;


    [Header("Monitoring")]
    public int lastCurrencyChangeAmount = 24;

    [Header("Checkpoint Info")]
    [SerializeField] private string sceneToLoad = ""; // Scene to load on respawn
    [SerializeField] private Vector2 spawnLocation = Vector2.zero; // Player's spawn position in the scene

    [Header("Events")]
    public UnityEvent currencyChanged;
    public GameEvent currencyChangedGameEvent;

    [Header("Event Asset Path")]
    [SerializeField] private string currencyChangedEventPath = "Assets/Events/CurrencyChanged.asset";

    // Getters
    public int GetMaximumHealth() => maximumHealth;
    public int GetCurrentHealth() => currentHealth;
    public int GetTotalCurrency() => totalCurrency;
    public int GetTotemPower() => totemPower;
    public string GetSceneToLoad() => sceneToLoad;
    public Vector2 GetSpawnLocation() => spawnLocation;

    public string GetCurrencyChangedEventPath() => currencyChangedEventPath;

    // Restore GameEvent dynamically
    public void RestoreCurrencyChangedEvent()
    {
        #if UNITY_EDITOR
        // Use AssetDatabase to find the asset in the Editor
        GameEvent loadedEvent = UnityEditor.AssetDatabase.LoadAssetAtPath<GameEvent>(currencyChangedEventPath);
        #else
        // Use Resources.Load for runtime builds (ensure asset is in Resources folder)
        GameEvent loadedEvent = Resources.Load<GameEvent>(currencyChangedEventPath);
        #endif

        if (loadedEvent != null)
        {
            currencyChangedGameEvent = loadedEvent;

            // Dynamically assign the GameEvent's Raise function to the UnityEvent
            if (currencyChanged != null)
            {
                currencyChanged.RemoveAllListeners(); // Clear previous listeners, if any
                currencyChanged.AddListener(() => currencyChangedGameEvent.Raise());
            }

            Debug.Log("CurrencyChanged GameEvent successfully restored and linked to UnityEvent.");
        }
        else
        {
            Debug.LogWarning($"Failed to restore GameEvent from path: {currencyChangedEventPath}");
        }
    }


    // Setters
    public void SetMaximumHealth(int value)
    {
        if (value > 0)
        {
            maximumHealth = value;
            if (currentHealth > maximumHealth)
                currentHealth = maximumHealth;
        }
    }

    public void SetCurrentHealth(int value)
    {
        currentHealth = Mathf.Clamp(value, 0, maximumHealth);
    }

    public void SetTotalCurrency(int value)
    {
        if (value >= 0)
        {
            totalCurrency = value;
        }
    }

    public void SetTotemPower(int value)
    {
        totemPower = Mathf.Clamp(value, 0, 100);
    }

    public void SetCheckpoint(string newScene, Vector2 newLocation)
    {
        sceneToLoad = newScene;
        spawnLocation = newLocation;
    }

    public bool BuyAbility()
    {
        if (totemPower >= abilityCost)
        {
            totemPower -= abilityCost; // Deduct the required totems
            return true; // Ability purchased successfully
        }
        else
        {
            Debug.Log("Not enough totems.");
            return false; // Not enough totems
        }
    }

    public int AddTotemPower(int addedPower)
    {
        int total = totemPower + addedPower;
        if (total >= 100)
        {
            total = 100;
        }
        totemPower = total;
        return totemPower;
    }

    // Public method to add currency
    public void AddCurrency(int amount)
    {
        int oldTotalCurrency = totalCurrency;
        totalCurrency = Mathf.Max(0, totalCurrency + amount);
        lastCurrencyChangeAmount = totalCurrency - oldTotalCurrency;
        currencyChanged.Invoke();
    }

    public bool DecrementHealth() {
        currentHealth -= 1;
        if (currentHealth <= 0) {
            Debug.Log("Player Death");
            return true; // Indicates the player is dead
        }
        return false;
    }

    public void RefillHealthAndTotemPower()
    {
        currentHealth = maximumHealth;
        totemPower = 100;
        Debug.Log("Player fully healed and totem power restored.");
    }

    // Getter for the checkpoint scene
    public string GetCheckpointScene()
    {
        return sceneToLoad;
    }

    // Getter for the spawn location
    public Vector2 GetCheckpointLocation()
    {
        return spawnLocation;
    }

    // Store last time player took damage
    public void SetLastDamageTime()
    {
        lastDamageTime = Time.time;
    }

    // Get last damage time
    public float GetLastDamageTime()
    {
        return lastDamageTime;
    }



    public int GetAbilityCost() => abilityCost;
    public void SetAbilityCost(int value) => abilityCost = Mathf.Max(0, value);
}
