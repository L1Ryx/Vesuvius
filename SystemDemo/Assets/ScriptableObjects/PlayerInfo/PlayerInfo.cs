using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInfo", menuName = "ScriptableObjects/PlayerInfo", order = 1)]
public class PlayerInfo : ScriptableObject
{
    [Header("Player Info")]
    [SerializeField] private int maximumHealth = 5;
    [SerializeField] private int currentHealth = 5;
    [SerializeField] private int totalCurrency = 0;
    [SerializeField] [Range(0, 100)] private int totemPower = 0;
    [SerializeField] private int abilityCost = 30; // Required totems to use the ability

    // Getters
    public int GetMaximumHealth() => maximumHealth;
    public int GetCurrentHealth() => currentHealth;
    public int GetTotalCurrency() => totalCurrency;
    public int GetTotemPower() => totemPower;


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

    public int DecrementHealth() {
        currentHealth -= 1;
        if (currentHealth <= 0) {
            Debug.Log("Player Death");
            // death logic
        }
        return currentHealth;
    }

    public int GetAbilityCost() => abilityCost;
    public void SetAbilityCost(int value) => abilityCost = Mathf.Max(0, value);
}


