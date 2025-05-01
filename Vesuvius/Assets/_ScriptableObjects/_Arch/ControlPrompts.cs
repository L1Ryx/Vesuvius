using System.Collections.Generic;
using UnityEngine;

namespace _ScriptableObjects
{
    /// <summary>
    /// SO that holds all prompts containing a control button, for use with text replacement
    /// New prompts will need new entries here.          
    /// </summary>
    [CreateAssetMenu(fileName = "ControlPrompts", menuName = "ScriptableObjects/ControlPrompts", order = 2)]
    public class ControlPrompts : ScriptableObject
    {

        [Header("Template Prompt Text")]
        [Tooltip("Use the name of the action in {} to be text replaced.")]
        public string movePromptTemplate;
        public string swingPromptTemplate;
        public string jumpPromptTemplate;
        public string healPromptTemplate;
        public string interactPromptTemplate;

        [Header("Scheme Prompt Text")]
        [Tooltip("Text replaced prompt templates for current control scheme")]
        public string movePrompt;
        public string swingPrompt;
        public string jumpPrompt;
        public string healPrompt;
        public string interactPrompt;
    }
}