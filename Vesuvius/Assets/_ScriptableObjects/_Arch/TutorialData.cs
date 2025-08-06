using System.Collections.Generic;
using UnityEngine;

namespace _ScriptableObjects
{
    [CreateAssetMenu(fileName = "TutorialData", menuName = "ScriptableObjects/TutorialData", order = 2)]
    public class TutorialData : Saveable
    {
        [Header("Tutorial Tracking")]
        [Tooltip("List of tutorial IDs that have been shown.")]
        public List<string> shownTutorials = new List<string>();

        /// <summary>
        /// Marks a tutorial as shown by adding its ID to the list.
        /// </summary>
        /// <param name="tutorialID">The unique identifier of the tutorial.</param>
        public void MarkTutorialAsShown(string tutorialID)
        {
            if (!shownTutorials.Contains(tutorialID))
            {
                shownTutorials.Add(tutorialID);
                Debug.Log($"Tutorial {tutorialID} marked as shown.");
            }
        }

        /// <summary>
        /// Checks if a tutorial has already been shown.
        /// </summary>
        /// <param name="tutorialID">The unique identifier of the tutorial.</param>
        /// <returns>True if the tutorial has been shown, otherwise false.</returns>
        public bool HasTutorialBeenShown(string tutorialID)
        {
            return shownTutorials.Contains(tutorialID);
        }

        /// <summary>
        /// Clears all tracked tutorials, resetting the list.
        /// </summary>
        public override void Reset()
        {
            shownTutorials.Clear();
            Debug.Log("All tutorials have been reset.");
        }
    }
}
