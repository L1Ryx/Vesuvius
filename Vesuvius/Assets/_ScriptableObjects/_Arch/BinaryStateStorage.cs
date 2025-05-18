using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

//Scriptable object that can be created and used to store guids to objects that have some binary state
//For example, you could store the guids of all gates that have been opened, so that when loading a Scene
//it keeps them opened.
[CreateAssetMenu(fileName = "BinaryStateStorage", menuName = "Scriptable Objects/BinaryStateStorage")]
public class BinaryStateStorage : ScriptableObject
{
    HashSet<string> binaryStateStorage = new HashSet<string>();

    public bool isInteractableBlocked(string key)
    {
        return binaryStateStorage.Contains(key);
    }

    public void Add(string key)
    {
        binaryStateStorage.Add(key);
    }
}
