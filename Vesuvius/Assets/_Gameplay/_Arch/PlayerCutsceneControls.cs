using UnityEngine;

//Called by cutscenes to keep players from executing actions during cutscenes.
public class PlayerCutsceneControls : MonoBehaviour
{
    public void DisableNormalControls()
    {
        PlayerControlManager.Instance.DisableNormalControls();
    }

    public void EnableNormalControls()
    {
        PlayerControlManager.Instance.EnableNormalControls();
    }
}
