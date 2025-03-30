using _Gameplay._Arch;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerSpawner))]
public class PlayerSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector
        DrawDefaultInspector();

        // Add spacing
        EditorGUILayout.Space();

        // Reference to the target component
        PlayerSpawner spawner = (PlayerSpawner)target;

        // Add a button that calls UpdateSpawnLocationFromDummy
        if (GUILayout.Button("Update Spawn Location From Dummy"))
        {
            spawner.UpdateSpawnLocationFromDummy();
            EditorUtility.SetDirty(spawner); // Mark as dirty so changes are saved
        }
    }
}