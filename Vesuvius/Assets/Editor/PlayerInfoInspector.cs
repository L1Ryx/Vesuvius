using _ScriptableObjects.PlayerInfo;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerInfo))]
public class PlayerInfoEditor : Editor
{
    private int currencyAmountToAdd; // Field for the amount of currency to add

    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Reference to the PlayerInfo ScriptableObject
        PlayerInfo playerInfo = (PlayerInfo)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Health Controls", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Increment Max Health"))
        {
            playerInfo.SetMaximumHealth(playerInfo.GetMaximumHealth() + 1);
        }

        if (GUILayout.Button("Decrement Max Health"))
        {
            playerInfo.SetMaximumHealth(playerInfo.GetMaximumHealth() - 1);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Increment Current Health"))
        {
            playerInfo.SetCurrentHealth(playerInfo.GetCurrentHealth() + 1);
        }

        if (GUILayout.Button("Decrement Current Health"))
        {
            playerInfo.SetCurrentHealth(playerInfo.GetCurrentHealth() - 1);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Currency Controls", EditorStyles.boldLabel);

        // Input field for the amount of currency to add
        currencyAmountToAdd = EditorGUILayout.IntField("Currency Amount to Add", currencyAmountToAdd);

        // Button to apply the currency change using the AddCurrency method
        if (GUILayout.Button("Add/Subtract Currency"))
        {
            playerInfo.AddCurrency(currencyAmountToAdd);
        }

        // Save changes to the ScriptableObject
        if (GUI.changed)
        {
            EditorUtility.SetDirty(playerInfo);
        }
    }
}
