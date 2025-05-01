using System.Collections.Generic;
using UnityEngine;

namespace _ScriptableObjects
{
    [CreateAssetMenu(fileName = "TutorialData", menuName = "ScriptableObjects/TutorialData", order = 2)]
    public class TutorialData : ScriptableObject
    {
        [Header("Tutorial Tracking")]
        [Tooltip("List of tutorial IDs that have been shown.")]
        public List<string> shownTutorials = new List<string>();

        [Header("Template Tutorial Text")]
        //[Tooltip("List of tutorial IDs that have been shown.")]
        public string moveTutorial;
        public string swingTutorial;
        public string jumpTutorial;
        public string healTutorial;
        public string interactTutorial;

        [Header("Scheme Tutorial Text")]
        [Tooltip("Text replaced tutorial templates for current control scheme")]
        public string movePrompt;
        public string swingPrompt;
        public string jumpPrompt;
        public string healPrompt;
        public string interactPrompt;

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
        public void ResetTutorials()
        {
            shownTutorials.Clear();
            Debug.Log("All tutorials have been reset.");
        }
    }
}
