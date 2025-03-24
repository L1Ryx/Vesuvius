using ScriptableObjects;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TutorialData))]
public class TutorialDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector UI
        DrawDefaultInspector();

        // Add a space for better UI appearance
        EditorGUILayout.Space();

        // Add a button to clear all tutorials
        if (GUILayout.Button("Clear All Tutorials"))
        {
            // Get the target object as TutorialData
            TutorialData tutorialData = (TutorialData)target;

            // Confirm before clearing
            if (EditorUtility.DisplayDialog(
                "Clear All Tutorials",
                "Are you sure you want to clear all tutorials? This action cannot be undone.",
                "Yes", "No"))
            {
                // Call the ResetTutorials method to clear the list
                tutorialData.ResetTutorials();

                // Mark the ScriptableObject as dirty so changes are saved
                EditorUtility.SetDirty(tutorialData);

                Debug.Log("All tutorials have been cleared.");
            }
        }
    }
}
