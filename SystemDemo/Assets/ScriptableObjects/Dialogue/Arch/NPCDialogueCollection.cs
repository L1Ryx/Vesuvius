using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCDialogueCollection", menuName = "NPC/Dialogue Collection")]
public class NPCDialogueCollection : ScriptableObject
{
    [System.Serializable]
    public class DialogueTree
    {
        public string treeID; // Unique identifier for the dialogue tree
        [TextArea] public List<string> dialogues; // Ordered dialogues for the tree
        public List<string> wwiseEvents; // List of optional Wwise events corresponding to dialogues
        public string nextTreeID; // ID of the next dialogue tree to transition to
    }

    [Header("Dialogue Trees")]
    [SerializeField] private List<DialogueTree> dialogueTrees = new List<DialogueTree>();

    /// <summary>
    /// Retrieves a dialogue tree by its unique ID.
    /// </summary>
    /// <param name="treeID">The ID of the dialogue tree to retrieve.</param>
    /// <returns>The DialogueTree with the specified ID, or null if not found.</returns>
    public DialogueTree GetDialogueTreeByID(string treeID)
    {
        return dialogueTrees.Find(tree => tree.treeID == treeID);
    }

    /// <summary>
    /// Retrieves a specific dialogue from a given tree by index.
    /// </summary>
    /// <param name="treeID">The ID of the dialogue tree.</param>
    /// <param name="dialogueIndex">The index of the dialogue within the tree.</param>
    /// <returns>The dialogue string, or null if not found.</returns>
    public string GetDialogueFromTree(string treeID, int dialogueIndex)
    {
        DialogueTree tree = GetDialogueTreeByID(treeID);
        if (tree != null && dialogueIndex >= 0 && dialogueIndex < tree.dialogues.Count)
        {
            return tree.dialogues[dialogueIndex];
        }
        Debug.LogWarning($"Dialogue not found: TreeID={treeID}, Index={dialogueIndex}");
        return null;
    }

    /// <summary>
    /// Retrieves the next tree ID for a given dialogue tree.
    /// </summary>
    /// <param name="treeID">The ID of the current dialogue tree.</param>
    /// <returns>The ID of the next dialogue tree, or null if not defined.</returns>
    public string GetNextTreeID(string treeID)
    {
        DialogueTree tree = GetDialogueTreeByID(treeID);
        if (tree != null)
        {
            return tree.nextTreeID;
        }
        Debug.LogWarning($"Next tree ID not found: TreeID={treeID}");
        return null;
    }

    /// <summary>
    /// Checks if a dialogue tree exists by ID.
    /// </summary>
    /// <param name="treeID">The ID of the dialogue tree to check.</param>
    /// <returns>True if the dialogue tree exists, false otherwise.</returns>
    public bool HasDialogueTree(string treeID)
    {
        return dialogueTrees.Exists(tree => tree.treeID == treeID);
    }

    /// <summary>
    /// Adds a new dialogue tree to the collection.
    /// </summary>
    /// <param name="treeID">The unique ID for the new dialogue tree.</param>
    public void AddDialogueTree(string treeID)
    {
        if (!HasDialogueTree(treeID))
        {
            dialogueTrees.Add(new DialogueTree { treeID = treeID });
        }
        else
        {
            Debug.LogWarning($"Dialogue tree with ID '{treeID}' already exists.");
        }
    }
}
