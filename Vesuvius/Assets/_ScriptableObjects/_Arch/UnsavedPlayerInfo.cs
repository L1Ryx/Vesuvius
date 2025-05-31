using UnityEngine;

//TODO: temporary separate SO for data that I want to store but don't want to save
// will roll it into our normal playerinfo SO after we sort out how to select which things to save

[CreateAssetMenu(fileName = "UnsavedPlayerInfo", menuName = "Scriptable Objects/UnsavedPlayerInfo")]
public class UnsavedPlayerInfo : ScriptableObject
{
    public bool isInMenuMode = false;
}
