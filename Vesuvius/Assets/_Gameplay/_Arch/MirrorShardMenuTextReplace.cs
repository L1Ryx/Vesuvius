using _ScriptableObjects;
using TMPro;
using UnityEngine;

public class MirrorShardMenuTextReplace : MonoBehaviour
{
    public TextMeshProUGUI realityShiftText;
    public ControlPrompts controlPrompts;
    private string textTemplate;

    public void OnControlsChanged()
    {
        realityShiftText.text = textTemplate.Replace("{RealityShift}", controlPrompts.realityShiftKeybind);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textTemplate = realityShiftText.text;
        OnControlsChanged();
    }

}
