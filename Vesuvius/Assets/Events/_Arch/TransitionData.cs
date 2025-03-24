using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: DECOUPLE FROM CURRENT SCENE
[CreateAssetMenu(fileName = "TransitionData", menuName = "Scene Management/Transition Data")]
public class TransitionData : ScriptableObject {
    public string sceneToLoad;
    public Vector2 spawnPosition;
    
}
