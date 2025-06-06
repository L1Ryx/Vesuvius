using _ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;


public class ControlPromptUpdater : MonoBehaviour
{
    public PlayerInput m_PlayerInput;
    public ControlPrompts controlPrompts;

    [Header("Events")]
    public UnityEvent ControlsChanged;

    //called once at the start to set prompts in case there isn't a change
    void Start()
    {
        OnControlsChanged();
    }

    //currently replaces everything at once, could be slightly more performant if only replace what we need
    //but this is safer and more centralized.
    public void OnControlsChanged()
    {
        //replace all templates with inputs from new scheme
        controlPrompts.movePrompt = controlPrompts.movePromptTemplate.Replace("{Move}",
                        GetBindingDisplayStringOrCompositeName(m_PlayerInput.actions["Move"]));

        controlPrompts.jumpPrompt = controlPrompts.jumpPromptTemplate.Replace("{Jump}",
                        GetBindingDisplayStringOrCompositeName(m_PlayerInput.actions["Jump"]));

        controlPrompts.swingPrompt = controlPrompts.swingPromptTemplate.Replace("{Swing}",
                        GetBindingDisplayStringOrCompositeName(m_PlayerInput.actions["Swing"]));

        controlPrompts.healPrompt = controlPrompts.healPromptTemplate.Replace("{Heal}",
                        GetBindingDisplayStringOrCompositeName(m_PlayerInput.actions["Heal"]));

        controlPrompts.interactPrompt = controlPrompts.interactPromptTemplate.Replace("{Interact}",
            GetBindingDisplayStringOrCompositeName(m_PlayerInput.actions["Interact"]));

        controlPrompts.doubleJumpPrompt = controlPrompts.doubleJumpPromptTemplate.Replace("{Jump}",
            GetBindingDisplayStringOrCompositeName(m_PlayerInput.actions["Jump"]));

        controlPrompts.realityShiftPrompt = controlPrompts.realityShiftPromptTemplate.Replace("{RealityShift}",
            GetBindingDisplayStringOrCompositeName(m_PlayerInput.actions["RealityShift"]));

        controlPrompts.realityShiftKeybind = GetBindingDisplayStringOrCompositeName(m_PlayerInput.actions["RealityShift"]);

        ControlsChanged.Invoke();
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

    public string ReplaceRealityShiftText(string templateText)
    {
        return templateText.Replace("{RealityShift}",
            GetBindingDisplayStringOrCompositeName(m_PlayerInput.actions["RealityShift"]));
    }
}
