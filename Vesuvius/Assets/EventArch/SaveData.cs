using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class SaveData : ScriptableObject
{
    public Transform playerTransform;
    public Scene playerScene;
}
