using System;
using System.Text.RegularExpressions;
using _Gameplay._Arch;
using Events._Arch;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(GameEventListener))] //call OnControlsChanged with ControlsChanged Game Event
public class ControlsTextReplacement : MonoBehaviour
{
    public TMP_Text[] textsNeedingReplacement;
    private string[] textTemplates; //caches text before replacement so we can reuse it
    private string pattern = "{((.|\n|\r)*)}"; //text to be replaced must be enclosed in {}

    private PlayerInput m_PlayerInput;

    // Instantiate the regular expression object.
    private Regex r;

    public void OnControlsChanged()
    {
        for (int i = 0; i < textsNeedingReplacement.Length; i++)
        {
            MatchCollection matches = r.Matches(textTemplates[i]);
            if (matches.Count == 0)
            {
                Debug.LogError("No text replacement found, is your action enclosed in {}?" + "\n"
                    + "Check " + GetFullName(textsNeedingReplacement[i].gameObject));
            }
            //doesn't currently handle having multiple text replacements required in one line
            foreach (Match match in matches)
            {
                //protect against key not found exception in the case of mistakes and prints trace to text where issue occurs
                try
                {
                    textsNeedingReplacement[i].text = textTemplates[i].Replace("{" + match.Groups[1].Value + "}",
                        GetBindingDisplayStringOrCompositeName(m_PlayerInput.actions[match.Groups[1].Value]));
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message + "\n" + "Check " + GetFullName(textsNeedingReplacement[i].gameObject));
                }
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //some texts have been set in the inspector. This doesn't always have to be the case (ie it is initialized by interactable)
        if (textsNeedingReplacement.Length > 0)
        {
            Initialize(textsNeedingReplacement);
        }
    }

    // class can be set up from an external class (such as interactable)
    public void Initialize(TMP_Text[] textsToReplace)
    {
        textsNeedingReplacement = textsToReplace;
        r = new Regex(pattern);
        textTemplates = new string[textsNeedingReplacement.Length];
        for (int i = 0; i < textsNeedingReplacement.Length; i++)
        {
            textTemplates[i] = textsNeedingReplacement[i].text;
        }

        m_PlayerInput = FindFirstObjectByType<PlayerSpawner>()?.GetRuntimePlayer().GetComponent<PlayerInput>();

        OnControlsChanged();
    }

    string GetBindingDisplayStringOrCompositeName(InputAction action)
    {
        // if composite action / can't use binding index or need to specify
        int bindingIndex = action.GetBindingIndex(group: m_PlayerInput.currentControlScheme);

        if (action.bindings[bindingIndex].isPartOfComposite)
        {
            // hard coded logic - assumes that if you found a part of a composite, that it's the first one.
            // And that the one preceeding it, must be the 'Composite head' that contains the parts
            return action.bindings[bindingIndex - 1].name;
        }
        else { return action.GetBindingDisplayString(); } // if not a composite, bindingId can just be updated
    }
    string GetFullName(GameObject go)
    {
        string name = go.name;
        while (go.transform.parent != null)
        {

            go = go.transform.parent.gameObject;
            name = go.name + "/" + name;
        }
        return name;
    }
}
