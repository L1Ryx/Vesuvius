using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Draws the default inspector

        GameEvent gameEvent = (GameEvent)target;

        if (GUILayout.Button("Raise"))
        {
            gameEvent.Raise(); // Calls the Raise method when the button is pressed
        }
    }
}
