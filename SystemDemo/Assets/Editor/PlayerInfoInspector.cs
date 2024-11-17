using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerInfoInspector))]
public class PlayerInfoInspector : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
    }
}

[CustomEditor(typeof(PlayerInfo))]
public class PlayerInfoEditor : Editor
{
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

        // Save changes to the ScriptableObject
        if (GUI.changed)
        {
            EditorUtility.SetDirty(playerInfo);
        }
    }
}
