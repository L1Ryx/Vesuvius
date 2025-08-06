using _ScriptableObjects;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BulbData))]
public class BulbDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BulbData bulbData = (BulbData)target;

        if (GUILayout.Button("Reset All Bulbs"))
        {
            bulbData.Reset();
            EditorUtility.SetDirty(bulbData); // Mark the ScriptableObject as dirty so changes are saved
            Debug.Log("ResetBulbs button pressed. All bulbs are now marked as alive.");
        }
    }
}
