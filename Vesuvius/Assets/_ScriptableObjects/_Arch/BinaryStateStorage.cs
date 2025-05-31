using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

//Scriptable object that can be created and used to store guids to objects that have some binary state
//For example, you could store the guids of all gates that have been opened, so that when loading a Scene
//it keeps them opened.
[CreateAssetMenu(fileName = "BinaryStateStorage", menuName = "Scriptable Objects/BinaryStateStorage")]
public class BinaryStateStorage : ScriptableObject
{
    [SerializeField]
    HashSet<string> binaryStateStorage = new HashSet<string>();

    public List<string> shownGuids = new List<string>();

    public bool isInteractableBlocked(string key)
    {
        return binaryStateStorage.Contains(key);
    }

    void OnEnable()
    {
        foreach (string guid in shownGuids)
        {
            binaryStateStorage.Add(guid);
        }
    }

    public void Add(string key)
    {
        binaryStateStorage.Add(key);
        ShowInList();
    }

    private void ShowInList()
    {
        foreach (string guid in binaryStateStorage)
        {
            shownGuids.Add(guid);
        }
    }
}
