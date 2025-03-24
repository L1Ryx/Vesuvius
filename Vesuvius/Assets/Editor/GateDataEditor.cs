using ScriptableObjects;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GateData))]
public class GateDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GateData gateData = (GateData)target;

        if (GUILayout.Button("Reset All Gates"))
        {
            gateData.ResetGates();
            EditorUtility.SetDirty(gateData); // Mark the ScriptableObject as dirty so changes are saved
            Debug.Log("ResetGates button pressed. All gates are now locked.");
        }
    }
}
